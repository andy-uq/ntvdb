using System;

namespace NTvdb
{
	public sealed class Genre : IEquatable<Genre>
	{
		public static readonly Genre None = new Genre();

		private Genre()
		{
			Name = "(None)";
		}

		public Genre(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
		}

		public string Name { get; }

		public bool Equals(Genre other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is Genre && Equals((Genre) obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(Genre left, Genre right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Genre left, Genre right)
		{
			return !Equals(left, right);
		}

		public override string ToString() => Name;
	}
}