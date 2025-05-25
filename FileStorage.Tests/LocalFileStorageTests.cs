using FileStorage.Infrastructure.FileSystem;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileStorage.Infrastructure.Tests.FileSystem
{
    public class LocalFileStorageTests : IDisposable
    {
        private readonly string _tempDir;

        public LocalFileStorageTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        private LocalFileStorage CreateStorage()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("Storage:RootPath", _tempDir)
                })
                .Build();

            return new LocalFileStorage(config);
        }

        [Fact]
        public async Task SaveAsync_WritesFile_AndReturnsPath()
        {
            // Arrange
            var storage = CreateStorage();
            var fileName = "testfile.txt";
            var content = "Hello, world!";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var path = await storage.SaveAsync(stream, fileName);

            // Assert
            Assert.True(File.Exists(path));
            var fileContent = await File.ReadAllTextAsync(path);
            Assert.Equal(content, fileContent);
        }

        [Fact]
        public async Task GetAsync_ReturnsStream_WhenFileExists()
        {
            // Arrange
            var storage = CreateStorage();
            var fileName = "existing.txt";
            var filePath = Path.Combine(_tempDir, fileName);
            var content = "abc123";
            await File.WriteAllTextAsync(filePath, content);

            // Act
            using var stream = await storage.GetAsync(filePath);
            using var reader = new StreamReader(stream);
            var readContent = await reader.ReadToEndAsync();

            // Assert
            Assert.Equal(content, readContent);
        }

        [Fact]
        public async Task GetAsync_Throws_WhenFileDoesNotExist()
        {
            // Arrange
            var storage = CreateStorage();
            var filePath = Path.Combine(_tempDir, "missing.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => storage.GetAsync(filePath));
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }
    }
}

