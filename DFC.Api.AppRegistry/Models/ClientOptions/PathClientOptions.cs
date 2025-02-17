﻿using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.AppRegistry.Models.ClientOptions
{
    [ExcludeFromCodeCoverage]
    public class PathClientOptions : ClientOptionsModel
    {
        public string Endpoint { get; set; } = "api/paths";
    }
}
