using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NTvdb;
using Xunit;

namespace Tests
{
	public class Tests
	{
		[Fact]
		public void ParseTvdbResponse()
		{
			const string json =
		@"{
			""aliases"": [
				""Serenity""
			],
			""banner"": ""graphical/78874-g3.jpg"",
			""firstAired"": ""2002-09-20"",
			""id"": 78874,
			""network"": ""FOX (US)"",
			""overview"": ""In the far-distant future, Captain Malcolm \""Mal\"" Reynolds is a renegade former brown-coat sergeant, now turned smuggler & rogue, who is the commander of a small spacecraft, with a loyal hand-picked crew made up of the first mate, Zoe Warren; the pilot Hoban \""Wash\"" Washburn; the gung-ho grunt Jayne Cobb; the engineer Kaylee Frye; the fugitives Dr. Simon Tam and his psychic sister River. Together, they travel the far reaches of space in search of food, money, and anything to live on."",
			""seriesName"": ""Firefly"",
			""status"": ""Ended""
	   }";

			var searchResult = new TvdbSeriesSearchResult(DecodeJson<SearchResultSchema>(json));

			searchResult.Id.Should().Be(78874);
			searchResult.SeriesName.Should().Be("Firefly");
		}

		[Fact]
		public void ParseSeriesDetailsResponse()
		{
			const string json =
				@"{
    ""id"": 78874,
	 ""seriesName"": ""Firefly"",
    ""aliases"": [
      ""Serenity""
    ],
    ""banner"": ""graphical/78874-g3.jpg"",
    ""seriesId"": ""7097"",
    ""status"": ""Ended"",
    ""firstAired"": ""2002-09-20"",
    ""network"": ""FOX (US)"",
    ""networkId"": """",
    ""runtime"": ""45"",
    ""genre"": [
      ""Drama"",
      ""Science-Fiction""
    ],
    ""overview"": ""In the far-distant future, Captain Malcolm \""Mal\"" Reynolds is a renegade former brown-coat sergeant, now turned smuggler & rogue, who is the commander of a small spacecraft, with a loyal hand-picked crew made up of the first mate, Zoe Warren; the pilot Hoban \""Wash\"" Washburn; the gung-ho grunt Jayne Cobb; the engineer Kaylee Frye; the fugitives Dr. Simon Tam and his psychic sister River. Together, they travel the far reaches of space in search of food, money, and anything to live on."",
    ""lastUpdated"": 1468991697,
    ""airsDayOfWeek"": """",
    ""airsTime"": """",
    ""rating"": ""TV-14"",
    ""imdbId"": ""tt0303461"",
    ""zap2itId"": ""EP00524463"",
    ""added"": """",
    ""addedBy"": null,
    ""siteRating"": 9.5,
    ""siteRatingCount"": 465
  }";
	
			var series = new TvdbSeries(DecodeJson<SeriesDetailsSchema>(json));
			series.Id.Should().Be(78874);
			series.SeriesName.Should().Be("Firefly");
			series.Aliases.Should().Equal("Serenity");
			series.Genre.Should().Equal(new Genre("Drama"), new Genre("Science-Fiction"));
		}

		private static T DecodeJson<T>(string json)
		{
			var rawBytes = new MemoryStream(Encoding.ASCII.GetBytes(json));
			return rawBytes.ReadAsJson<T>();
		}
	}
}
