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

			var rawBytes = new MemoryStream(Encoding.ASCII.GetBytes(json));
			var searchResult = new TvdbSeriesSearchResult(rawBytes.ReadAsJson<SearchResultSchema>());

			searchResult.Id.Should().Be(78874);
			searchResult.SeriesName.Should().Be("Firefly");
		}
	}
}
