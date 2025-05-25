using FileStorage.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.FileSystem
{
    public class LocalFileStorage: IFileStorage
    {
        private readonly string storagePath;
        public LocalFileStorage(IConfiguration config)
        {
            storagePath = config["Storage:RootPath"] ?? "storage";
            Directory.CreateDirectory(storagePath);
        }
        public async Task<string> SaveAsync(Stream fileStream, string fileName)
        {
            var filePath = Path.Combine(storagePath, fileName);
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            
            await fileStream.CopyToAsync(fs);
            
            return filePath;
        }
        public async Task<Stream> GetAsync(string location)
        {
            if (!File.Exists(location))
            {
                throw new FileNotFoundException("File not found", location);
            }

            return new FileStream(location, FileMode.Open, FileAccess.Read);
        }
    }    
}
