using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Dapper;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SimpleStack.Orm.Expressions;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using Directory = System.IO.Directory;
using Document = Lucene.Net.Documents.Document;
using Version = Lucene.Net.Util.Version;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class LuceneRepository : BaseRepository, ILuceneRepository
	{
		protected static readonly Analyzer Analyzer = new StandardAnalyzer(Version.LUCENE_30);
		protected DirectoryInfo IndexHistoryInfo;
		protected readonly string ReIndexHistoryKey;

		public LuceneRepository()
		{
			ReIndexHistoryKey = $"lucene:{IndexName.History}:{IndexAction.ReIndexAll}".FormatQueueKey();
		}

		public virtual void Initialize()
		{
			string indexFolder = Config.IndexFolder;
			if (string.IsNullOrWhiteSpace(indexFolder))
			{
				throw new TourDeFranceException("No index folder specified");
			}
			string historyIndexPath = Path.Combine(indexFolder, "History");
			if (!Directory.Exists(historyIndexPath))
			{
				Directory.CreateDirectory(historyIndexPath);
			}
			IndexHistoryInfo = new DirectoryInfo(historyIndexPath);
			if (!IndexHistoryInfo.Exists)
			{
				using (FSDirectory fsDirectory = FSDirectory.Open(IndexHistoryInfo))
				using (var writer = new IndexWriter(fsDirectory, Analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
				{
					writer.Commit();
				}
			}
		}

		public virtual void ProcessWaitingJobs()
		{
			ProcessWaitingHistoryJobs();
		}

		public void CreateJobReIndexation(IndexName indexName)
		{
			throw new NotImplementedException();
			//using (IRedisClient client = RedisClientsManager.GetClient())
			//{
			//	string key = $"lucene:{indexName}:{IndexAction.ReIndexAll}".FormatQueueKey();
			//	client.SetEntryIfNotExists(key, "true");
			//}
		}

		public PagingResult<Log> GetHistoryForUser(Guid userId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			if (CurrentUser.Id != userId)
			{
				EnsureUserIsAdmin();
			}

			var result = new PagingResult<Log>();

			using (FSDirectory fsDirectory = FSDirectory.Open(IndexHistoryInfo))
			using (IndexReader indexReader = IndexReader.Open(fsDirectory, true))
			using (Searcher indexSearch = new IndexSearcher(indexReader))
			{
				var queryParser = new QueryParser(Version.LUCENE_30, "Message", Analyzer);
				var query = new BooleanQuery();

				if (startDate.HasValue && endDate.HasValue)
				{
					var rangeQuery = new TermRangeQuery("Date",
						DateTools.DateToString(startDate.Value, DateTools.Resolution.DAY),
						DateTools.DateToString(endDate.Value, DateTools.Resolution.SECOND),
						true, true);
					query.Add(rangeQuery, Occur.MUST);
				}

				var builder = new StringBuilder();
				builder.AppendFormat("UserId:\"{0}\"", userId.ToString());
				Query otherFilters = builder.Length == 0 ? new MatchAllDocsQuery() : queryParser.Parse(builder.ToString());
				query.Add(otherFilters, Occur.MUST);

				var sort = new Sort(new SortField("Date", SortField.LONG, true));

				TopDocs resultDocs = indexSearch.Search(query, null, offset + max, sort);
				result.Max = max;
				result.Total = resultDocs.TotalHits;
				result.Offset = offset;

				var logEntryViews = new List<Log>();

				for (int i = 0; i < max; i++)
				{
					if (offset + i >= resultDocs.ScoreDocs.Length)
					{
						break;
					}
					logEntryViews.Add(FromDocument<Log>(indexSearch.Doc(resultDocs.ScoreDocs[offset + i].Doc)));
				}

				result.Results = logEntryViews;
			}

			return result;
		}

		public PagingResult<Log> GetHistoryForGame(Guid gameId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			throw new NotImplementedException();
		}

		public PagingResult<Log> GetHistoryForPlayer(Guid playerId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			throw new NotImplementedException();
		}

		protected virtual void AddOrUpdateHistoryLines(Guid[] ids)
		{
			using (FSDirectory fsDirectory = FSDirectory.Open(IndexHistoryInfo))
			using (var writer = new IndexWriter(fsDirectory, Analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			using (var scope = new TransactionScope())
			{
				var query = Log.Sqlbuilder.Where<DbLog>(x => Sql.In(x.Id, ids));
				foreach (var log in scope.Connection.Query<Log>(query.ToSql(), query.Parameters))
				{
					writer.UpdateDocument(new Term("Id", log.Id.ToString()), DocumentFromLog(log));
				}

				scope.Complete();
				writer.Commit();
			}
		}

		protected virtual void ReIndexHistory()
		{
			//1. Open Index Write
			using (FSDirectory fsDirectory = FSDirectory.Open(IndexHistoryInfo))
			using (var writer = new IndexWriter(fsDirectory, Analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			using (var scope =  new TransactionScope())
			{
				//2. Get history information and index
				writer.DeleteAll();

				int i = 0;
				foreach (Log log in scope.Connection.Query<Log>(Log.Sqlbuilder.ToSql()))
				{
					writer.AddDocument(DocumentFromLog(log));
					i++;
					if (i % 10000 == 0)
					{
						writer.Commit();
					}
				}

				scope.Complete();
				writer.Commit();
			}
		}

		protected virtual void ProcessWaitingHistoryJobs()
		{
			throw new NotImplementedException();
			//using (IRedisClient cache = Context.Current.RedisClientsManager.GetClient())
			//using (var scope =  new TransactionScope())
			//{
			//	// re index all
			//	if (Cache.Get<string>(ReIndexHistoryKey) != null)
			//	{
			//		string key = $"lucene:{IndexName.History}:*".FormatQueueKey();
			//		Cache.RemoveAll(Cache.SearchKeys(key));
			//		ReIndexHistory();
			//		Cache.Remove(ReIndexHistoryKey);
			//	}

			//	// index
			//	IList<Guid> logIds = new List<Guid>();
			//	string logEntryIdString = cache.PopItemWithLowestScoreFromSortedSet(IndexHistorySetName);
			//	while (logEntryIdString != null)
			//	{
			//		logIds.Add(Guid.Parse(logEntryIdString));
			//		logEntryIdString = cache.PopItemWithLowestScoreFromSortedSet(IndexHistorySetName);
			//	}

			//	int currentId = 0;
			//	const int maxIdByProcess = 1000;
			//	while (currentId < logIds.Count)
			//	{
			//		AddOrUpdateHistoryLines(logIds.Skip(currentId).Take(maxIdByProcess).ToArray());
			//		currentId += maxIdByProcess;
			//	}

			//	scope.Complete();
			//}
		}

		/// <summary>
		///    Generic method to convert a Lucene Document into a given T class object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="document"></param>
		/// <returns></returns>
		protected virtual T FromDocument<T>(Document document) where T : new()
		{
			var obj = new T();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				string s = document.Get(propertyInfo.Name);
				if (s != null)
				{
					if (propertyInfo.PropertyType == typeof(string))
					{
						propertyInfo.SetValue(obj, s, null);
					}
					else if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
					{
						propertyInfo.SetValue(obj, Guid.Parse(s), null);
					}
					else if (propertyInfo.PropertyType == typeof(DateTime))
					{
						propertyInfo.SetValue(obj, DateTools.StringToDate(s), null);
					}
					else if (propertyInfo.PropertyType.IsEnum)
					{
						propertyInfo.SetValue(obj, Enum.Parse(propertyInfo.PropertyType, s, true), null);
					}
					else if ((propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericArguments()[0].IsEnum))
					{
						propertyInfo.SetValue(obj, Enum.Parse(propertyInfo.PropertyType.GetGenericArguments()[0], s, true), null);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						propertyInfo.SetValue(obj, int.Parse(s), null);
					}
				}
			}
			return obj;
		}

		protected virtual Document DocumentFromLog(Log log)
		{
			var document = new Document();
			AddField(document, "Id", log.Id, true, true);
			AddField(document, "GameId", log.GameId, true, true);
			AddField(document, "GameParticipantId", log.GameParticipantId, true, true);
			AddField(document, "UserId", log.UserId, true, true);
			AddField(document, "Date", log.Date, true, true);
			AddField(document, "Type", log.Type.ToString(), true, true);
			AddField(document, "Paramaters", log.Paramaters, true, true);
			AddField(document, "CreationDate", log.CreationDate, true, true);
			AddField(document, "LastUpdateDate", log.LastUpdateDate, true, true);
			AddField(document, "LastUpdateBy", log.LastUpdateBy, true, true);
			AddField(document, "Username", log.Username, true, true);
			AddField(document, "UserFirstName", log.UserFirstName, true, true);
			AddField(document, "UserLastName", log.UserLastName, true, true);
			AddField(document, "PlayerNickname", log.PlayerNickname, true, true);
			AddField(document, "PlayerFirstName", log.PlayerFirstName, true, true);
			AddField(document, "PlayerLastName", log.PlayerLastName, true, true);
			AddField(document, "RaceName", log.RaceName, true, true);
			return document;
		}

		protected virtual void AddField(Document document, string fieldName, Guid? fieldValue, bool store, bool analyze)
		{
			if (fieldValue.HasValue)
			{
				AddField(document, fieldName, fieldValue.Value.ToString(), store, analyze);
			}
		}

		protected virtual void AddField(Document document, string fieldName, DateTime? fieldValue, bool store, bool analyze)
		{
			if (fieldValue.HasValue)
			{
				AddField(document, fieldName, DateTools.DateToString(fieldValue.Value, DateTools.Resolution.MILLISECOND), store, analyze);
			}
		}

		protected virtual void AddField(Document document, string fieldName, string fieldValue, bool store, bool analyze)
		{
			if (fieldValue != null)
			{
				document.Add(new Field(fieldName, fieldValue, store ? Field.Store.YES : Field.Store.NO,
					analyze ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));
			}
		}
	}
}
