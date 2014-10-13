using System;
using Windows.ApplicationModel.Store;

namespace CrazyGuessing.Functions
{
    public sealed class MarketplaceReviewTask
    {
        public async void Show()
        {
            await Windows.System.Launcher.LaunchUriAsync(
                new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }
    }
}
