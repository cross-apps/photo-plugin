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

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var formsView = e.NewElement;
                photoTakerView = new UIPhotoTakerView(e.NewElement.Camera);
                formsView.SaveFilesCommand = new Command(async () => 
                { 
                    var files = photoTakerView.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });

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
