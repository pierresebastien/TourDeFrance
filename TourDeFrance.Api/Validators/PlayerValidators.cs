using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
	{
		public CreatePlayerRequestValidator()
		{
			RuleFor(x => x.Nickname).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Nickname can't be null or empty");
			RuleFor(x => x.FirstName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("First name can't be null or empty");
			RuleFor(x => x.LastName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Last name can't be null or empty");
			RuleFor(x => x.BirthDate).Must(x => x < DateTime.Today).When(x => x.BirthDate.HasValue).WithMessage("Birth date must be in the past");
			RuleFor(x => x.Height).Must(x => x > 0).When(x => x.Height.HasValue).WithMessage("Height must be strictly positive");
			RuleFor(x => x.Weight).Must(x => x > 0).When(x => x.Weight.HasValue).WithMessage("Weight must be strictly positive");
		}
	}

	public class UpdatePlayerRequestValidator : AbstractValidator<UpdatePlayerRequest>
	{
		public UpdatePlayerRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			Include(new CreatePlayerRequestValidator());
		}
	}
}
