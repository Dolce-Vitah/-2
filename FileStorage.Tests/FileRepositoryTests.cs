using FileStorage.Domain.Entities;
using FileStorage.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FileStorage.Infrastructure.Tests.Database
{
    public class FileRepositoryTests
    {
        private FileDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<FileDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FileDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsFileToDatabase()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repo = new FileRepository(dbContext);
            var file = new StoredFile(Guid.NewGuid(), "file.txt", "hash1", "loc1");

            // Act
            await repo.AddAsync(file);

            // Assert
            var stored = await dbContext.Files.FirstOrDefaultAsync(f => f.ID == file.ID);
            Assert.NotNull(stored);
            Assert.Equal("file.txt", stored.FileName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFile_WhenExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var file = new StoredFile(Guid.NewGuid(), "file2.txt", "hash2", "loc2");
            dbContext.Files.Add(file);
            dbContext.SaveChanges();
            var repo = new FileRepository(dbContext);

            // Act
            var result = await repo.GetByIdAsync(file.ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(file.ID, result.ID);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repo = new FileRepository(dbContext);

            // Act
            var result = await repo.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllByHashAsync_ReturnsMatchingFiles()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var file1 = new StoredFile(Guid.NewGuid(), "a.txt", "hashX", "locA");
            var file2 = new StoredFile(Guid.NewGuid(), "b.txt", "hashX", "locB");
            var file3 = new StoredFile(Guid.NewGuid(), "c.txt", "hashY", "locC");
            dbContext.Files.AddRange(file1, file2, file3);
            dbContext.SaveChanges();
            var repo = new FileRepository(dbContext);

            // Act
            var results = (await repo.GetAllByHashAsync("hashX")).ToList();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, f => f.ID == file1.ID);
            Assert.Contains(results, f => f.ID == file2.ID);
        }
    }
}
