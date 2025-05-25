using Gateway.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.Text.Json;

namespace Gateway.Web.Services
{
    internal class FileStorageService: IFileStorageService
    {
        private readonly HttpClient httpClient;

        public FileStorageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<Guid> UploadFileAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            content.Add(new StreamContent(fileStream), "File", file.FileName);

            var response = await httpClient.PostAsync("api/storage/files/upload", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var uploadResponse = JsonSerializer.Deserialize<UploadFileResponse>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                return uploadResponse.ID;
            }
            else
            {
                throw new Exception("File upload failed");
            }

        }
        public async Task<Stream> DownloadFileAsync(Guid fileId)
        {
            var response = await httpClient.GetAsync($"api/storage/files/download/{fileId}");
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
