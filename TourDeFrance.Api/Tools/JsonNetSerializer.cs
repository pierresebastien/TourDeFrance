using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nancy;

namespace TourDeFrance.Api.Tools
{
	public class JsonNetSerializer : ISerializer
	{
		public IEnumerable<string> Extensions
		{
			get { throw new NotImplementedException(); }
		}

		private readonly JsonSerializer _serializer;

		public JsonNetSerializer()
		{
			var settings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};

			_serializer = JsonSerializer.Create(settings);
		}

		public bool CanSerialize(string contentType)
		{
			return contentType == "application/json";
		}

		public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
		{
			using (var writer = new JsonTextWriter(new StreamWriter(outputStream)))
			{
				_serializer.Serialize(writer, model);
				writer.Flush();
			}
		}
	}
}
