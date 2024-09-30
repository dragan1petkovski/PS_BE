using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
	public class ResponseDownloadCertificate
	{
		public FileContentResult FileContent { get; set; }
		public string filename { get; set; }
	}
}
