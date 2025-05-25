using FileAnalysis.Domain.Entities;
using FileAnalysis.Domain.Interfaces;
using FileAnalysis.UseCases.Interfaces;
using FileAnalysis.Web.Controllers;
using FileAnalysis.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FileAnalysis.Web.Tests.Controllers
{
    public class AnalysisControllerTests
    {
        private readonly Mock<IStatisticsService> _statisticsServiceMock = new();
        private readonly Mock<IAnalysisRepository> _analysisRepositoryMock = new();
        private readonly Mock<IFileStorageService> _fileStorageServiceMock = new();

        private AnalysisController CreateController() =>
            new AnalysisController(_statisticsServiceMock.Object, _analysisRepositoryMock.Object, _fileStorageServiceMock.Object);

        [Fact]
        public async Task AnalyzeFile_ReturnsCachedResult_WhenExists()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var analysis = new AnalysisResult(fileId, 2, 10, 100);
            _analysisRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync(analysis);

            var controller = CreateController();

            // Act
            var result = await controller.AnalyzeFile(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<AnalysisResultDto>(okResult.Value);
            Assert.Equal(2, dto.ParagraphCount);
            Assert.Equal(10, dto.WordCount);
            Assert.Equal(100, dto.CharacterCount);
        }

        [Fact]
        public async Task AnalyzeFile_AnalyzesFile_WhenNotCached()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            _analysisRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync((AnalysisResult?)null);
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            _fileStorageServiceMock.Setup(s => s.DownloadFileAsync(fileId)).ReturnsAsync(stream);
            var analysis = new AnalysisResult(fileId, 5, 20, 200);
            _statisticsServiceMock.Setup(s => s.AnalyzeFile(fileId, stream)).ReturnsAsync(analysis);

            var controller = CreateController();

            // Act
            var result = await controller.AnalyzeFile(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<AnalysisResultDto>(okResult.Value);
            Assert.Equal(5, dto.ParagraphCount);
            Assert.Equal(20, dto.WordCount);
            Assert.Equal(200, dto.CharacterCount);
        }

        [Fact]
        public async Task AnalyzeFile_Returns500_WhenExceptionThrown()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            _analysisRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync((AnalysisResult?)null);
            _fileStorageServiceMock.Setup(s => s.DownloadFileAsync(fileId)).ThrowsAsync(new Exception("fail"));

            var controller = CreateController();

            // Act
            var result = await controller.AnalyzeFile(fileId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("fail", objectResult.Value.ToString());
        }
    }
}
