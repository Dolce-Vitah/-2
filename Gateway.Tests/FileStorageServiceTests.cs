using Gateway.Web.Services;
using Gateway.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Shared.DTOs;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Web.Tests.Services
{
    public class FileStorageServiceTests
    {
        private static HttpClient CreateMockHttpClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
        }

        [Fact]
        public async Task UploadFileAsync_ReturnsId_WhenSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var uploadResponse = new UploadFileResponse { ID = fileId };
            var json = System.Text.Json.JsonSerializer.Serialize(uploadResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var httpClient = CreateMockHttpClient(response);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

            var service = new FileStorageService(httpClient);

            // Act
            var result = await service.UploadFileAsync(fileMock.Object);

            // Assert
            Assert.Equal(fileId, result);
        }

        [Fact]
        public async Task UploadFileAsync_ThrowsException_WhenNotSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var httpClient = CreateMockHttpClient(response);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

            var service = new FileStorageService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UploadFileAsync(fileMock.Object));
        }

        [Fact]
        public async Task DownloadFileAsync_ReturnsStream_WhenSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var expectedContent = new byte[] { 1, 2, 3 };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(expectedContent))
            };
            var httpClient = CreateMockHttpClient(response);
            var service = new FileStorageService(httpClient);

            // Act
            var result = await service.DownloadFileAsync(fileId);

            // Assert
            Assert.NotNull(result);
            var buffer = new byte[3];
            var bytesRead = await result.ReadAsync(buffer, 0, buffer.Length);
            Assert.Equal(3, bytesRead);
            Assert.Equal(expectedContent, buffer);
        }

        [Fact]
        public async Task DownloadFileAsync_ThrowsException_WhenNotSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var httpClient = CreateMockHttpClient(response);
            var service = new FileStorageService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DownloadFileAsync(fileId));
        }
    }
}
