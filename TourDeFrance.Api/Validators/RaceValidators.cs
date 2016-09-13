using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateRaceRequestValidator: AbstractValidator<CreateRaceRequest>
	{
		public CreateRaceRequestValidator()
		{
			RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name can't be null or empty");
		}
	}

	public class UpdateRaceRequestValidator: AbstractValidator<UpdateRaceRequest>
	{
		public UpdateRaceRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			Include(new CreateRaceRequestValidator());
		}
	}
}
