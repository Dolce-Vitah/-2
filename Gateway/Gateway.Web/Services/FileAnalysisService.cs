using Gateway.Web.Services.Interfaces;
using Shared.DTOs;
using System.Text.Json;

namespace Gateway.Web.Services
{
    internal class FileAnalysisService : IFileAnalysisService
    {
        private readonly HttpClient httpClient;
        public FileAnalysisService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<AnalysisResultDto> AnalyzeFileAsync(Guid fileId)
        {
            var response = await httpClient.GetAsync($"api/analysis/files/analyze/{fileId}");
            if (response.IsSuccessStatusCode)
            {
                var json =  await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AnalysisResultDto>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );             

                return result;
            }
            else
            {
                throw new Exception("File analysis failed");
            }
        }
    }
}
