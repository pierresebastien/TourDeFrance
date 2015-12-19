using TourDeFrance.Core.Repositories.Interfaces;

namespace TourDeFrance.Core.Repositories
{
	public class GameRepository : BaseRepository, IGameRepository
	{
		//// TODO: cache on cureent game + game team + game participant
		//// TODO: create views to limitate the number of querys in this repository (important to be fast)

		//// TODO: invalider cache du user qui cree, lsq on ajoute our retire un user au jeu, lsq la partie est finie
		//public DbGame CreateGame(Guid raceId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var race = RaceRepository.GetRaceById(raceId);
		//		var game = new DbGame {RaceId = race.Id, Status = GameStatus.Configuration};
		//		game.BeforeInsert();
		//		scope.Connection.Insert(game);
		//		AddGameLog(game.Id, "Game created");
		//		scope.Complete();
		//		return game;
		//	}
		//}

		//public DbGame GetGameById(Guid id, bool throwIfNotExist = true)
		//{
		//	return GetDbObjectById<DbGame>(id, "Game", throwIfNotExist);
		//}

		//// TODO: check user has rights on this game
		//public DbGame StartGame(Guid gameId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var game = GetGameById(gameId);
		//		EnsureGameIsInState(game, GameStatus.Configuration);
		//		IList<DbGameTeam> gameTeams = scope.Connection.Select<DbGameTeam>(x => x.GameId == game.Id);
		//		if (gameTeams.Count < 2)
		//		{
		//			throw new TourDeFranceException("A game must have at least 2 teams");
		//		}
		//		foreach (var dbGameTeam in gameTeams)
		//		{
		//			var gameTeamId = dbGameTeam.Id;
		//			if (scope.Connection.Select<DbGameParticipant>(x => x.GameTeamId == gameTeamId).Count < 1)
		//			{
		//				var team = TeamRepository.GetTeamById(dbGameTeam.TeamId);
		//				throw new TourDeFranceException($"The team '{team.Name}' must have at least one player");
		//			}
		//		}
		//		// TODO: other checks?? like Race has at least one stage and each stage has at least one drink?? -> or do that in other repo => check when remove that number is always >= 1
		//		game.Status = GameStatus.StageStart;
		//		game.BeforeUpdate();
		//		scope.Connection.Update(game);

		//		StartNewStage(gameId, 1);
		//		scope.Complete();
		//		return game;
		//	}
		//}

		//// TODO: invalidate cache users of this game??
		//public void StartNewStage(Guid gameId, int stageNumber)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var game = GetGameById(gameId);
		//		EnsureGameIsInState(game, GameStatus.StageStart);
		//		var stage = scope.Connection.FirstOrDefault<DbRaceStage>(x => x.RaceId == game.RaceId && x.Order == stageNumber);
		//		if (stage == null)
		//		{
		//			throw new Exception(); // TODO:
		//		}
		//		IList<DbStageDrink> drinks = scope.Connection.Select<DbStageDrink>(x => x.StageId == stage.StageId);

		//		foreach (var gameTeam in scope.Connection.Select<DbGameTeam>(x => x.GameId == gameId))
		//		{
		//			var gameTeamId = gameTeam.Id;
		//			foreach (var participant in scope.Connection.Select<DbGameParticipant>(x => x.GameTeamId == gameTeamId))
		//			{
		//				foreach (var drink in drinks)
		//				{
		//					var score = new DbScore
		//					            {GameParticipantId = participant.Id, DrinkId = drink.Id, StageId = stage.StageId, Number = 0};
		//					score.BeforeInsert();
		//					scope.Connection.Insert(score);
		//				}
		//			}
		//		}

		//		game.Status = GameStatus.InProgress;
		//		game.BeforeUpdate();
		//		scope.Connection.Update(game);
		//		scope.Complete();
		//	}
		//}

		//// TODO: ensure user has right on game
		//// TODO: cache on stage drinks for participant???
		//// TODO: get game by gameParticipant
		//public DbGame AddDrinkToParticipant(Guid gameParticipantId, Guid drinkId, Guid stageId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var participant = GetGameParticipantById(gameParticipantId);
		//		var gameTeam = GetGameTeamById(participant.GameTeamId);
		//		var game = GetGameById(gameTeam.GameId);
		//		EnsureGameIsInState(game, GameStatus.InProgress);

		//		var score = GetScore(gameParticipantId, drinkId, stageId);
		//		score.Number++;
		//		score.BeforeUpdate();
		//		scope.Connection.Update(score);

		//		scope.Complete();
		//		return game;
		//	}
		//}

		//public DbScore GetScore(Guid gameParticipantId, Guid drinkId, Guid stageId, bool throwIfNotExist = true)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var score = scope.Connection.FirstOrDefault<DbScore>(
		//			x => x.GameParticipantId == gameParticipantId && x.DrinkId == drinkId && x.StageId == stageId);
		//		if (score == null && throwIfNotExist)
		//		{
		//			throw new NotFoundException("Score not found");
		//		}
		//		scope.Complete();
		//		return score;
		//	}
		//}

		//#region Team

		//// TODO: check user has rights on this game
		//public DbGameTeam AddTeamToGame(Guid gameId, Guid teamId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var game = GetGameById(gameId);
		//		EnsureGameIsInState(game, GameStatus.Configuration);
		//		var team = TeamRepository.GetTeamById(teamId);
		//		if (scope.Connection.FirstOrDefault<DbGameTeam>(x => x.GameId == game.Id && x.TeamId == team.Id) != null)
		//		{
		//			throw new TourDeFranceException($"Team '{ team.Name}' already assigned to this game");
		//		}
		//		var gameTeam = new DbGameTeam {GameId = game.Id, TeamId = team.Id};
		//		gameTeam.BeforeInsert();
		//		scope.Connection.Insert(gameTeam);
		//		scope.Complete();
		//		return gameTeam;
		//	}
		//}

		//// TODO: check user has rights on this game
		//public DbGameTeam RemoveTeamOfGame(Guid gameTeamId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var gameTeam = GetGameTeamById(gameTeamId);
		//		var game = GetGameById(gameTeam.GameId);
		//		EnsureGameIsInState(game, GameStatus.Configuration);
		//		scope.Connection.DeleteById<DbGameTeam>(gameTeamId);
		//		scope.Complete();
		//		return gameTeam;
		//	}
		//}

		//public DbGameTeam GetGameTeamById(Guid id, bool throwIfNotExist = true)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		DbGameTeam gameTeam = scope.Connection.GetDbObjectById<DbGameTeam>(id, "Game team", throwIfNotExist);
		//		scope.Complete();
		//		return gameTeam;
		//	}
		//}

		//#endregion

		//#region Participant

		//// TODO: check user has rights on this game + invalidate cache of user id
		//public DbGameParticipant AddParticipantToGame(Guid gameTeamId, Guid playerId, Guid? userId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var gameTeam = GetGameTeamById(gameTeamId);
		//		var game = GetGameById(gameTeam.GameId);
		//		EnsureGameIsInState(game, GameStatus.Configuration);
		//		var player = PlayerRepository.GetPlayerById(playerId);
		//		if (userId.HasValue)
		//		{
		//			UserRepository.GetDbUserById(userId.Value);
		//		}
		//		// TODO: check player is not assigned to game (user too)
		//		var participant = new DbGameParticipant {GameTeamId = gameTeam.Id, PlayerId = player.Id, UserId = userId};
		//		participant.BeforeInsert();
		//		scope.Connection.Insert(participant);
		//		scope.Complete();
		//		return participant;
		//	}
		//}

		//// TODO: check user has rights on this game + invalidate cache of user id
		//public DbGameParticipant RemoveParticpantOfGame(Guid gameParticipantId)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		var participant = GetGameParticipantById(gameParticipantId);
		//		var gameTeam = GetGameTeamById(participant.GameTeamId);
		//		var game = GetGameById(gameTeam.GameId);
		//		EnsureGameIsInState(game, GameStatus.Configuration);
		//		scope.Connection.DeleteById<DbGameParticipant>(gameParticipantId);
		//		scope.Complete();
		//		return participant;
		//	}
		//}

		//public DbGameParticipant GetGameParticipantById(Guid id, bool throwIfNotExist = true)
		//{
		//	using (var scope = new TransactionScope())
		//	{
		//		DbGameParticipant gameParticipant = scope.Connection.GetDbObjectById<DbGameParticipant>(id, "Game particpant",
		//			throwIfNotExist);
		//		scope.Complete();
		//		return gameParticipant;
		//	}
		//}

		//#endregion
	}
}