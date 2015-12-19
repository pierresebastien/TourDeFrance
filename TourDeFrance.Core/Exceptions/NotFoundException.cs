using System;

namespace TourDeFrance.Core.Exceptions
{
	[Serializable]
	public class NotFoundException : TourDeFranceException
	{
		public NotFoundException(string message) : base(message)
		{
		}

		public NotFoundException()
		{
		}

		public NotFoundException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}