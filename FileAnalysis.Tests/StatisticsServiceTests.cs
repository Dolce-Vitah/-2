using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileAnalysis.Domain.Entities;
using FileAnalysis.Domain.Interfaces;
using FileAnalysis.UseCases.Interfaces;
using FileAnalysis.UseCases.Services;
using Moq;
using Xunit;

namespace FileAnalysis.Tests
{
    public class StatisticsServiceTests
    {
        [Fact]
        public async Task AnalyzeFile_ReturnsCorrectCounts_ForSimpleText()
        {
            // Arrange  
            var repoMock = new Mock<IAnalysisRepository>();
            var service = new StatisticsService(repoMock.Object);   
            var id = Guid.NewGuid();
            var text = "Hello world!\n\nThis is a test.";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            // Act  
            var result = await service.AnalyzeFile(id, stream);

            // Assert  
            Assert.Equal(id, result.ID);
            Assert.Equal(2, result.ParagraphCount);
            Assert.Equal(6, result.WordCount);
            Assert.Equal(29, result.CharacterCount);

            repoMock.Verify(r => r.AddAsync(It.IsAny<AnalysisResult>()), Times.Once);
        }

        [Fact]
        public async Task AnalyzeFile_ReturnsZeroCounts_ForEmptyText()
        {
            var repoMock = new Mock<IAnalysisRepository>();
            var service = new StatisticsService(repoMock.Object);
            var id = Guid.NewGuid();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));

            var result = await service.AnalyzeFile(id, stream);

            Assert.Equal(0, result.ParagraphCount);
            Assert.Equal(0, result.WordCount);
            Assert.Equal(0, result.CharacterCount);
            repoMock.Verify(r => r.AddAsync(It.IsAny<AnalysisResult>()), Times.Once);
        }

        [Fact]
        public async Task AnalyzeFile_CountsSymbolCharacters()
        {
            var repoMock = new Mock<IAnalysisRepository>();
            var service = new StatisticsService(repoMock.Object);
            var id = Guid.NewGuid();
            var text = "Hello $world! @2024 #test";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var result = await service.AnalyzeFile(id, stream);

            // '$', '@', and '#' are symbol characters  
            Assert.Equal(25, result.CharacterCount);
            repoMock.Verify(r => r.AddAsync(It.IsAny<AnalysisResult>()), Times.Once);
        }
    }
}
