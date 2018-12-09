using System;
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
                photoTakerView = new UIPhotoTakerView(e.NewElement.Camera);
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
