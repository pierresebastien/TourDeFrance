using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateTeamRequestValidator<T> : AbstractValidator<T> where T : CreateTeamRequest
	{
		public CreateTeamRequestValidator()
		{
			RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name can't be null or empty");
		}
	}

	public class UpdateTeamRequestValidator : CreateTeamRequestValidator<UpdateTeamRequest>
	{
		public UpdateTeamRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
