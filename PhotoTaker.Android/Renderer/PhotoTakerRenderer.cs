using System;
using PhotoTaker.Custom;
using PhotoTaker.Droid.Controls;
using PhotoTaker.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace PhotoTaker.Droid.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, PhotoTakerWidget>
    {
        PhotoTakerWidget photoTakerWidget;
        PhotoTakerView formsView;

        public PhotoTakerRenderer(Android.Content.Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                formsView = e.NewElement;
                photoTakerWidget = new PhotoTakerWidget(Context);
                photoTakerWidget.MaxImageCount = e.NewElement.MaxImageCount;
                photoTakerWidget.TakenImagesThumbnailVisible = e.NewElement.TakenImagesThumbnailVisible;

                formsView.SaveFilesCommand = new Command(() =>
                {
                    var files = photoTakerWidget.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });

                photoTakerWidget.SendButtonTapped += PhotoTagerWidget_SendButtonTapped;
                SetNativeControl(photoTakerWidget);
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

        void PhotoTagerWidget_SendButtonTapped(object sender, EventArgs e)
        {
            formsView?.SaveFilesCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
