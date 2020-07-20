﻿using Castle.Core.Logging;
using DFC.Api.AppRegistry.Contracts;
using DFC.Api.AppRegistry.Models;
using DFC.Api.AppRegistry.Models.ClientOptions;
using DFC.Api.AppRegistry.Models.Pages;
using DFC.Api.AppRegistry.Services;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Api.AppRegistry.UnitTests.ServicesTests
{
    [Trait("Category", "LegacyPath - Service tests")]
    public class PagesDataLoadServiceTests
    {
        private static readonly List<PageModel> PageModels = new List<PageModel>() { new PageModel { Url = new Uri("http://somewhere.com/1"), CanonicalName = "A Name", RedirectLocations = new List<string> { "http://somewhere/somewherelese" } }, new PageModel { Url = new Uri("http://somewhere.com/2") } };

        private readonly ILogger<PagesDataLoadService> logger = A.Fake<ILogger<PagesDataLoadService>>();
        private readonly HttpClient fakeHttpClient = A.Fake<HttpClient>();
        private readonly IApiDataService fakeApiDataService = A.Fake<IApiDataService>();
        private readonly ILegacyDataLoadService fakeLegacyDataLoadService = A.Fake<ILegacyDataLoadService>();
        private readonly PagesClientOptions pagesClientOptions;

        public PagesDataLoadServiceTests()
        {
            this.pagesClientOptions = new PagesClientOptions { BaseAddress = new Uri("http://somewhere.com"), SummaryEndpoint = "pages" };
        }

        [Fact]
        public async Task PagesDataLoadServiceLoadAsyncNullRegistrationDocumentNoUpdate()
        {
            // Arrange
            AppRegistrationModel? model = null;
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);

            // Act
            await serviceToTest.LoadAsync().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task PagesDataLoadServiceRemoveAsyncNullRegistrationDocumentNoUpdate()
        {
            // Arrange
            AppRegistrationModel? model = null;
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);

            // Act
            await serviceToTest.RemoveAsync(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task PagesDataLoadServiceLoadAsyncNullPagesNoRegistrationDocumentUpdated()
        {
            // Arrange
            AppRegistrationModel model = new AppRegistrationModel { Path = "/pages" };
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);

            List<PageModel>? pageModel = null;

            A.CallTo(() => fakeApiDataService.GetAsync<List<PageModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(pageModel);

            // Act
            await serviceToTest.LoadAsync().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataService.GetAsync<List<PageModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PagesDataLoadServiceLoadAsyncRegistrationDocumentUpdated()
        {
            // Arrange
            AppRegistrationModel model = new AppRegistrationModel { Path = "/pages" };
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);
            A.CallTo(() => fakeApiDataService.GetAsync<List<PageModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(PageModels);

            // Act
            await serviceToTest.LoadAsync().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeApiDataService.GetAsync<List<PageModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PagesDataLoadServiceCreateOrUpdateAsyncRegistrationDocumentUpdated()
        {
            // Arrange
            AppRegistrationModel model = new AppRegistrationModel { Path = "/pages", Locations = new List<string> { "http://somewhere.com/a-place-a-page" } };
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(PageModels.FirstOrDefault());

            // Act
            await serviceToTest.CreateOrUpdateAsync(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PagesDataLoadServiceCreateOrUpdateAsyncNullPagesNoRegistrationDocumentUpdated()
        {
            // Arrange
            AppRegistrationModel model = new AppRegistrationModel { Path = "/pages" };
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);

            PageModel? pageModel = null;

            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(pageModel);

            // Act
            var result = await serviceToTest.CreateOrUpdateAsync(Guid.Parse("66088614-5c55-4698-8382-42b47ec0be10")).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PagesDataLoadServiceCreateAsyncNullRegistrationDocumentNoUpdate()
        {
            // Arrange
            AppRegistrationModel? model = null;
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);

            // Act
            await serviceToTest.CreateOrUpdateAsync(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task PagesDataLoadServiceRemoveAsyncRegistrationDocumentUpdated()
        {
            // Arrange
            AppRegistrationModel model = new AppRegistrationModel { Path = "/pages", Locations = new List<string> { "http://somewhere.com/66088614-5c55-4698-8382-42b47ec0be10" } };
            var serviceToTest = new PagesDataLoadService(logger, fakeHttpClient, pagesClientOptions, fakeApiDataService, fakeLegacyDataLoadService);
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).Returns(model);
            A.CallTo(() => fakeApiDataService.GetAsync<PageModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(PageModels.FirstOrDefault());

            // Act
            await serviceToTest.RemoveAsync(Guid.Parse("66088614-5c55-4698-8382-42b47ec0be10")).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyDataLoadService.GetAppRegistrationByPathAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLegacyDataLoadService.UpdateAppRegistrationAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
