using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class ObjectByIdRequestValidator<T> : AbstractValidator<T> where T : ObjectByIdRequest
	{
		public ObjectByIdRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Id cannot be null or empty");
		}
	}

	public class ObjectByGuidRequestValidator<T> : AbstractValidator<T> where T : ObjectByGuidRequest
	{
		public ObjectByGuidRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
