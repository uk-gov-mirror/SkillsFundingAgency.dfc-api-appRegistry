using DFC.Api.AppRegistry.Enums;

namespace DFC.Api.AppRegistry.Models
{
    public class BannerModel
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Icon { get; set; }

        public BannerPriority Priority { get; set; }
    }
}
