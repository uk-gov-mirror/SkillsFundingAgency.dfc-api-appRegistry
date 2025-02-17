﻿using DFC.Api.AppRegistry.Contracts;
using DFC.Api.AppRegistry.Functions;
using DFC.Api.AppRegistry.Models.Legacy;
using DFC.Api.AppRegistry.UnitTests.TestModels;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Api.AppRegistry.UnitTests.FunctionsTests
{
    [Trait("Category", "RegionsCosmosDbTrigger - tests")]
    public class RegionsCosmosDbTriggerTests
    {
        private readonly ILogger<RegionsCosmosDbTrigger> logger;
        private readonly ILegacyDataLoadService legacyDataLoadService;

        public RegionsCosmosDbTriggerTests()
        {
            logger = A.Fake<ILogger<RegionsCosmosDbTrigger>>();
            legacyDataLoadService = A.Fake<ILegacyDataLoadService>();
        }

        [Fact]
        public async Task RegionsCosmosDbTriggerWhenExecutedUpdatesRegion()
        {
            // Arrange
            var function = new RegionsCosmosDbTrigger(logger, legacyDataLoadService);

            // Act
            await function.Run(new List<Document> { ModelBuilders.ValidLegacyRegionModel(ModelBuilders.PathName, Enums.PageRegion.Head) }).ConfigureAwait(false);

            // Assert
            A.CallTo(() => legacyDataLoadService.UpdateRegionAsync(A<LegacyRegionModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RegionsCosmosDbTriggerWhenExecutedThrowsException()
        {
            A.CallTo(() => legacyDataLoadService.UpdateRegionAsync(A<LegacyRegionModel>.Ignored)).ThrowsAsync(new HttpRequestException());

            // Arrange
            var function = new RegionsCosmosDbTrigger(logger, legacyDataLoadService);

            // Act
            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => function.Run(new List<Document> { ModelBuilders.ValidLegacyRegionModel(ModelBuilders.PathName, Enums.PageRegion.Head) })).ConfigureAwait(false);
        }
    }
}
