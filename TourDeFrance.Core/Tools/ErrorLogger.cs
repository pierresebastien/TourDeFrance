﻿using System;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Core.Tools
{
	public class ErrorLogger
	{
		private static readonly ILog Logger = LogProvider.For<ErrorLogger>();

		// TODO: to review
		public static void LogException(Exception e, string message = "Exception : ")
		{
			Logger.Error(message, e);
		}
	}
}
