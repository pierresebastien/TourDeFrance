using System;
using TourDeFrance.Core.Business;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface ISearchHistoryRepository
	{
		PagingResult<Log> GetHistoryForUser(Guid userId, DateTime? startDate, DateTime? endDate, int offset, int max);

		PagingResult<Log> GetHistoryForGame(Guid gameId, DateTime? startDate, DateTime? endDate, int offset, int max);

		PagingResult<Log> GetHistoryForPlayer(Guid playerId, DateTime? startDate, DateTime? endDate, int offset, int max);
	}
}
