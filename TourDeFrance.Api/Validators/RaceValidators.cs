using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateRaceRequestValidator<T> : AbstractValidator<T> where T : CreateRaceRequest
	{
		public CreateRaceRequestValidator()
		{
			RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name can't be null or empty");
		}
	}

	public class UpdateRaceRequestValidator: CreateRaceRequestValidator<UpdateRaceRequest>
	{
		public UpdateRaceRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
