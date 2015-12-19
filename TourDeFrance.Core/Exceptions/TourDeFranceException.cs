using System;

namespace TourDeFrance.Core.Exceptions
{
	[Serializable]
	public class TourDeFranceException : Exception
	{
		public TourDeFranceException(string message)
			: base(message)
		{
		}

		public TourDeFranceException()
		{
		}

		public TourDeFranceException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}