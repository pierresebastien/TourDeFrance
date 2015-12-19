using System;
using SimpleStack.Orm.Expressions;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Repositories.Interfaces;

namespace TourDeFrance.Core.Repositories
{
	// TODO: add method in base repo to search on dynamic view => use slq builder
	public class SearchHistoryRepository : BaseRepository, ISearchHistoryRepository
	{
		public virtual PagingResult<Log> GetHistoryForGame(Guid gameId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			Action<SqlExpressionVisitor<Log>> where =
				x =>
				{
					x.Where(y => y.GameId == gameId);
					if (startDate.HasValue && endDate.HasValue)
					{
						x.Where(y => y.Date >= startDate && y.Date <= endDate);
					}
				};
			Action<SqlExpressionVisitor<Log>> orderBy = x => x.OrderByDescending(y => y.Date);
			return SearchDbObjects(where, offset, max, orderBy);
		}

		public virtual PagingResult<Log> GetHistoryForPlayer(Guid playerId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			Action<SqlExpressionVisitor<Log>> where =
				x =>
				{
					x.Where(y => y.PlayerId == playerId);
					if (startDate.HasValue && endDate.HasValue)
					{
						x.Where(y => y.Date >= startDate && y.Date <= endDate);
					}
				};
			Action<SqlExpressionVisitor<Log>> orderBy = x => x.OrderByDescending(y => y.Date);
			return SearchDbObjects(where, offset, max, orderBy);
		}

		public virtual PagingResult<Log> GetHistoryForUser(Guid userId, DateTime? startDate, DateTime? endDate, int offset, int max)
		{
			Action<SqlExpressionVisitor<Log>> where =
				x =>
				{
					x.Where(y => y.UserId == userId);
					if (startDate.HasValue && endDate.HasValue)
					{
						x.Where(y => y.Date >= startDate && y.Date <= endDate);
					}
				};
			Action<SqlExpressionVisitor<Log>> orderBy = x => x.OrderByDescending(y => y.Date);
			return SearchDbObjects(where, offset, max, orderBy);
		}
	}
}
