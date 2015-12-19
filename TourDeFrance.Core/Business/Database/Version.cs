using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("versions")]
	public class DbVersion
	{
		[PrimaryKey]
		[Alias("major")]
		public int Major { get; set; }

		[PrimaryKey]
		[Alias("minor")]
		public int Minor { get; set; }

		[PrimaryKey]
		[Alias("patch")]
		public int Patch { get; set; }
	}
}