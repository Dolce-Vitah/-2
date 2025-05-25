using Shared.DTOs;

namespace Gateway.Web.Services.Interfaces
{
    public interface IFileAnalysisService
    {
        Task<AnalysisResultDto> AnalyzeFileAsync(Guid fileId);
    }
}
