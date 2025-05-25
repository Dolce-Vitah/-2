using Gateway.Web.Controllers;
using Gateway.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Web.Tests.Controllers
{
    public class GatewayControllerTests
    {
        private readonly Mock<IFileStorageService> _fileStorageServiceMock = new();
        private readonly Mock<IFileAnalysisService> _fileAnalysisServiceMock = new();

        private GatewayController CreateController() =>
            new GatewayController(_fileStorageServiceMock.Object, _fileAnalysisServiceMock.Object);

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
            var request = new UploadFileRequest { File = null! };
            var controller = CreateController();

            var result = await controller.UploadFile(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            var request = new UploadFileRequest { File = fileMock.Object };
            var controller = CreateController();

            var result = await controller.UploadFile(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNotTxt()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("file.pdf");
            var request = new UploadFileRequest { File = fileMock.Object };
            var controller = CreateController();

            var result = await controller.UploadFile(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Only .txt files are allowed", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_ReturnsOk_WhenFileIsValid()
        {
            var fileId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            _fileStorageServiceMock.Setup(s => s.UploadFileAsync(fileMock.Object)).ReturnsAsync(fileId);

            var request = new UploadFileRequest { File = fileMock.Object };
            var controller = CreateController();

            var result = await controller.UploadFile(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(fileId, okResult.Value);
        }

        [Fact]
        public async Task UploadFile_Returns500_OnException()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            _fileStorageServiceMock.Setup(s => s.UploadFileAsync(fileMock.Object)).ThrowsAsync(new Exception("upload error"));

            var request = new UploadFileRequest { File = fileMock.Object };
            var controller = CreateController();

            var result = await controller.UploadFile(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("upload error", objectResult.Value.ToString());
        }

        [Fact]
        public async Task DownloadFile_ReturnsFileResult_WhenSuccess()
        {
            var fileId = Guid.NewGuid();
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            _fileStorageServiceMock.Setup(s => s.DownloadFileAsync(fileId)).ReturnsAsync(stream);

            var controller = CreateController();

            var result = await controller.DownloadFile(fileId);

            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/plain", fileResult.ContentType);
            Assert.Equal($"{fileId}.txt", fileResult.FileDownloadName);
            Assert.Equal(stream, fileResult.FileStream);
        }

        [Fact]
        public async Task DownloadFile_Returns500_OnException()
        {
            var fileId = Guid.NewGuid();
            _fileStorageServiceMock.Setup(s => s.DownloadFileAsync(fileId)).ThrowsAsync(new Exception("download error"));

            var controller = CreateController();

            var result = await controller.DownloadFile(fileId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("download error", objectResult.Value.ToString());
        }

        [Fact]
        public async Task AnalyzeFile_ReturnsOk_WhenSuccess()
        {
            var fileId = Guid.NewGuid();
            var dto = new AnalysisResultDto { ParagraphCount = 1, WordCount = 2, CharacterCount = 3 };
            _fileAnalysisServiceMock.Setup(s => s.AnalyzeFileAsync(fileId)).ReturnsAsync(dto);

            var controller = CreateController();

            var result = await controller.AnalyzeFile(fileId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task AnalyzeFile_Returns500_OnException()
        {
            var fileId = Guid.NewGuid();
            _fileAnalysisServiceMock.Setup(s => s.AnalyzeFileAsync(fileId)).ThrowsAsync(new Exception("analysis error"));

            var controller = CreateController();

            var result = await controller.AnalyzeFile(fileId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("analysis error", objectResult.Value.ToString());
        }
    }
}
