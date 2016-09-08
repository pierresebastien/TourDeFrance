using System;
using FluentValidation;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Api.Validators
{
	public class ObjectByIdRequestValidator<T> : AbstractValidator<T> where T : IIdentifiable<string>
	{
		public ObjectByIdRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Id cannot be null or empty");
		}
	}

	public class ObjectByGuidRequestValidator<T> : AbstractValidator<T> where T : IIdentifiable<Guid>
	{
		public ObjectByGuidRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
