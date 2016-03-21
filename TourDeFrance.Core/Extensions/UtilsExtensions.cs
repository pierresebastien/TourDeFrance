using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Core.Extensions
{
	public static class UtilsExtension
	{
		public static T Parse<T>(this string s)
		{
			MethodInfo method = typeof (T).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(x => x.Name.EqualsInvariantIgnoreCase("Parse") && x.GetParameters().Length == 1)
				.FirstOrDefault(x => x.GetParameters().First().ParameterType == typeof (string));
			if (method == null)
			{
				throw new ArgumentException("No method to parse a string found for type '{0}'", typeof (T).Name);
			}
			return (T) method.Invoke(null, new object[] {s});
		}

		public static T SetOwner<T>(this T obj) where T : IOwnObject
		{
			obj.Owner = Context.Current.User.Id;
			return obj;
		}

		#region Autofac

		public static void RegisterFactory<T>(this ContainerBuilder builder, IEnumerable<T> objects)
			where T : Auditable
		{
			RegisterFactory<T, Guid>(builder, objects);
		}

		public static void RegisterFactory<T, TId>(this ContainerBuilder builder, IEnumerable<T> objects)
			where T : class, IIdentifiable<TId>
		{
			RegisterFactory(builder, objects, x => x.Id);
		}

		public static void RegisterFactory<T, TId>(this ContainerBuilder builder, IEnumerable<T> objects, Func<T, TId> getKey)
			where T : class
		{
			IList<TId> ids = new List<TId>();
			foreach (var o in objects)
			{
				TId id = getKey(o);
				if (ids.Contains(id))
				{
					throw new TypeLoadException($"There is more than one '{typeof (T).Name}' with id '{id}'");
				}
				ids.Add(id);
				builder.RegisterInstance(o).As<T>().Keyed<T>(id).SingleInstance();
			}
		}

		#endregion

		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			return interfaceType.IsInterface && type.GetInterfaces().Any(t => t == interfaceType);
		}

		public static bool IsSameOrSubclass(this Type type, Type baseType)
		{
			return baseType.IsClass && (type == baseType || type.IsSubclassOf(baseType));
		}

		#region Cache

		public static string GenerateCacheKey<T>(this object key)
		{
			return GenerateCacheKey(typeof(T).Name, key);
		}

		public static string GenerateCacheKey(this string objectName, object key)
		{
			string cacheKey = objectName;
			if (key != null)
			{
				cacheKey += ":" + key;
			}
			return cacheKey.FormatCacheKey();
		}

		public static string GenerateCacheKey(this string objectName, int argumentOrder, object[] args)
		{
			string cacheKey = objectName;
			if (argumentOrder >= 0)
			{
				cacheKey += ":" + args[argumentOrder];
			}
			return cacheKey.FormatCacheKey();
		}

		public static string FormatCacheKey(this string key)
		{
			return $"{Constants.BASE_CACHE_KEY}:{key}";
		}

		public static string FormatQueueKey(this string key)
		{
			return $"{Constants.BASE_QUEUE_KEY}:{key}";
		}

		#endregion

		#region Drink

		public static decimal CalculateDrinkVolume(this decimal? volume, IList<SubDrink> subDrinks)
		{
			decimal result = volume ?? 0;
			if (subDrinks.Any())
			{
				result = subDrinks.Sum(x => x.Volume);
			}
			return result;
		}

		public static decimal CalculateDrinkAlcoholByVolume(this decimal? alcoholByVolume, IList<SubDrink> subDrinks)
		{
			decimal result = alcoholByVolume ?? 0;
			if (subDrinks.Any())
			{
				decimal calculatedVolume = CalculateDrinkAlcoholByVolume(null, subDrinks);
				if (calculatedVolume != 0)
				{
					result = subDrinks.Sum(x => x.Volume*x.AlcoholByVolume ?? 0)/calculatedVolume;
				}
			}
			return result;
		}

		#endregion

		public static void Error(this ILog logger, string message, Exception e)
		{
			logger.Error($"{message}{e.Message}\nStacktrace : {e.StackTrace}");
		}
	}
}