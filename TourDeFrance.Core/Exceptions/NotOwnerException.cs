using System;

namespace TourDeFrance.Core.Exceptions
{
	[Serializable]
	public class NotOwnerException : TourDeFranceException
	{
		public NotOwnerException(string message)
			: base(message)
		{
		}

		public NotOwnerException()
		{
		}

		public NotOwnerException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}