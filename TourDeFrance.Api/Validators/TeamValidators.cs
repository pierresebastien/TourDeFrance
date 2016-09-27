using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
	{
		public CreateTeamRequestValidator()
		{
			RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name can't be null or empty");
		}
	}

	public class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
	{
		public UpdateTeamRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			Include(new CreateTeamRequestValidator());
		}
	}
}
