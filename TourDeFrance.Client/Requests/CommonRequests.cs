using System;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class ObjectByIdRequest : IIdentifiable<string>
	{
		public string Id { get; set; }
	}

	public class ObjectByGuidRequest : IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
