using System;
using System.Data;
using ServiceStack.Logging;
using ServiceStack.Redis;
using SimpleStack.Orm;

namespace TourDeFrance.Core.Tools.DataBase
{
	public class TransactionScope : IDisposable
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(TransactionScope));

		[ThreadStatic]
		private static TransactionScope _rootScope;

		private OrmConnection _rootScopeConnection;
		private IDbTransaction _transaction;
		private IRedisClient _redisClient;
		private IRedisTransaction _redisTransaction;

		protected TransactionScope RootScope { get { return _rootScope; } set { _rootScope = value; } }

		public TransactionScope(IDialectProvider dialectProvider, ApplicationConfig config, IRedisClientsManager redisClientsManager = null)
		{
			if (RootScope == null)
			{
				// Create DB Connection scope
				_rootScopeConnection = dialectProvider.CreateConnection(config.ConnectionString);
				_rootScopeConnection.Open();

				// Create a transcaction
				_transaction = _rootScopeConnection.BeginTransaction();

				// Setup self as the root scope
				RootScope = this;
				
				if(redisClientsManager != null)
				{
					_redisClient = redisClientsManager.GetClient();
					_redisTransaction = _redisClient.CreateTransaction();
				}
			}

			Completed = false;
		}

		public TransactionScope()
			: this(
				Context.Current.DialectProvider, Context.Current.ApplicationConfig,
				Context.Current.TryResolve<IRedisClientsManager>())
		{
		}

		public OrmConnection Connection
		{
			get
			{
				if (Completed)
				{
					throw new InvalidOperationException("Scope is already completed");
				}
				return RootScope._rootScopeConnection;
			}
		}

		public IRedisTransaction RedisTransaction
		{
			get
			{
				if (Completed)
				{
					throw new InvalidOperationException("Scope is already completed");
				}
				return RootScope._redisTransaction;
			}
		}

		public bool Completed { get; private set; }

		public void Complete()
		{
			// Set completed flag
			Completed = true;

			// Don't complete if we're not the root scope.
			if (RootScope != this)
			{
				return;
			}

			if (_transaction != null)
			{
				Logger.DebugFormat("Committing Transaction");
				_transaction.Commit();
			}

			_redisTransaction?.Commit();

			Cleanup();
		}

		public void Cancel()
		{
			// If we are not the root scope, cancel the entire transaction
			if (RootScope != null && RootScope != this)
			{
				RootScope.Cancel();
				return;
			}

			if (_transaction != null)
			{
				Logger.WarnFormat("Rollback Transaction - StackTrace:{0}", Environment.StackTrace);
				_transaction.Rollback();
			}

			_redisTransaction?.Rollback();

			Cleanup();
		}

		private void Cleanup()
		{
			if (_transaction != null)
			{
				_transaction.Dispose();
				_transaction = null;
			}

			if (_rootScopeConnection != null)
			{
				_rootScopeConnection.Dispose();
				_rootScopeConnection = null;
			}

			if (_redisTransaction != null)
			{
				_redisTransaction.Dispose();
				_redisTransaction = null;
			}

			if (_redisClient != null)
			{
				_redisClient.Dispose();
				_redisClient = null;
			}

			RootScope = null;
		}

		public void Dispose()
		{
			if (!Completed)
			{
				Cancel();
			}
		}
	}
}