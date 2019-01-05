using System;
using CrossAppsPhotoPlugin;
using CrossAppsPhotoPlugin.iOS.Renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

// [assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace CrossAppsPhotoPlugin.iOS.Renderer
{
    public class MainPageRenderer : PageRenderer
    {
        public override bool ShouldAutorotate()
        {
            return false;
        }
    }
}
