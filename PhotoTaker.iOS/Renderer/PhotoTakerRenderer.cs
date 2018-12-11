using System;
using System.ComponentModel;
using PhotoTaker.Custom;
using PhotoTaker.iOS.Controls;
using PhotoTaker.iOS.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace PhotoTaker.iOS.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, UIPhotoTakerView>
    {
        UIPhotoTakerView photoTakerView;
        PhotoTakerView formsView;

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                formsView = e.NewElement;
                photoTakerView = new UIPhotoTakerView(e.NewElement.Camera);
                photoTakerView.MaxImageCount = e.NewElement.MaxImageCount;

                formsView.SaveFilesCommand = new Command(() => 
                { 
                    var files = photoTakerView.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });

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
