using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using TourDeFrance.Core.Exceptions;

namespace TourDeFrance.Core.Extensions
{
	// TODO: replace message by object (or field) name + standard message
	public static class ArgumentCheckExtensions
	{	
		/// <summary>
		/// Throw ArgumentNullException if param is null
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsNotNull(this object param, string message)
		{
			if (param == null)
			{
				throw new ArgumentNullException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentNullException if the param equals Guid.Empty
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsNotEmpty(this Guid param, string message)
		{
			if (param == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(param), message);
			}
		}

		#region String

		/// <summary>
		/// Throw ArgumentNullException if param is null, empty or contains only whitespaces
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsNotEmpty(this string param, string message)
		{
			if (string.IsNullOrWhiteSpace(param))
			{
				throw new ArgumentNullException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentException if param doesn't match regex.
		/// </summary>
		public static void EnsureMatchRegex(this string param, string regex, string message)
		{
			if (!Regex.IsMatch(param, regex))
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		public static void EnsureMatchRegex(this string param, Regex regex, string message)
		{
			if (!regex.IsMatch(param))
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		/// <summary>
		/// Throw ArgumentException if param contains forbidden chars.
		/// </summary>
		public static void EnsureDoesNotContain(this string param, char[] forbidden, string message)
		{
			foreach (var forbid in forbidden)
			{
				if (param.Contains(forbid))
				{
					throw new ArgumentException(message, nameof(param));
				}
			}
		}

		private static readonly EmailAddressAttribute EmailAddressAttribute = new EmailAddressAttribute();

		/// <summary>
		/// Throw ArgumentNullException if param is null
		/// Throw ArgumentException if param is not a valid email address
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsAValidEmailAddress(this string param, string message)
		{
			param.EnsureIsNotEmpty(message);
			if (!EmailAddressAttribute.IsValid(param))
			{
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		/// Throw ArgumentException if param is false
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsTrue(this bool param, string message)
		{
			if (!param)
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		/// <summary>
		/// Throw ArgumentException if param is true
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsFalse(this bool param, string message)
		{
			if (param)
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		/// <summary>
		/// Throw ArgumentException if the param is less than x chars
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="numberOfChars">the minimal required characters count</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsAtLeastNCaracter(this string param, int numberOfChars, string message)
		{
			if (param.Length < numberOfChars)
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		/// <summary>
		/// Throw ArgumentException if the param is not longer than x chars
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="numberOfChars">the maximal number characters allowed</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsAtMostNCaracter(this string param, int numberOfChars, string message)
		{
			if (!string.IsNullOrEmpty(param) && param.Length > numberOfChars)
			{
				throw new ArgumentException(message, nameof(param));
			}
		}

		#endregion

		#region Integer

		/// <summary>
		/// Throw ArgumentOutOfRangeException if the first param is smaller than the second
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="greater">The test value</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsSmallerThanOtherInt(this int param, int greater, string message)
		{
			if (param > greater)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentOutOfRangeException if param is &lt; 0
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsPositive(this int param, string message)
		{
			if (param < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentOutOfRangeException if param is not stricly &gt; 0
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsStrictlyPositive(this int param, string message)
		{
			if (param <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentOutOfRangeException if the param is not in the given range
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="min">Minimum value (not included)</param>
		/// <param name="max">Maximum value (not included)</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsInRange(this int param, int min, int max, string message)
		{
			if (param > max || param < min)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		#endregion

		/// <summary>
		/// Throw ArgumentOutOfRangeException if param is not stricly &gt; 0
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsStrictlyPositive(this decimal param, string message)
		{
			if (param <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentOutOfRangeException if param is not &gt;= 0
		/// </summary>
		/// <param name="param">The parameter to check</param>
		/// <param name="message">Exception message</param>
		public static void EnsureIsPositive(this decimal param, string message)
		{
			if (param < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		/// <summary>
		/// Throw ArgumentOutOfRangeException if the date has a value and is not in the given range
		/// </summary>
		/// <param name="param">The param to check</param>
		/// <param name="minDate">Minimum date (not included)</param>
		/// <param name="maxDate">Maximum date (not included)</param>
		public static void EnsurIsInRange(this DateTime param, DateTime minDate, DateTime maxDate)
		{
			if (param < minDate || param > maxDate)
			{
				throw new ArgumentOutOfRangeException(nameof(param), $"Date {param} must be between {minDate} and {maxDate}");
			}
		}

		public static void EnsureIsInPast(this DateTime param, string message)
		{
			if (param > DateTime.Today)
			{
				throw new ArgumentOutOfRangeException(nameof(param), message);
			}
		}

		public static void EnsureIsSecure(this string password)
		{
			foreach (string regex in Context.Current.Config.PasswordRegexes)
			{
				if (!Regex.IsMatch(password, regex))
				{
					throw new TourDeFranceException(Context.Current.Config.PasswordErrorMessage);
				}
			}
		}
	}
}
