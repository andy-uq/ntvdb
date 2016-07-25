using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nito.AsyncEx;

namespace NTvdb
{
	public interface ITvdbApi
	{
		Task<TvdbSeriesSearchResult> SearchSeriesAsync(string name, CancellationToken cancellationToken);
	}

	public static class HttpClientMethods
	{
		public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
		{
			return ReadAsJson<T>(await content.ReadAsStreamAsync());
		}

		public static async Task<T> ReadAsAnonymousJsonAsync<T>(this HttpContent content, T schema)
		{
			return ReadAsJson<T>(await content.ReadAsStreamAsync());
		}

		public static T ReadAsJson<T>(this Stream stream)
		{
			var serialiser = new JsonSerializer();
			using (var reader = new JsonTextReader(new StreamReader(stream)))
			{
				return serialiser.Deserialize<T>(reader);
			}
		}
	}

	public class SearchResultSchema
	{
		public string[] aliases { get; set; }
		public string banner { get; set; }
		public string firstAired { get; set; }
		public int id { get; set; }
		public string network { get; set; }
		public string overview { get; set; }
		public string seriesName { get; set; }
		public string status { get; set; }
	}

	public class TvdbApi : ITvdbApi
	{
		private readonly AsyncLazy<HttpClient> _httpClient;

		public TvdbApi(string baseUrl, TvdbKeys tvdbKeys)
		{
			_httpClient = new AsyncLazy<HttpClient>(() => CreateTvdbClient(baseUrl, tvdbKeys));
		}

		public async Task<TvdbSeriesSearchResult> SearchSeriesAsync(string name, CancellationToken cancellationToken)
		{
			var search = new TvdbSearchRequest(name);
			var request = new HttpRequestMessage(HttpMethod.Post, "/search/series")
			{
				Content = new StringContent(JsonConvert.SerializeObject(search))
			};

			var response = await SendAsync(request, cancellationToken);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadAsJsonAsync<SearchResultSchema>();
			return new TvdbSeriesSearchResult(result);
		}

		private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var client = await _httpClient;
			var response = await client.SendAsync(request, cancellationToken);
			response.EnsureSuccessStatusCode();

			return response;
		}

		private async Task<HttpClient> CreateTvdbClient(string baseUrl, TvdbKeys tvdbKeys)
		{
			var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
			client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

			var jwt = await GetJwtToken(tvdbKeys, client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			return client;
		}

		private async Task<string> GetJwtToken(TvdbKeys tvdbKeys, HttpClient client)
		{
			var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
			{
				Content = new StringContent(JsonConvert.SerializeObject(tvdbKeys), Encoding.ASCII, "application/json")
			};

			var response = await client.SendAsync(loginRequest);
			response.EnsureSuccessStatusCode();

			var schema = new { token = "" };
			var jwt = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), schema);

			return jwt.token;
		}
	}
}