using System;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace TourDeFrance.ASP.Common.Modules
{
	/// <summary>
	///     A module for handling conditional GET requests.
	/// </summary>
	public class ConditionalGetModule : IHttpModule
	{
		/// <summary>
		///     A regular expression for filtering content types.
		/// </summary>
		private static readonly Regex ContentTypeFilter = new Regex("text/*|image/*|application/javascript",
			RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

		/// <summary>
		///     Processes the response and sets a response filter for conditional GETs.
		/// </summary>
		/// <param name="sender">The HTTP application of the current request.</param>
		/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
		private void ProcessResponse(object sender, EventArgs e)
		{
			var context = ((HttpApplication)sender).Context;
			var contentType = context.Response.ContentType;

			if (context.Request.HttpMethod == "GET" && ContentTypeFilter.IsMatch(contentType) &&
			   context.Response.StatusCode == 200)
			{
				context.Response.Filter = new ConditionalGetFilterStream(context);
			}
		}

		/// <summary>
		///     Disposes of the resources (other than memory) used by the module that implements
		///     <see cref="T:System.Web.IHttpModule" />.
		/// </summary>
		public void Dispose()
		{
			// Nothing to dispose.
		}

		/// <summary>
		///     Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">
		///     An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties,
		///     and events common to all application objects within an ASP.NET application
		/// </param>
		public void Init(HttpApplication context)
		{
			context.PostReleaseRequestState += ProcessResponse;
		}
	}

	/// <summary>
	///     A respons filter for handling conditional GETs using the Etag header.
	/// </summary>
	public class ConditionalGetFilterStream : BaseFilterStream
	{
		/// <summary>
		///     The HTTP context in which this filter should be applied.
		/// </summary>
		private readonly HttpContext _context;

		/// <summary>
		///     The hash algorithm used to calculate the hash code of the response.
		/// </summary>
		private static readonly HashAlgorithm HashAlgorithm = new MD5CryptoServiceProvider();

		/// <summary>
		///     Used for mutexing when accessing the hashAlgorithm
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		///     Gets or sets a value indicating whether to ignore requests that are too large to be garbage collected efficiently.
		/// </summary>
		/// <value>
		///     <c>true</c> if large response should be ignored; otherwise, <c>false</c>.
		/// </value>
		protected override bool IgnoreLargeResponse
		{
			get { return false; }
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionalGetFilterStream" /> class.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		public ConditionalGetFilterStream(HttpContext context)
		{
			Sink = context.Response.Filter;
			_context = context;
		}

		/// <summary>
		///     Process and manipulate the buffer.
		/// </summary>
		/// <param name="buffer">
		///     An array of bytes. This method copies <paramref name="count" /> bytes from
		///     <paramref name="buffer" /> to the current stream.
		/// </param>
		/// <param name="offset">
		///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
		///     current stream.
		/// </param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		protected override void Process(byte[] buffer, int offset, int count)
		{
			if (_context.Items["optimizer.conditionalget"] == null)
			{
				try
				{
					lock (SyncRoot)
					{
						var etag = Convert.ToBase64String(HashAlgorithm.ComputeHash(buffer, offset, count));
						SetConditionalGetHeaders(etag, _context);
						_context.Items.Add("optimizer.conditionalget", 1);
					}
				}
				catch (HttpException)
				{
					// The response have been manually flushed and no headers can be added to the response.
				}
			}

			Sink.Write(buffer, offset, count);
		}

		/// <summary>
		/// Sets the Etag for the conditional get headers.
		/// </summary>
		/// <param name="etag">The etag to send to the response.</param>
		/// <param name="context">The HTTP context.</param>
		public static void SetConditionalGetHeaders(string etag, HttpContext context)
		{
			string ifNoneMatch = context.Request.Headers["If-None-Match"];
			etag = "\"" + etag + "\"";

			if (ifNoneMatch != null && ifNoneMatch.Contains(","))
			{
				ifNoneMatch = ifNoneMatch.Substring(0, ifNoneMatch.IndexOf(",", StringComparison.Ordinal));
			}

			context.Response.AppendHeader("Etag", etag);
			context.Response.Cache.VaryByHeaders["If-None-Match"] = true;

			if (etag == ifNoneMatch)
			{
				context.Response.ClearContent();
				context.Response.StatusCode = (int)HttpStatusCode.NotModified;
				context.Response.SuppressContent = true;
			}
		}
	}
}