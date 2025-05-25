using FileAnalysis.Domain.Entities;
using FileAnalysis.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FileAnalysis.Tests
{
    public class AnalysisRepositoryTests
    {
        private AnalysisDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AnalysisDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AnalysisDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dbContext = CreateDbContext();
            dbContext.AnalysisResults.Add(new AnalysisResult(id, 2, 10, 100));
            dbContext.SaveChanges();
            var repo = new AnalysisRepository(dbContext);

            // Act
            var result = await repo.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.ID);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repo = new AnalysisRepository(dbContext);

            // Act
            var result = await repo.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsEntityToDbSet_AndSavesChanges()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repo = new AnalysisRepository(dbContext);
            var entity = new AnalysisResult(Guid.NewGuid(), 1, 2, 3);

            // Act
            await repo.AddAsync(entity);

            // Assert
            var data = dbContext.AnalysisResults.ToList();
            Assert.Single(data);
            Assert.Equal(entity.ID, data[0].ID);
            Assert.Equal(entity.ParagraphCount, data[0].ParagraphCount);
            Assert.Equal(entity.WordCount, data[0].WordCount);
            Assert.Equal(entity.CharacterCount, data[0].CharacterCount);
        }
    }
}