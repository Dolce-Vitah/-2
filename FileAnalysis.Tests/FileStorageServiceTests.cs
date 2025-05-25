using FileAnalysis.Web.Services;
using FileAnalysis.Web.Services.Interfaces;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileAnalysis.Web.Tests.Services
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
        public async Task DownloadFileAsync_ReturnsStream_WhenSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var expectedContent = new MemoryStream(new byte[] { 1, 2, 3 });
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(expectedContent.ToArray()))
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
            Assert.Equal(new byte[] { 1, 2, 3 }, buffer);
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
