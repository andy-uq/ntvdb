using System;
using System.IO;
using System.Linq;
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
		Task<TvdbSeriesSearchResult[]> SearchSeriesAsync(string name, CancellationToken cancellationToken);
		Task<TvdbSeries> GetSeries(int seriesId, CancellationToken cancellationToken);
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

	public class TvdbApi : ITvdbApi
	{
		private readonly AsyncLazy<HttpClient> _httpClient;
		private readonly JsonSerializer _serialiser;

		public TvdbApi(string baseUrl, TvdbKeys tvdbKeys)
		{
			_httpClient = new AsyncLazy<HttpClient>(() => CreateTvdbClient(baseUrl, tvdbKeys));
			_serialiser = new JsonSerializer {ContractResolver = new CamelCasePropertyNamesContractResolver()};
		}

		public async Task<TvdbSeriesSearchResult[]> SearchSeriesAsync(string name, CancellationToken cancellationToken)
		{
			name = Uri.EscapeDataString(name);

			var result = await SendAsync<SearchResultSchema[]>(() => new HttpRequestMessage(HttpMethod.Get, $"/search/series?name={name}"), cancellationToken);
			return result.Select(r => new TvdbSeriesSearchResult(r)).ToArray();
		}

		public async Task<TvdbSeries> GetSeries(int seriesId, CancellationToken cancellationToken)
		{
			var result = await SendAsync<SeriesDetailsSchema>(() => new HttpRequestMessage(HttpMethod.Get, $"/series/{seriesId}"), cancellationToken);
			return new TvdbSeries(result);
		}

		private HttpRequestMessage BuildPostRequest(object content, string relativeUrl)
		{
			var stream = new MemoryStream();
			var writer = new JsonTextWriter(new StreamWriter(stream));
			_serialiser.Serialize(writer, content);

			var httpContent = new StreamContent(stream) { Headers = { ContentType = MediaTypeHeaderValue.Parse("application/json") } };
			return new HttpRequestMessage(HttpMethod.Post, relativeUrl)
			{
				Content = httpContent
			};
		}

		private async Task<T> SendAsync<T>(Func<HttpRequestMessage> requestBuilder, CancellationToken cancellationToken)
		{
			using (var request = requestBuilder())
			{
				using (var response = await SendAsync(request, cancellationToken))
				{
					response.EnsureSuccessStatusCode();

					var schema = new {data = default(T)};

					var result = await response.Content.ReadAsAnonymousJsonAsync(schema);
					return result.data;
				}
			}
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
			var loginJson = JsonConvert.SerializeObject(new { apikey = tvdbKeys.ApiKey, username = tvdbKeys.Username, userkey = tvdbKeys.UserKey });
			var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
			{
				Content = new StringContent(loginJson)
				{
					Headers = { ContentType = MediaTypeHeaderValue.Parse("application/json") }
				}
			};

			var response = await client.SendAsync(loginRequest);
			response.EnsureSuccessStatusCode();

			var schema = new { token = "" };
			var jwt = await response.Content.ReadAsAnonymousJsonAsync(schema);

			return jwt.token;
		}
	}
}