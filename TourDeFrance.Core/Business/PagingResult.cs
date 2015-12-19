using System.Collections.Generic;

namespace TourDeFrance.Core.Business
{
	public class PagingResult<T>
	{
		public IEnumerable<T> Results { get; set; }

		public int Offset { get; set; }

		public int Max { get; set; }

		public int Total { get; set; }
	}
}