using FileAnalysis.Web.Services.Interfaces;

namespace FileAnalysis.Web.Services
{
    internal class FileStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;

        public FileStorageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stream> DownloadFileAsync(Guid fileId)
        {
            var response = await _httpClient.GetAsync($"api/storage/files/download/{fileId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                throw new Exception("File download failed");
            }
        }
    }
}
