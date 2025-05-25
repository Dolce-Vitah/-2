using FileStorage.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace FileStorage.Web.Controllers
{
    [ApiController]
    [Route("api/storage/files")]
    public class StorageController: ControllerBase
    {
        private readonly IFileService _fileService;

        public StorageController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            var file = request.File;
            try
            {
                var fileId = await _fileService.UploadFileAsync(file.OpenReadStream(), file.FileName);
                return Ok(new UploadFileResponse { ID = fileId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            try
            {
                var stream = await _fileService.GetFileAsync(fileId);
                return Ok(stream);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
    
}
