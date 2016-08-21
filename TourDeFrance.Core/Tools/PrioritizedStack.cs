using System.Collections.Generic;
using ServiceStack.Net30.Collections.Concurrent;
using ServiceStack.Redis;
using TourDeFrance.Core.Extensions;

namespace TourDeFrance.Core.Tools
{
	// TODO: transactionnal ?? linked to transaction
	public interface IPrioritizedStack
	{
		void AddItem(string set, string value, double score);

		string PopItem(string set);

		void FlushAll(string set);
	}

	public class RedisPrioritizedStack : IPrioritizedStack
	{
		/*
			using (var scope = new TransactionScope())
			{
				scope.RedisTransaction.QueueCommand(x => x.AddItemToSortedSet(IndexHistorySetName, log.Id.ToString(), 1));
				scope.Complete();
			}
		*/

		private readonly IRedisClientsManager _redisClientsManager;

		public RedisPrioritizedStack(IRedisClientsManager redisClientsManager)
		{
			_redisClientsManager = redisClientsManager;
		}

		public void AddItem(string set, string value, double score)
		{
			using (var client = _redisClientsManager.GetClient())
			{
				client.AddItemToSortedSet(set, value, score);
			}
		}

		public string PopItem(string set)
		{
			using (var client = _redisClientsManager.GetClient())
			{
				return client.PopItemWithLowestScoreFromSortedSet(set);
			}
		}

		public void FlushAll(string set)
		{
			using (var client = _redisClientsManager.GetClient())
			{
				client.RemoveAll(client.SearchKeys($"{set}:*"));
			}
		}
	}

	public class InMemoryPrioritizedStack : IPrioritizedStack
	{
		private class Element
		{
			public string Value { get; set; }

			public double Score { get; set; }
		}

		private readonly IComparer<Element> _comparer;
		private readonly IDictionary<string, C5.IntervalHeap<Element>> _memory;

		public InMemoryPrioritizedStack()
		{
			_memory = new ConcurrentDictionary<string, C5.IntervalHeap<Element>>();
			_comparer = Comparer<Element>.Create((x, y) => x?.Score.CompareTo(y.Score) ?? (y == null ? 0 : -1));
		}

		public void AddItem(string set, string value, double score)
		{
			if (!_memory.ContainsKey(set))
			{
				_memory.Add(set, new C5.IntervalHeap<Element>(_comparer));
			}
			_memory[set].Add(new Element { Value = value, Score = score});
		}

		public string PopItem(string set)
		{
			return !_memory.ContainsKey(set) || _memory[set].Count == 0 ? null : _memory[set].DeleteMin().Value;
		}

		public void FlushAll(string set)
		{
			if (_memory.ContainsKey(set))
			{
				_memory[set] = new C5.IntervalHeap<Element>(_comparer);
			}
		}
	}
}
