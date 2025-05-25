namespace Gateway.Web.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<Guid> UploadFileAsync(IFormFile file);
        Task<Stream> DownloadFileAsync(Guid fileId);
    }
}
