using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.UseCases.Interfaces
{
    public interface IFileService
    {
        Task<Guid> UploadFileAsync(Stream content, string fileName);
        Task<Stream> GetFileAsync(Guid id);
    }
}
