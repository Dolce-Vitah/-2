using Gateway.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Gateway.Web.Controllers
{
    [ApiController]
    [Route("api/gateway/files")]
    public class GatewayController: ControllerBase
    {
        private readonly IFileStorageService fileStorageService;
        private readonly IFileAnalysisService fileAnalysisService;

        public GatewayController(IFileStorageService fileStorageService, IFileAnalysisService fileAnalysisService)
        {
            this.fileStorageService = fileStorageService;
            this.fileAnalysisService = fileAnalysisService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .txt files are allowed");
            }
            try
            {
                var fileId = await fileStorageService.UploadFileAsync(file);
                return Ok(fileId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            try
            {
                var stream = await fileStorageService.DownloadFileAsync(fileId);
                return File(stream, "text/plain", $"{fileId}.txt");
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }

        [HttpGet("{fileId}/analysis")]
        public async Task<IActionResult> AnalyzeFile(Guid fileId)
        {
            try
            {
                var analysis = await fileAnalysisService.AnalyzeFileAsync(fileId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
