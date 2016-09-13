using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace TourDeFrance.Api.Validators
{
	// TODO: check if usable
	public class CompositeValidatorRule : IValidationRule
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

		public IEnumerable<ValidationFailure> Validate(ValidationContext context)
		{
			var ret = new List<ValidationFailure>();

			foreach (var v in _validators)
			{
				ret.AddRange(v.Validate(context).Errors);
			}

			return ret;
		}

		public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation)
		{
			throw new NotImplementedException();
		}

		public void ApplyAsyncCondition(Func<object, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPropertyValidator> Validators
		{
			get { yield break; }
		}

		#endregion
	}
}
