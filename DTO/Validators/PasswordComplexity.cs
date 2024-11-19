
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.Validators
{
	public class PasswordComplexity: ValidationAttribute
	{
		public PasswordComplexity() : base("Password is not complex enough")
		{ }
		
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
		
			string temp = value as string;
			if(temp.Count() < 8)
			{
				new ValidationResult("Password do not contain 8 characters");
			}
			if(temp.Distinct().Count() < 8)
			{
				new ValidationResult("Password do not contain 8 unique characters");
			}
			if(!temp.Any(c => char.IsNumber(c)))
			{
				new ValidationResult("Password do not contain any numbers");
			}
			if(!temp.Any(c => char.IsLower(c)))
			{
				new ValidationResult("Password do not contain any lower case characters");
			}
			if(!temp.Any(c => char.IsUpper(c)))
			{
				new ValidationResult("Password do not contain any upper case characters");
			}
			if(!temp.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
			{
				new ValidationResult("Password do not contain any special symbol characters");
			}

			return ValidationResult.Success;

			
		}
	}
}
