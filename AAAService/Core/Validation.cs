using AAAService.Interface;
using System.Reflection.Metadata.Ecma335;

namespace AAAService.Core
{
	public class Validation : IDisposable
	{
		private List<iValidator> _validators = new List<iValidator>();

		public void AddValidator(iValidator validator)
		{
			_validators.Add(validator);
		}

		public async Task<bool> ProcessAsync()
		{
			foreach(iValidator validator in _validators)
			{
				if (!(await validator.ProcessAsync()))
				{
					return false;
				}
			}
			return true;
		}

		public void Dispose() { }
	}
}
