using FileStorage.Domain.Entities;
using FileStorage.Domain.Interfaces;
using FileStorage.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.UseCases.Services
{
    internal class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;
        public FileService(IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }
        public async Task<Guid> UploadFileAsync(Stream content, string fileName)
        {
            var hash = await ComputeHash(content);
            var candidates = await _fileRepository.GetAllByHashAsync(hash);
            foreach (var candidate in candidates)
            {
                var storedContent = await _fileStorage.GetAsync(candidate.Location);
                if (await StreamsAreEqualAsync(content, storedContent))
                {
                    return candidate.ID;
                }
                content.Position = 0;
            }

            var id = Guid.NewGuid();
            var location = await _fileStorage.SaveAsync(content, fileName);
            var storedFile = new StoredFile(id, fileName, hash, location);
            await _fileRepository.AddAsync(storedFile);
            return id;
        }

        public async Task<Stream> GetFileAsync(Guid id)
        {
            var storedFile = await _fileRepository.GetByIdAsync(id);
            if (storedFile == null)
            {
                throw new FileNotFoundException("File not found", id.ToString());
            }
            return await _fileStorage.GetAsync(storedFile.Location);
        }

        private async Task<string> ComputeHash(Stream content)
        {
            using var sha256 = SHA256.Create();
            content.Position = 0;
            var hash = await sha256.ComputeHashAsync(content);
            content.Position = 0;
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private async Task<bool> StreamsAreEqualAsync(Stream stream1, Stream stream2)
        {
            if (stream1.Length != stream2.Length)
            {
                return false;
            }
            const int bufferSize = 8192;
            stream1.Position = stream2.Position = 0;
            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];

            while (true)
            {
                int bytesRead1 = await stream1.ReadAsync(buffer1, 0, bufferSize);
                int bytesRead2 = await stream2.ReadAsync(buffer2, 0, bufferSize);
                if (bytesRead1 != bytesRead2)
                {
                    return false;
                }
                if (bytesRead1 == 0)
                {
                    break;
                }
                if (!buffer1.Take(bytesRead1).SequenceEqual(buffer2.Take(bytesRead2)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
