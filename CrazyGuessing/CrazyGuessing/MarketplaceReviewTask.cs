using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace CrazyGuessing
{
    public sealed class MarketplaceReviewTask
    {
        public MarketplaceReviewTask()
        { }

        public async void Show()
        {
            await Windows.System.Launcher.LaunchUriAsync(
                new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }
    }
}
