using FileStorage.UseCases.Interfaces;
using FileStorage.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FileStorage.Web.Tests.Controllers
{
    public class StorageControllerTests
    {
        private readonly Mock<IFileService> _fileServiceMock = new();

        private StorageController CreateController() =>
            new StorageController(_fileServiceMock.Object);

        [Fact]
        public async Task UploadFile_ReturnsOk_WithFileId()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

            var request = new UploadFileRequest { File = fileMock.Object };
            _fileServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), "test.txt"))
                .ReturnsAsync(fileId);

            var controller = CreateController();

            // Act
            var result = await controller.UploadFile(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UploadFileResponse>(okResult.Value);
            Assert.Equal(fileId, response.ID);
        }

        [Fact]
        public async Task UploadFile_Returns500_OnException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

            var request = new UploadFileRequest { File = fileMock.Object };
            _fileServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), "test.txt"))
                .ThrowsAsync(new Exception("upload error"));

            var controller = CreateController();

            // Act
            var result = await controller.UploadFile(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("upload error", objectResult.Value.ToString());
        }

        [Fact]
        public async Task DownloadFile_ReturnsOk_WithStream()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            _fileServiceMock.Setup(s => s.GetFileAsync(fileId)).ReturnsAsync(stream);

            var controller = CreateController();

            // Act
            var result = await controller.DownloadFile(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(stream, okResult.Value);
        }

        [Fact]
        public async Task DownloadFile_Returns500_OnException()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            _fileServiceMock.Setup(s => s.GetFileAsync(fileId)).ThrowsAsync(new Exception("download error"));

            var controller = CreateController();

            // Act
            var result = await controller.DownloadFile(fileId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("download error", objectResult.Value.ToString());
        }
    }
}
