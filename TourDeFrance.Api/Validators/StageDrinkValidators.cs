using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class BaseStageDrinkRequestValidator : AbstractValidator<BaseStageDrinkRequest>
	{
		public BaseStageDrinkRequestValidator()
		{
			RuleFor(x => x.NumberToDrink).GreaterThan(0).WithMessage("Number of drinks must be strictly positive");
			RuleFor(x => x.OverridedVolume).Must(x => x > 0).When(x => x.OverridedVolume.HasValue).WithMessage("Volume must be strictly positive");
		}
	}

	public class CreateStageDrinkRequestValidator : AbstractValidator<CreateStageDrinkRequest>
	{
		public CreateStageDrinkRequestValidator()
		{
			Include(new BaseStageDrinkRequestValidator());
			RuleFor(r => r.DrinkId).Must(x => x == Guid.Empty).WithMessage("Drink id cannot be empty");
			RuleFor(r => r.StageId).Must(x => x == Guid.Empty).WithMessage("Stage id cannot be empty");
		}
	}

	public class UpdateStageDrinkRequestValidator : AbstractValidator<UpdateStageDrinkRequest>
	{
		public UpdateStageDrinkRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			Include(new BaseStageDrinkRequestValidator());
			RuleFor(x => x.Order).GreaterThan(0).WithMessage("Order must be strictly positive");
		}
	}
}
