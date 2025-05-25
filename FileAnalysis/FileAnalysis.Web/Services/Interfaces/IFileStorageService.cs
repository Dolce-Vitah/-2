namespace FileAnalysis.Web.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<Stream> DownloadFileAsync(Guid fileId);
    }
}
