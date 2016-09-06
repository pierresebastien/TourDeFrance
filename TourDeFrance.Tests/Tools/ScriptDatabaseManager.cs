using Npgsql;
using SimpleStack.Orm;
using TourDeFrance.Core;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Tests.Tools
{
	public class TestScriptDatabaseManager : ScriptDatabaseManager
	{
		public TestScriptDatabaseManager(IDialectProvider dialectProvider, ApplicationConfig config)
			: base(dialectProvider, config)
		{
		}

		protected override void Finish(TransactionScope scope)
		{
			base.Finish(scope);
			// NOTE: refresh cache with new types added in scripts
			(scope.Connection.DbConnection as NpgsqlConnection)?.ReloadTypes();
		}
	}
}
