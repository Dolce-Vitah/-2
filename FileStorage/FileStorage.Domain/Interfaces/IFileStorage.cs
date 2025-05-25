using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Domain.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(Stream content, string fileName);
        Task<Stream> GetAsync(string location);
    }
}
