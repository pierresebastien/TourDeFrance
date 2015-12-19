using NUnit.Framework;
using SimpleStack.Orm;
using SimpleStack.Orm.Attributes;
using SimpleStack.Orm.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace TourDeFrance.Tests
{
	[TestFixture]
	public class OrmTests
	{
		public class Bar
		{
			[PrimaryKey]
			public int Id { get; set; }

			[Alias("Test")]
			public string BarProperty { get; set; }
		}
		
		public class Foo
		{
			[PrimaryKey]
			public int Id { get; set; }

			public string FooProperty { get; set; }
		}

		public class BarView
		{
			public string BarProperty { get; set; }

			public string FooProperty { get; set; }
		}

		[Test]
		public void Test()
		{
			IDialectProvider dialectProvider = new SqliteDialectProvider();
			using (OrmConnection connection = dialectProvider.CreateConnection(":memory:"))
			{
				connection.Open();

				connection.CreateTable<Bar>(true);
				connection.CreateTable<Foo>(true);

				var builder = new JoinSqlBuilder<Bar, Bar>(dialectProvider)
					.Join<Bar, Foo>(x => x.Id, x => x.Id)
					.SelectAll<Bar>()
					.Select<Foo>(x => new {x.FooProperty});
				IList<BarView> views = connection.Query<BarView>(builder.ToSql(), builder.Parameters).ToList();
				Assert.AreEqual(10, views.Count);
				foreach (var view in views)
				{
					Assert.AreEqual(view.BarProperty.Replace("_Bar", string.Empty), view.FooProperty.Replace("_Foo", string.Empty));
				}

				connection.Close();
			}
		}
	}
}
