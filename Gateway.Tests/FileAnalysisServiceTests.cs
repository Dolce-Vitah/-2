using Gateway.Web.Services;
using Gateway.Web.Services.Interfaces;
using Moq;
using Moq.Protected;
using Shared.DTOs;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Web.Tests.Services
{
    public class FileAnalysisServiceTests
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
        public async Task AnalyzeFileAsync_ReturnsResult_WhenSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var dto = new AnalysisResultDto { ParagraphCount = 2, WordCount = 10, CharacterCount = 100 };
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var httpClient = CreateMockHttpClient(response);
            var service = new FileAnalysisService(httpClient);

            // Act
            var result = await service.AnalyzeFileAsync(fileId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ParagraphCount);
            Assert.Equal(10, result.WordCount);
            Assert.Equal(100, result.CharacterCount);
        }

        [Fact]
        public async Task AnalyzeFileAsync_ThrowsException_WhenNotSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var httpClient = CreateMockHttpClient(response);
            var service = new FileAnalysisService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.AnalyzeFileAsync(fileId));
        }
    }
}
