using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateRiderRequestValidator<T> : AbstractValidator<T> where T : CreateRiderRequest
	{
		public CreateRiderRequestValidator()
		{
			// TODO: concat rule with player rules and user rules ???
			RuleFor(x => x.FirstName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("First name can't be null or empty");
			RuleFor(x => x.LastName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Last name can't be null or empty");
			RuleFor(x => x.BirthDate).Must(x => x < DateTime.Today).When(x => x.BirthDate.HasValue).WithMessage("Birth date must be in the past");
			RuleFor(x => x.Height).Must(x => x > 0).When(x => x.Height.HasValue).WithMessage("Height must be strictly positive");
			RuleFor(x => x.Weight).Must(x => x > 0).When(x => x.Weight.HasValue).WithMessage("Weight must be strictly positive");
			RuleFor(r => r.TeamId).Must(x => x == Guid.Empty).WithMessage("Team id cannot be empty");
		}
	}

	public class UpdateRiderRequestValidator : CreateRiderRequestValidator<UpdateRiderRequest>
	{
		public UpdateRiderRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
