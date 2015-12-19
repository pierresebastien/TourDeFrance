﻿using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("races")]
	public class DbRace : BaseOwnObjectNameable
	{
		public Client.Race.Race ToModel()
		{
			return new Client.Race.Race
			{
				Id = Id,
				Name = Name,
				OwnerId = Owner
			};
		}
	}

	[Alias("race_stages")]
	public class DbRaceStage : Auditable, IOrderable
	{
		[ForeignKey(typeof(DbStage))]
		[Alias("stage_id")]
		public Guid StageId { get; set; }

		[ForeignKey(typeof(DbRace))]
		[Alias("race_id")]
		public Guid RaceId { get; set; }

		[Alias("order")]
		public int Order { get; set; }
	}
}