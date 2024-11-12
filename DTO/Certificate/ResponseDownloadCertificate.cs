using Microsoft.AspNetCore.Mvc;

namespace DTO.Certificate
{
	public class ResponseDownloadCertificate
	{
		public FileContentResult FileContent { get; set; }
		public string filename { get; set; }
	}
}
