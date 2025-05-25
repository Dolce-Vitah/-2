using FileAnalysis.Domain.Interfaces;
using FileAnalysis.UseCases.Interfaces;
using FileAnalysis.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace FileAnalysis.Web.Controllers
{
    [ApiController]
    [Route("api/analysis/files")]
    public class AnalysisController: ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IFileStorageService _fileStorageService;
        public AnalysisController(IStatisticsService statisticsService, IAnalysisRepository analysisRepository,
                                  IFileStorageService fileStorageService)
        {
            _statisticsService = statisticsService;
            _analysisRepository = analysisRepository;
            _fileStorageService = fileStorageService;
        }

        [HttpGet("analyze/{fileId}")]
        public async Task<IActionResult> AnalyzeFile(Guid fileId)
        {
            var existing = await _analysisRepository.GetByIdAsync(fileId);
            if (existing != null)
            {
                return Ok(new AnalysisResultDto
                {
                    ParagraphCount = existing.ParagraphCount,
                    WordCount = existing.WordCount,
                    CharacterCount = existing.CharacterCount
                });
            }

            try
            {
                var fileStream = await _fileStorageService.DownloadFileAsync(fileId);
                var result = await _statisticsService.AnalyzeFile(fileId, fileStream);
                return Ok(new AnalysisResultDto { 
                    ParagraphCount = result.ParagraphCount,
                    WordCount = result.WordCount, 
                    CharacterCount = result.CharacterCount});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
