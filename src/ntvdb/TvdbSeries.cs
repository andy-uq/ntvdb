using System;
using System.Collections.Immutable;
using System.Linq;

namespace NTvdb
{
	public class TvdbSeries
	{
		public TvdbSeries(SeriesDetailsSchema result)
		{
			Banner = result.banner;
			FirstAired = DateTime.Parse(result.firstAired);
			Id = result.id;
			SeriesId = result.seriesId;
			SeriesName = result.seriesName;
			Overview = result.overview;
			Aliases = result.aliases.ToImmutableArray();
			Genre = result.genre.Select(g => new Genre(g)).ToImmutableArray();
		}

		public int Id { get; set; }
		public string SeriesName { get; set; }
		public IImmutableList<string> Aliases { get; set; }
		public string Banner { get; set; }
		public string SeriesId { get; set; }
		public string Status { get; set; }
		public DateTime FirstAired { get; set; }
		public string Network { get; set; }
		public string NetworkId { get; set; }
		public string Runtime { get; set; }
		public IImmutableList<Genre> Genre { get; set; }
		public string Overview { get; set; }
		public int LastUpdated { get; set; }
		public string AirsDayOfWeek { get; set; }
		public string AirsTime { get; set; }
		public string Rating { get; set; }
		public string ImdbId { get; set; }
		public string Zap2ItId { get; set; }
		public string Added { get; set; }
		public double SiteRating { get; set; }
		public int SiteRatingCount { get; set; }
	}
}