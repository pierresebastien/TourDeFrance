using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Api.Services
{
	public class RiderService : BaseService
	{
		public RiderService() : base("/riders")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllRiders());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetRider(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateRider(this.BindAndValidate<CreateRiderRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateRider(this.BindAndValidate<UpdateRiderRequest>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeleteRider(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Rider[] GetAllRiders()
		{
			return RiderRepository.GetAllRiders().Select(x => x.ToModel()).ToArray();
		}

		public Rider GetRider(ObjectByGuidRequest request)
		{
			return RiderRepository.GetRiderById(request.Id).ToModel();
		}

		public Rider CreateRider(CreateRiderRequest request)
		{
			return
				RiderRepository.CreateRider(request.FirstName, request.LastName, request.Gender, request.BirthDate,
					request.Nationality, request.Height, request.Weight, request.Picture, request.TeamId).ToModel();
		}

		public Rider UpdateRider(UpdateRiderRequest request)
		{
			return
				RiderRepository.UpdateRider(request.Id, request.FirstName, request.LastName, request.Gender, request.BirthDate,
					request.Nationality, request.Height, request.Weight, request.Picture, request.TeamId).ToModel();
		}

		public Rider DeleteRider(ObjectByGuidRequest request)
		{
			return RiderRepository.DeleteRider(request.Id).ToModel();
		}
	}
}
