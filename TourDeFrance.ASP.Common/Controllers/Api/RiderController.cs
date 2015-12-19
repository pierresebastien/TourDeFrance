using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Rider;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class RiderController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/riders")]
		public IEnumerable<Rider> GetAllRiders()
		{
			return RiderRepository.GetAllRiders().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/riders/{riderId}")]
		public Rider GetRider(Guid riderId)
		{
			return RiderRepository.GetRiderById(riderId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/riders")]
		public Rider CreateRider(CreateUpdateRider model)
		{
			return
				RiderRepository.CreateRider(model.FirstName, model.LastName, model.Gender, model.BirthDate, model.Nationality,
					model.Height, model.Weight, model.Picture, model.TeamId).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/riders/{riderId}")]
		public Rider UpdateRider(Guid riderId, CreateUpdateRider model)
		{
			return
				RiderRepository.UpdateRider(riderId, model.FirstName, model.LastName, model.Gender, model.BirthDate,
					model.Nationality, model.Height, model.Weight, model.Picture, model.TeamId).ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/riders/{riderId}")]
		public IHttpActionResult DeleteRider(Guid riderId)
		{
			RiderRepository.DeleteRider(riderId);
			return Ok();
		}

		#endregion
	}
}
