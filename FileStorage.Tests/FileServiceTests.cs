using FileStorage.Domain.Entities;
using FileStorage.Domain.Interfaces;
using FileStorage.Infrastructure.Database;
using FileStorage.UseCases.Interfaces;
using FileStorage.UseCases.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileStorage.UseCases.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IFileRepository> _fileRepositoryMock = new();
        private readonly Mock<IFileStorage> _fileStorageMock = new();

        private FileService CreateService() =>
            new FileService(_fileRepositoryMock.Object, _fileStorageMock.Object);

        [Fact]
        public async Task UploadFileAsync_ReturnsExistingId_WhenDuplicateExists()
        {
            // Arrange
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var hash = "abc123";
            var existingId = Guid.NewGuid();
            var storedFile = new StoredFile(existingId, "file.txt", hash, "location1");

            _fileRepositoryMock.Setup(r => r.GetAllByHashAsync(It.IsAny<string>()))
                .ReturnsAsync(new[] { storedFile });
            _fileStorageMock.Setup(s => s.GetAsync(storedFile.Location))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("test content")));

            var service = CreateService();

            // Act
            var result = await service.UploadFileAsync(fileContent, "file.txt");

            // Assert
            Assert.Equal(existingId, result);
        }

        [Fact]
        public async Task UploadFileAsync_SavesAndReturnsNewId_WhenNoDuplicate()
        {
            // Arrange
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("unique content"));
            _fileRepositoryMock.Setup(r => r.GetAllByHashAsync(It.IsAny<string>()))
                .ReturnsAsync(Array.Empty<StoredFile>());
            _fileStorageMock.Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("new-location");

            Guid? addedId = null;
            _fileRepositoryMock.Setup(r => r.AddAsync(It.IsAny<StoredFile>()))
                .Callback<StoredFile>(f => addedId = f.ID)
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            var result = await service.UploadFileAsync(fileContent, "file2.txt");

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            Assert.Equal(addedId, result);
        }

        [Fact]
        public async Task GetFileAsync_ReturnsStream_WhenFileExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var storedFile = new StoredFile(id, "file.txt", "hash", "location");
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("abc"));

            _fileRepositoryMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(storedFile);
            _fileStorageMock.Setup(s => s.GetAsync(storedFile.Location))
                .ReturnsAsync(fileStream);

            var service = CreateService();

            // Act
            var result = await service.GetFileAsync(id);

            // Assert
            Assert.Equal(fileStream, result);
        }

        [Fact]
        public async Task GetFileAsync_Throws_WhenFileNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileRepositoryMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((StoredFile?)null);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.GetFileAsync(id));
        }
    }
}
