using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.Rider;

namespace TourDeFrance.Api.Services
{
	public class RiderService : BaseService
	{
		public RiderService() : base("/riders")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllRiders());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetRider(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateRider(this.BindAndValidate<>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateRider(this.BindAndValidate<>()));
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

		public Rider CreateRider(CreateUpdateRider model)
		{
			return
				RiderRepository.CreateRider(model.FirstName, model.LastName, model.Gender, model.BirthDate, model.Nationality,
					model.Height, model.Weight, model.Picture, model.TeamId).ToModel();
		}

		public Rider UpdateRider(Guid riderId, CreateUpdateRider model)
		{
			return
				RiderRepository.UpdateRider(riderId, model.FirstName, model.LastName, model.Gender, model.BirthDate,
					model.Nationality, model.Height, model.Weight, model.Picture, model.TeamId).ToModel();
		}

		public Rider DeleteRider(ObjectByGuidRequest request)
		{
			return RiderRepository.DeleteRider(request.Id).ToModel();
		}
	}
}
