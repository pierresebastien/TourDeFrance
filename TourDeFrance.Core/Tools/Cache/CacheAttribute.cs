using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CacheMethodResult.Attributes;
using MethodDecorator.Attributes;
using TourDeFrance.Core.Extensions;

namespace TourDeFrance.Core.Tools.Cache
{
	public class CacheAttribute : BaseCacheAttribute
	{
		public CacheAttribute()
		{
			ArgumentOrder = -1;
			CacheValidityInMinutes = -1;
		}

		public string CacheKey { get; set; }

		public int ArgumentOrder { get; set; }

		public int CacheValidityInMinutes { get; set; }

		public override T Retrieve<T>(MethodBase method, object[] args)
		{
			if(string.IsNullOrWhiteSpace(CacheKey))
			{
				CacheKey = ((MethodInfo)method).ReturnType.Name;
			}
			return Context.Current.Cache.Get<T>(CacheKey.GenerateCacheKey(ArgumentOrder, args));
		}

		public override void Store<T>(T t, MethodBase method, object[] args)
		{
			string key = CacheKey.GenerateCacheKey(ArgumentOrder, args);
			if (CacheValidityInMinutes > 0)
			{
				Context.Current.Cache.Set(key, t, TimeSpan.FromMinutes(CacheValidityInMinutes));
			}
			else
			{
				Context.Current.Cache.Set(key, t);
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class InvalidateCacheAttribute : DecoratorAttribute
	{
		public const int NoArgumentOrder = -1;

		public IDictionary<string, int?> CacheKeys;

		// TODO: to remove???
		public InvalidateCacheAttribute()
		{
		}

		public InvalidateCacheAttribute(string[] names = null, int[] nameArgumentOrders = null, Type[] types = null, int[] typeArgumentOrders = null)
		{
			CacheKeys = new Dictionary<string, int?>();
			if(names != null)
			{
				if(nameArgumentOrders == null)
				{
					throw new ArgumentException(); // TODO:
				}
				if(names.Length != nameArgumentOrders.Length)
				{
					throw new ArgumentException(); // TODO:
				}
				for(int i = 0; i < names.Length; i++)
				{
					if(nameArgumentOrders[i] < NoArgumentOrder)
					{
						throw new ArgumentException(); // TODO:
					}
					int? argumentOrder = nameArgumentOrders[i] == NoArgumentOrder ? (int?) null : nameArgumentOrders[i];
					CacheKeys.Add(names[i], argumentOrder);
				}
			}
			if(types != null)
			{
				if(typeArgumentOrders == null)
				{
					throw new ArgumentException(); // TODO:
				}
				if(types.Length != typeArgumentOrders.Length)
				{
					throw new ArgumentException(); // TODO:
				}
				for(int i = 0; i < types.Length; i++)
				{
					if(typeArgumentOrders[i] < NoArgumentOrder)
					{
						throw new ArgumentException(); // TODO:
					}
					int? argumentOrder = typeArgumentOrders[i] == NoArgumentOrder ? (int?) null : typeArgumentOrders[i];
					CacheKeys.Add(types[i].Name, argumentOrder);
				}
			}
		}

		// TODO : private set??
		public string[] Names { get; set; }

		public Type[] Types { get; set; }

		public int[] NameArgumentOrders { get; set; }

		public int[] TypeArgumentOrders { get; set; }

		public override void OnEntry(MethodBase method, object[] args)
		{
		}

		public override void OnExit(object returnValue, MethodBase method, object[] args)
		{
			Context.Current.Cache.RemoveAll(CacheKeys.Keys.Select(x => x.GenerateCacheKey(CacheKeys[x] ?? 0, args)));
		}

		public override void OnException(Exception exception, MethodBase method, object[] args)
		{
		}
	}
}