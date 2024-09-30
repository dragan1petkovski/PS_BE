using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel
{
	public class CertificateFile
	{
		[Key] public Guid id {  get; set; }
		public string originalFileName { get; set; }
		public string currentFileName { get; set; }

		//Filehash need to be implemented to prevet from certificate swap on the server
		public DateTime createdate { get; set; }

		[InverseProperty("file")]
		public List<Certificate> certificates { get; set; }

		[InverseProperty("key")]
		public List<Certificate> keys { get; set; }


	}
}
