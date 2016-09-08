using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace TourDeFrance.Api.Validators
{
	// TODO: check if usable
	public class CompositeValidatorRule : IValidationRule<object>
	{
		private readonly IValidator[] _validators;

		public CompositeValidatorRule(params IValidator[] validators)
		{
			_validators = validators;
		}

		#region IValidationRule Members

		public string RuleSet
		{
			get; set;
		}

		public IEnumerable<ValidationFailure> Validate(ValidationContext<object> context)
		{
			var ret = new List<ValidationFailure>();

			foreach (var v in _validators)
			{
				ret.AddRange(v.Validate(context).Errors);
			}

			return ret;
		}

		public IEnumerable<IPropertyValidator> Validators
		{
			get { yield break; }
		}

		#endregion
	}
}
