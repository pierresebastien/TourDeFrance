using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public abstract class BaseStageDrinkRequestValidator<T> : AbstractValidator<T> where T : BaseStageDrinkRequest
	{
		protected BaseStageDrinkRequestValidator()
		{
			RuleFor(x => x.NumberToDrink).GreaterThan(0).WithMessage("Number of drinks must be strictly positive");
			RuleFor(x => x.OverridedVolume).Must(x => x > 0).When(x => x.OverridedVolume.HasValue).WithMessage("Volume must be strictly positive");
		}
	}

	public class CreateStageDrinkRequestValidator : BaseStageDrinkRequestValidator<CreateStageDrinkRequest>
	{
		public CreateStageDrinkRequestValidator()
		{
			RuleFor(r => r.DrinkId).Must(x => x == Guid.Empty).WithMessage("Drink id cannot be empty");
			RuleFor(r => r.StageId).Must(x => x == Guid.Empty).WithMessage("Stage id cannot be empty");
		}
	}

	public class UpdateStageDrinkRequestValidator : BaseStageDrinkRequestValidator<UpdateStageDrinkRequest>
	{
		public UpdateStageDrinkRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
			RuleFor(x => x.Order).GreaterThan(0).WithMessage("Order must be strictly positive");
		}
	}
}
