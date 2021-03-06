﻿using System;
using System.Threading.Tasks;
using CrossAppsPhotoPlugin.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("CrossAppsPhotoPlugin")]
[assembly: ExportEffect(typeof(OrientationEffect), "OrientationEffect")]
namespace CrossAppsPhotoPlugin.iOS.Effects
{
    public class OrientationEffect : PlatformEffect
    {
        private UIView container = null;

        protected override void OnAttached()
        {
            var control = Control;
            this.container = Container;

            try
            {
                var window = container.Window;
                var controller = window.RootViewController;

                container.NextResponder.InputViewController
                         .ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation.PortraitUpsideDown);
            }
            catch (Exception ex)
            {

            }

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                var x = container;

                try
                {

                }
                catch (Exception ex)
                {

                }
            });
        }

        protected override void OnDetached()
        {

        }
    }
}