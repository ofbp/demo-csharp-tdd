using System;
using Xunit;
using CloudAcademy.BitcoinCoverter.Code;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CloudAcademy.BitcoinConverter.Tests
{
    public class BitcoinConverterSvcShould
    {
        private const string MOCK_RESPONSE_JSON = @"{""time"": {""updated"": ""Oct 15, 2020 22:55:00 UTC"",""updatedISO"": ""2020-10-15T22:55:00+00:00"",""updateduk"": ""Oct 15, 2020 at 23:55 BST""},""chartName"": ""Bitcoin"",""bpi"": {""USD"": {""code"": ""USD"",""symbol"": ""&#36;"",""rate"": ""11,486.5341"",""description"": ""United States Dollar"",""rate_float"": 11486.5341},""GBP"": {""code"": ""GBP"",""symbol"": ""&pound;"",""rate"": ""8,900.8693"",""description"": ""British Pound Sterling"",""rate_float"": 8900.8693},""EUR"": {""code"": ""EUR"",""symbol"": ""&euro;"",""rate"": ""9,809.3278"",""description"": ""Euro"",""rate_float"": 9809.3278}}}";
        private ConverterSvc mockConverter;

        public BitcoinConverterSvcShould(){
            mockConverter = GetMockBitcoinConverterService();
        }

        private ConverterSvc GetMockBitcoinConverterService()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MOCK_RESPONSE_JSON),
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            var converter = new ConverterSvc(httpClient);

            return converter;
        }

        [Theory]
        [InlineData(Currency.USD, 11486.5341)]
        [InlineData(Currency.GBP, 8900.8693)]
        [InlineData(Currency.EUR, 9809.3278)]
        public async void GetExchangeRate_Currency_ReturnsCurrencyExchangeRate(Currency currency, double expectedValue)
        {
            //Act
            var exchangeRate = await mockConverter.GetExchangeRate(currency);

            //Assert
            Assert.Equal(expectedValue,exchangeRate);

        }

        [Theory]
        [InlineData(Currency.USD,1, 11486.5341)]
        [InlineData(Currency.GBP,1, 8900.8693)]
        [InlineData(Currency.EUR,1, 9809.3278)]       
        [InlineData(Currency.USD,2, 22973.0682)]
        [InlineData(Currency.GBP,2, 17801.7386)]
        [InlineData(Currency.EUR,2, 19618.6556)] 
        public async void ConvertBitcoins_10BitcoinToCurrency_ReturnsCurrencyDollars(Currency currency, int coins, double expectedValue)
        {
            //Act
            var bitcoinExchange = await mockConverter.ConvertBitcoins(currency,coins);

            //Assert
            Assert.Equal(expectedValue,bitcoinExchange);
        }

        /*[Theory]
        [InlineData(Currency.USD,1, -1)]
        public async void ConvertBitcoins_BitcoinsAPIServiceUnavailable_ReturnNegativeOne(Currency currency, int coins, double expectedValue)
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("Error.."),
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            var converter = new ConverterSvc(httpClient);

            //Act
            var bitcoinExchange = await converter.ConvertBitcoins(currency,coins);

            //Assert
            Assert.Equal(expectedValue,bitcoinExchange);
        }*/

        [Theory]
        [InlineData(Currency.USD,-1)]
        public async void ConvertBitcoins_BitcoinsLessThanZero_ThrowsArgumentException(Currency currency, int coins)
        {
            //act
            Task result() => mockConverter.ConvertBitcoins(currency, coins);

            //assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
    }
}
