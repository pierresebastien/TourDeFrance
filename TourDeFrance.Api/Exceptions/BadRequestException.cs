﻿using System;
using Nancy.Validation;

namespace TourDeFrance.Api.Exceptions
{
	[Serializable]
	public class BadRequestException : Exception
	{
		public ModelValidationResult ModelValidationResult { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BadRequestException"/> class
		/// </summary>
		public BadRequestException()
		{
		}

		public BadRequestException(ModelValidationResult modelValidationResult)
		{
			ModelValidationResult = modelValidationResult;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BadRequestException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public BadRequestException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BadRequestException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public BadRequestException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
