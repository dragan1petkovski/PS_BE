using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerDB
{
	public class SQLExceptions: ApplicationException
	{
		public SQLExceptions(string message): base(message) { }
	}


}
