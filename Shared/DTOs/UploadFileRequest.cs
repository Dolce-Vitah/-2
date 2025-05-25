using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class UploadFileRequest
    {
        [Required]
        public required IFormFile File { get; set; }
    }
}
