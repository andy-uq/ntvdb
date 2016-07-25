namespace NTvdb
{
	public class TvdbSearchRequest
	{
		public TvdbSearchRequest(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}