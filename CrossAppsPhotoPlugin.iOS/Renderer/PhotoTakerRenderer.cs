using System;
using System.ComponentModel;
using Foundation;
using CrossAppsPhotoPlugin;
using CrossAppsPhotoPlugin.iOS.Controls;
using CrossAppsPhotoPlugin.iOS.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace CrossAppsPhotoPlugin.iOS.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, UIPhotoTakerView>
    {
        UIPhotoTakerView photoTakerView;
        PhotoTakerView formsView;

        public PhotoTakerRenderer()
        {
            // placeholder
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                formsView = e.NewElement;
                photoTakerView = new UIPhotoTakerView(CameraOptions.Front);
                photoTakerView.MaxImageCount = e.NewElement.MaxImageCount;
                photoTakerView.SetCloseVisibility(e.NewElement.CloseButtonVisible);
                // photoTakerView.TakenImagesThumbnailVisible = e.NewElement.TakenImagesThumbnailVisible;

                formsView.SaveFilesCommand = new Command(() =>
                {
                    var files = photoTakerView.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });


                photoTakerView.CloseButtonTapped += PhotoTakerView_CloseButtonTapped;
                photoTakerView.SendButtonTapped += PhotoTakerView_SendButtonTapped;
                SetNativeControl(photoTakerView);
            }

            if (e.OldElement != null)
            {
                // photoTakerView.RemoveTouchEvents();
            }

            if (e.NewElement != null)
            {
                // photoTakerView.AddTouchEvents();
            }
        }

        void PhotoTakerView_CloseButtonTapped(object sender, EventArgs e)
        {
            formsView.Closed?.Invoke(this, new EventArgs());
        }


        void PhotoTakerView_SendButtonTapped(object sender, EventArgs e)
        {
            formsView?.SaveFilesCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}