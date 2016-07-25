using System;
using System.Collections.Immutable;

namespace NTvdb
{
	public class TvdbSeriesSearchResult
	{
		public TvdbSeriesSearchResult(SearchResultSchema result)
		{
			Banner = result.banner;
			FirstAired = DateTime.Parse(result.firstAired);
			Id = result.id;
			SeriesName = result.seriesName;
			Overview = result.overview;
			Aliases = result.aliases.ToImmutableArray();
		}

		public int Id { get; }
		public string SeriesName { get; }
		public IImmutableList<string> Aliases { get; }
		public string Overview { get; }
		public DateTime FirstAired { get; }
		public string Banner { get; }
	}
}