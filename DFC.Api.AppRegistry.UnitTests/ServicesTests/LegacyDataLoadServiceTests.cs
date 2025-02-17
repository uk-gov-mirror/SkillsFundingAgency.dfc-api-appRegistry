﻿using DFC.Api.AppRegistry.Contracts;
using DFC.Api.AppRegistry.Models;
using DFC.Api.AppRegistry.Models.Legacy;
using DFC.Api.AppRegistry.Services;
using DFC.Api.AppRegistry.UnitTests.TestModels;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Api.AppRegistry.UnitTests.ServicesTests
{
    [Trait("Category", "LegacyDataLoad - Service tests")]
    public class LegacyDataLoadServiceTests
    {
        private readonly ILogger<LegacyDataLoadService> fakeLogger = A.Fake<ILogger<LegacyDataLoadService>>();
        private readonly IModelMappingService fakeModelMappingService = A.Fake<IModelMappingService>();
        private readonly IModelValidationService fakeModelValidationService = A.Fake<IModelValidationService>();
        private readonly IDocumentService<AppRegistrationModel> fakeDocumentService = A.Fake<IDocumentService<AppRegistrationModel>>();
        private readonly ILegacyPathService fakeLegacyPathService = A.Fake<ILegacyPathService>();
        private readonly ILegacyRegionService fakeLegacyRegionService = A.Fake<ILegacyRegionService>();
        private readonly LegacyDataLoadService legacyDataLoadService;

        public LegacyDataLoadServiceTests()
        {
            legacyDataLoadService = new LegacyDataLoadService(fakeLogger, fakeModelMappingService, fakeModelValidationService, fakeDocumentService, fakeLegacyPathService, fakeLegacyRegionService);
        }

        [Fact]
        public async Task LoadAsyncIsSuccessfulWhenDataPresent()
        {
            // Arrange
            var validLegacyPathModels = ModelBuilders.ValidLegacyPathModels();

            A.CallTo(() => fakeLegacyPathService.GetListAsync()).Returns(validLegacyPathModels);

            // Act
            await legacyDataLoadService.LoadAsync().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyPathService.GetListAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task LoadAsyncIsSuccessfulWhenNoDataPresent()
        {
            // Arrange
            IList<LegacyPathModel>? dummyLegacyPathModels = null;

            A.CallTo(() => fakeLegacyPathService.GetListAsync()).Returns(dummyLegacyPathModels);

            // Act
            await legacyDataLoadService.LoadAsync().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyPathService.GetListAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessPathsAsyncIsSuccessfulWhenDataPresent()
        {
            // Arrange
            var validLegacyPathModels = ModelBuilders.ValidLegacyPathModels();

            // Act
            await legacyDataLoadService.ProcessPathsAsync(validLegacyPathModels).ConfigureAwait(false);

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task ProcessPathsAsyncReturnsExceptionForNullLegacyPathmodels()
        {
            // Arrange
            IList<LegacyPathModel>? dummyLegacyPathModels = null;

            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await legacyDataLoadService.ProcessPathsAsync(dummyLegacyPathModels).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'legacyPathModels')", exceptionResult.Message);
        }

        [Fact]
        public async Task ProcessPathAsyncUpsertIsSuccessful()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.OK;
            const bool validationResult = true;
            var validLegacyPathModel = ModelBuilders.ValidLegacyPathModel(ModelBuilders.PathName);
            var validLegacyRegionModels = ModelBuilders.ValidLegacyRegionModels();
            var validAppRegistrationModels = ModelBuilders.ValidAppRegistrationModels(ModelBuilders.PathName);

            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).Returns(validLegacyRegionModels);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(validAppRegistrationModels);
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored));
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.ProcessPathAsync(validLegacyPathModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateRegionAsyncUpsertIsSuccessful()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.OK;
            const bool validationResult = true;
            var validLegacyRegionModel = ModelBuilders.ValidLegacyRegionModel(ModelBuilders.PathName, Enums.PageRegion.Head);
            var validAppRegistrationModels = ModelBuilders.ValidAppRegistrationModels(ModelBuilders.PathName);

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(validAppRegistrationModels);
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.UpdateRegionAsync(validLegacyRegionModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateRegionAsyncAppRegistrationNullNoUpdate()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.OK;
            const bool validationResult = true;
            var validLegacyRegionModel = ModelBuilders.ValidLegacyRegionModel(ModelBuilders.PathName, Enums.PageRegion.Head);

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns<IEnumerable<AppRegistrationModel>?>(null);
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.UpdateRegionAsync(validLegacyRegionModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateRegionAsyncReturnsExceptionForNullLegacyRegionModels()
        {
            // Arrange
            LegacyRegionModel? dummyLegacyRegionModel = null;

            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await legacyDataLoadService.UpdateRegionAsync(dummyLegacyRegionModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal("Value cannot be null. (Parameter 'legacyRegionModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task UpdatePathAsyncReturnsExceptionForNullLegacyPathModels()
        {
            // Arrange
            LegacyPathModel? dummyLegacyPathModel = null;

            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await legacyDataLoadService.UpdatePathAsync(dummyLegacyPathModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal("Value cannot be null. (Parameter 'legacyPathModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task UpdatePathAsyncAppRegistrationNullCreatesNew()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.OK;
            const bool validationResult = true;
            var validLegacyPathModel = ModelBuilders.ValidLegacyPathModel(ModelBuilders.PathName);

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns<IEnumerable<AppRegistrationModel>?>(null);
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.UpdatePathAsync(validLegacyPathModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessPathAsyncUpsertFailure()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.AlreadyReported;
            const bool validationResult = true;
            var validLegacyPathModel = ModelBuilders.ValidLegacyPathModel(ModelBuilders.PathName);
            var validLegacyRegionModels = ModelBuilders.ValidLegacyRegionModels();
            var validAppRegistrationModels = ModelBuilders.ValidAppRegistrationModels(ModelBuilders.PathName);

            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).Returns(validLegacyRegionModels);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(validAppRegistrationModels);
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored));
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.ProcessPathAsync(validLegacyPathModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessPathAsyncFailsValidation()
        {
            // Arrange
            const bool validationResult = false;
            var validLegacyPathModel = ModelBuilders.ValidLegacyPathModel(ModelBuilders.PathName);
            var invalidLegacyRegionModels = A.CollectionOfDummy<LegacyRegionModel>(2);
            var validAppRegistrationModels = ModelBuilders.ValidAppRegistrationModels(ModelBuilders.PathName);

            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).Returns(invalidLegacyRegionModels);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(validAppRegistrationModels);
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored));
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);

            // Act
            await legacyDataLoadService.ProcessPathAsync(validLegacyPathModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ProcessPathAsyncReturnsExceptionForNullLegacyPathModels()
        {
            // Arrange
            LegacyPathModel? dummyLegacyPathModel = null;

            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await legacyDataLoadService.ProcessPathAsync(dummyLegacyPathModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeLegacyRegionService.GetListAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal("Value cannot be null. (Parameter 'legacyPathModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task GetAppRegistrationByPathAsyncReturnsAppRegistrationModel()
        {
            // Arrange
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(new List<AppRegistrationModel> { new AppRegistrationModel { Path = PagesDataLoadService.AppRegistryPathNameForPagesApp } });

            // Act
            var result = await legacyDataLoadService.GetAppRegistrationByPathAsync("a-path").ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAppRegistrationByPathAsyncReturnsNull()
        {
            // Arrange
            IEnumerable<AppRegistrationModel>? models = null;
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).Returns(models);

            // Act
            var result = await legacyDataLoadService.GetAppRegistrationByPathAsync("a-path").ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<AppRegistrationModel, bool>>>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeModelMappingService.MapModels(A<AppRegistrationModel>.Ignored, A<LegacyPathModel>.Ignored, A<List<LegacyRegionModel>>.Ignored)).MustNotHaveHappened();

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAppRegistrationAsyncReturnsExceptionForNullAppRegistrationModel()
        {
            // Arrange
            AppRegistrationModel? dummyAppRegistrationModel = null;

            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await legacyDataLoadService.UpdateAppRegistrationAsync(dummyAppRegistrationModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal("Value cannot be null. (Parameter 'appRegistrationModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task UpdateAppRegistrationAsyncAppRegistrationxxxxxxxxxxxxxxx()
        {
            // Arrange
            const HttpStatusCode upsertResult = HttpStatusCode.OK;
            const bool validationResult = true;
            var validAppRegistrationModel = ModelBuilders.ValidAppRegistrationModel(ModelBuilders.PathName);

            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).Returns(validationResult);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).Returns(upsertResult);

            // Act
            await legacyDataLoadService.UpdateAppRegistrationAsync(validAppRegistrationModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeModelValidationService.ValidateModel(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<AppRegistrationModel>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
