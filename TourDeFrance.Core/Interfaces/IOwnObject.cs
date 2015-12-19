using System;

namespace TourDeFrance.Core.Interfaces
{
	public interface IOwnObject
	{
		Guid Owner { get; set; }
	}
}
