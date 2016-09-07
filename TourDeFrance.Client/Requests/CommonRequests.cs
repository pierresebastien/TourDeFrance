using System;

namespace TourDeFrance.Client.Requests
{
	public class ObjectByIdRequest
	{
		public string Id { get; set; }
	}

	public class ObjectByGuidRequest
	{
		public Guid Id { get; set; }
	}
}
