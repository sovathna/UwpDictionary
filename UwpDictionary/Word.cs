using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UwpDictionary
{
	public class WordUi
	{
		public int Id { get; set; }
		public string Value { get; set; }
	}

	[Table("words")]
	public class Word
	{
		[Key][Column("id")] public int Id { get; set; }

		[Column("value")][Required] public string Value { get; set; }

		[Column("definition")][Required] public string Definition { get; set; }
	}

	[Table("favorites")]
	public class Favorite
	{
		[Key][Column("id")] public int Id { get; set; }

		[Column("word_id")][Required] public int WordId { get; set; }

		[Column("value")][Required] public string Value { get; set; }
	}

	[Table("histories")]
	public class History
	{
		[Key][Column("id")] public int Id { get; set; }

		[Column("word_id")] public int WordId { get; set; }

		[Column("value")] public string Value { get; set; }
	}
}