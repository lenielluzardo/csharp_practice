using Http_Client;
using Http_Client.Handlers;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTests.Http_Client
{
   public class TestableClassWithApiAccessShould
   {
      [Fact]
      public void GetMovie_On401Response_And_ThrowUnauthorizedApiAccessException()
      {
         var httpClient = new HttpClient(new Return401UnauthorizedResponseHandler());
         var testableClass = new TestableClassWithApiAccess(httpClient);

         var cancellationTokenSource = new CancellationTokenSource();

         Assert.ThrowsAsync<UnauthorizedApiAccessException>(() => testableClass.GetMovie(cancellationTokenSource.Token));
      }
      [Fact]
      public void GetMovie_On401Response_And_ThrowUnauthorizedApiAccessException_WithMoq()
      {
         var unauthorizedResopnseHttpMessageHandlerMock = new Mock<HttpMessageHandler>();

         unauthorizedResopnseHttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage()
            {
               StatusCode = HttpStatusCode.Unauthorized
            });

         var httpClient = new HttpClient(unauthorizedResopnseHttpMessageHandlerMock.Object);

         var testableClass = new TestableClassWithApiAccess(httpClient);

         var cancellationTokenSource = new CancellationTokenSource();

         Assert.ThrowsAsync<UnauthorizedApiAccessException>(() => testableClass.GetMovie(cancellationTokenSource.Token));
      }
   }
}
