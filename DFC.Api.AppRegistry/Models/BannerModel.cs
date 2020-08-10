using System;

namespace DFC.Api.AppRegistry.Models
{
    public class BannerModel
    {
        public string? Heading { get; set; }

        public string ShortDescription { get; set; }

        public Uri URL { get; set; }

        public string LinkText { get; set; }

        public bool GlobalBanner { get; set; }
    }
}
