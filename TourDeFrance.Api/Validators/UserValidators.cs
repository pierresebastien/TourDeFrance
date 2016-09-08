using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	// TODO: improve rules on email, username, password
	public abstract class BaseUserRequestValidator<T> : AbstractValidator<T> where T : BaseUserRequest
	{
		protected BaseUserRequestValidator()
		{
			RuleFor(x => x.Password).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Password can't be null or empty");
			RuleFor(x => x.Email).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Email can't be null or empty");
			RuleFor(x => x.FirstName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("First name can't be null or empty");
			RuleFor(x => x.LastName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Last name can't be null or empty");
			RuleFor(x => x.BirthDate).Must(x => x < DateTime.Today).When(x => x.BirthDate.HasValue).WithMessage("Birth date must be in the past");
			RuleFor(x => x.Height).Must(x => x > 0).When(x => x.Height.HasValue).WithMessage("Height must be strictly positive");
			RuleFor(x => x.Weight).Must(x => x > 0).When(x => x.Weight.HasValue).WithMessage("Weight must be strictly positive");
		}
	}

	public class CreateUserRequestValidator : BaseUserRequestValidator<CreateUserRequest>
	{
		public CreateUserRequestValidator()
		{
			RuleFor(x => x.UserName).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Username cannot be null or empty");
		}
	}

	public class UpdateUserRequestValidator : BaseUserRequestValidator<UpdateUserRequest>
	{
		public UpdateUserRequestValidator()
		{
			RuleFor(r => r.Id).Must(x => x == Guid.Empty).WithMessage("Id cannot be empty");
		}
	}
}
