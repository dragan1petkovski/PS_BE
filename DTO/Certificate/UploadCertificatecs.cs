using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DTO.Certificate
{
	public class UploadCertificatecs
	{
		[Required]
		public IFormFile certfile {  get; set; }
		public string? certpass { get; set; }

		public IFormFile? certkey { get; set; }

		[Required]
		public List<string> team {  get; set; } 
	}
}
