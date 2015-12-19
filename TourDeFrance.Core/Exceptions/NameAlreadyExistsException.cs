using System;

namespace TourDeFrance.Core.Exceptions
{
	[Serializable]
	public class NameAlreadyExistsException : TourDeFranceException
	{
		public NameAlreadyExistsException(string message)
			: base(message)
		{
		}

		public NameAlreadyExistsException()
		{
		}

		public NameAlreadyExistsException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}