using System;

namespace TourDeFrance.Client.User
{
	public class AccessShare
	{
		public Guid Id { get; set; }

		public Guid SharingUserId { get; set; }

		public string SharingFirstName { get; set; }

		public string SharingLastName { get; set; }

		public string SharingUsername { get; set; }

		public Guid SharedUserId { get; set; }
		
		public string SharedUsername { get; set; }
		
		public string SharedFirstname { get; set; }

		public string SharedLastName { get; set; }
	}
}
