using System;
using System.Linq;
using ServiceStack.Text;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("config")]
	public class DbConfig : Auditable, IOrderable
	{
		[Index(Unique = true)]
		[Alias("key")]
		public string Key { get; set; }

		[Alias("value")]
		public string Value { get; set; }

		[Alias("default_value")]
		public string DefaultValue { get; set; }

		[Alias("display_name")]
		public string DisplayName { get; set; }

		[Alias("description")]
		public string Description { get; set; }

		[Alias("validation_regex")]
		public string ValidationRegex { get; set; }

		[Alias("type")]
		public ConfigType Type { get; set; }

		[Alias("order")]
		public int Order { get; set; }

		[Alias("dangerous")]
		public bool Dangerous { get; set; }

		public string ToDatabaseValue<T>(T value)
		{
			CheckType(value.GetType());
			switch (Type)
			{
				case ConfigType.String:
				case ConfigType.Integer:
					return value.ToString();
				case ConfigType.Boolean:
					return value.ToString().ToLower();
				case ConfigType.Array:
					return value.ToJson();
				default:
					throw new ArgumentOutOfRangeException(nameof(value), $"Invalid config type : {Type}");
			}
		}

		public T FromDatabaseValue<T>()
		{
			CheckType(typeof(T));
			switch (Type)
			{
				case ConfigType.String:
					return (T)Convert.ChangeType(Value, typeof(T));
				case ConfigType.Integer:
				case ConfigType.Boolean:
					return Value.Parse<T>();
				case ConfigType.Array:
					return Value.FromJson<T>();
				default:
					throw new ArgumentOutOfRangeException(string.Empty, $"Invalid config type : {Type}");
			}
		}

		public void CheckType(Type type)
		{
			Type[] possibleTypes;
			switch (Type)
			{
				case ConfigType.String:
					possibleTypes = new[] { typeof(string) };
					break;
				case ConfigType.Integer:
					possibleTypes = new[] { typeof(int), typeof(int?) };
					break;
				case ConfigType.Boolean:
					possibleTypes = new[] { typeof(bool), typeof(bool?) };
					break;
				case ConfigType.Array:
					possibleTypes = new[] { typeof(string[]) };
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), $"Unknown config type : '{type}'");
			}
			if (!possibleTypes.ToList().Contains(type))
			{
				throw new TourDeFranceException($"Object type '{type.Name}' is not supported for config type '{Type}'");
			}
		}

		public Client.Config.Config ToModel()
		{
			object value;
			switch (Type)
			{
				case ConfigType.String:
					value = Value;
					break;
				case ConfigType.Integer:
					value = int.Parse(Value);
					break;
				case ConfigType.Boolean:
					value = bool.Parse(Value);
					break;
				case ConfigType.Array:
					value = Value.FromJson<string[]>();
					break;
				default:
					throw new ArgumentOutOfRangeException(string.Empty, $"Invalid config type : {Type}");
			}
			return new Client.Config.Config
			{
				Key = Key,
				StringValue = Value,
				Value = value,
				Order = Order,
				Type = Type,
				Dangerous = Dangerous,
				DefaultValue = DefaultValue,
				Description = Description,
				DisplayName = DisplayName,
				ValidationRegex = ValidationRegex
			};
		}
	}
}
