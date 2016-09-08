using System.Linq;
using TourDeFrance.Client.Requests;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Api.Services
{
	public class PlayerService : BaseService
	{
		public PlayerService() : base("/players")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllPlayers());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetPlayer(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreatePlayer(this.BindAndValidate<CreatePlayerRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdatePlayer(this.BindAndValidate<UpdatePlayerRequest>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeletePlayer(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Player[] GetAllPlayers()
		{
			return PlayerRepository.GetAllPlayers().Select(x => x.ToModel()).ToArray();
		}

		public Player GetPlayer(ObjectByGuidRequest request)
		{
			return PlayerRepository.GetPlayerById(request.Id).ToModel();
		}

		public Player CreatePlayer(CreatePlayerRequest request)
		{
			return
				PlayerRepository.CreatePlayer(request.Nickname, request.FirstName, request.LastName, request.Gender,
					request.BirthDate, request.Height, request.Weight).ToModel();
		}

		public Player UpdatePlayer(UpdatePlayerRequest request)
		{
			return
				PlayerRepository.UpdatePlayer(request.Id, request.Nickname, request.FirstName, request.LastName, request.Gender,
					request.BirthDate, request.Height, request.Weight).ToModel();
		}

		public Player DeletePlayer(ObjectByGuidRequest request)
		{
			return PlayerRepository.DeletePlayer(request.Id).ToModel();
		}
	}
}
