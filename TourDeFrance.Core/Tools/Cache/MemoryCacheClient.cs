using System;
using System.Collections.Generic;
using ServiceStack.CacheAccess;
using ServiceStack.Net30.Collections.Concurrent;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Core.Tools.Cache
{
	public sealed class MemoryCacheClient : ICacheClient
	{
		private static readonly ILog Log = LogProvider.For<MemoryCacheClient>();

		private ConcurrentDictionary<string, CacheEntry> _memory;
		private ConcurrentDictionary<string, int> _counters;

		public bool FlushOnDispose { get; set; }

		private class CacheEntry
		{
			private object _cacheValue;

			public CacheEntry(object value, DateTime expiresAt)
			{
				Value = value;
				ExpiresAt = expiresAt;
				LastModifiedTicks = DateTime.Now.Ticks;
			}

			internal DateTime ExpiresAt { get; set; }

			internal object Value
			{
				get { return _cacheValue; }
				set
				{
					_cacheValue = value;
					LastModifiedTicks = DateTime.Now.Ticks;
				}
			}

			internal long LastModifiedTicks { get; private set; }
		}

		public MemoryCacheClient()
		{
			_memory = new ConcurrentDictionary<string, CacheEntry>();
			_counters = new ConcurrentDictionary<string, int>();
		}

		private bool CacheAdd(string key, object value)
		{
			return CacheAdd(key, value, DateTime.MaxValue);
		}

		private bool TryGetValue(string key, out CacheEntry entry)
		{
			return _memory.TryGetValue(key, out entry);
		}

		private void Set(string key, CacheEntry entry)
		{
			_memory[key] = entry;
		}

		private bool CacheAdd(string key, object value, DateTime expiresAt)
		{
			CacheEntry entry;
			if (TryGetValue(key, out entry)) return false;

			entry = new CacheEntry(value, expiresAt);
			Set(key, entry);

			return true;
		}

		private bool CacheSet(string key, object value)
		{
			return CacheSet(key, value, DateTime.MaxValue);
		}

		private bool CacheSet(string key, object value, DateTime expiresAt, long? checkLastModified = null)
		{
			CacheEntry entry;
			if (!TryGetValue(key, out entry))
			{
				entry = new CacheEntry(value, expiresAt);
				Set(key, entry);
				return true;
			}

			if (checkLastModified.HasValue
			&& entry.LastModifiedTicks != checkLastModified.Value) return false;

			entry.Value = value;
			entry.ExpiresAt = expiresAt;

			return true;
		}

		private bool CacheReplace(string key, object value)
		{
			return CacheReplace(key, value, DateTime.MaxValue);
		}

		private bool CacheReplace(string key, object value, DateTime expiresAt)
		{
			return !CacheSet(key, value, expiresAt);
		}

		public void Dispose()
		{
			if (!FlushOnDispose) return;

			_memory = new ConcurrentDictionary<string, CacheEntry>();
			_counters = new ConcurrentDictionary<string, int>();
		}

		public bool Remove(string key)
		{
			CacheEntry item;
			return _memory.TryRemove(key, out item);
		}

		public void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				try
				{
					Remove(key);
				}
				catch (Exception ex)
				{
					Log.Error($"Error trying to remove {key} from the cache", ex);
				}
			}
		}

		public object Get(string key)
		{
			long lastModifiedTicks;
			return Get(key, out lastModifiedTicks);
		}

		public object Get(string key, out long lastModifiedTicks)
		{
			lastModifiedTicks = 0;

			CacheEntry cacheEntry;
			if (_memory.TryGetValue(key, out cacheEntry))
			{
				if (cacheEntry.ExpiresAt < DateTime.Now)
				{
					_memory.TryRemove(key, out cacheEntry);
					return null;
				}
				lastModifiedTicks = cacheEntry.LastModifiedTicks;
				return cacheEntry.Value;
			}
			return null;
		}

		public T Get<T>(string key)
		{
			var value = Get(key);
			if (value != null) return (T)value;
			return default(T);
		}

		private int UpdateCounter(string key, int value)
		{
			if (!_counters.ContainsKey(key))
			{
				_counters[key] = 0;
			}
			_counters[key] += value;
			return _counters[key];
		}

		public long Increment(string key, uint amount)
		{
			return UpdateCounter(key, 1);
		}

		public long Decrement(string key, uint amount)
		{
			return UpdateCounter(key, -1);
		}

		public bool Add<T>(string key, T value)
		{
			return CacheAdd(key, value);
		}

		public bool Set<T>(string key, T value)
		{
			return CacheSet(key, value);
		}

		public bool Replace<T>(string key, T value)
		{
			return CacheReplace(key, value);
		}

		public bool Add<T>(string key, T value, DateTime expiresAt)
		{
			return CacheAdd(key, value, expiresAt);
		}

		public bool Set<T>(string key, T value, DateTime expiresAt)
		{
			return CacheSet(key, value, expiresAt);
		}

		public bool Replace<T>(string key, T value, DateTime expiresAt)
		{
			return CacheReplace(key, value, expiresAt);
		}

		public bool Add<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheAdd(key, value, DateTime.Now.Add(expiresIn));
		}

		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheSet(key, value, DateTime.Now.Add(expiresIn));
		}

		public bool Replace<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheReplace(key, value, DateTime.Now.Add(expiresIn));
		}

		public void FlushAll()
		{
			_memory = new ConcurrentDictionary<string, CacheEntry>();
		}

		public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
		{
			var valueMap = new Dictionary<string, T>();
			foreach (var key in keys)
			{
				var value = Get<T>(key);
				valueMap[key] = value;
			}
			return valueMap;
		}

		public void SetAll<T>(IDictionary<string, T> values)
		{
			foreach (var entry in values)
			{
				Set(entry.Key, entry.Value);
			}
		}
	}
}
