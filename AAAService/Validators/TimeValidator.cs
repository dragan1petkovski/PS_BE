using AAAService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAAService.Validators
{
	internal class TimeValidator: iValidator
	{
		private DateTime _startTime;
		private DateTime _endTime;

		public TimeValidator(DateTime startTime, DateTime endTime)
		{
			_startTime = startTime;
			_endTime = endTime;
		}

		public async Task<bool> ProcessAsync()
		{
			if(DateTime.Now > _startTime && DateTime.Now < _endTime) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
