namespace TourDeFrance.Core.Tools.DataBase
{
	public interface IDatabaseManager
	{
		void SetupDatabase();
	}

	public enum DatabaseType
	{
		PostgreSQL,
		SQLite
	}
}
