using TourDeFrance.Client.Enums;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface ILuceneRepository : ISearchHistoryRepository
	{
		void CreateJobReIndexation(IndexName indexName);

		void ProcessWaitingJobs();

		void Initialize();
	}
}
