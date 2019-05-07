using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebFrontend.Models
{
	public class FileUpload
	{
		[Required]
		[Display(Name="File")]
		public IFormFile UploadFile { get; set; }

	}
}
