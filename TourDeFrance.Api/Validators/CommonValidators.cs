using System;
using FluentValidation;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Api.Validators
{
	public class ObjectByIdRequestValidator: AbstractValidator<IIdentifiable<string>>
	{
		public ObjectByIdRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Id cannot be null or empty");
		}
	}

	public class ObjectByGuidRequestValidator : AbstractValidator<IIdentifiable<Guid>>
	{
		public ObjectByGuidRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
