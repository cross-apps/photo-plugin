using System;
using Android.Content;
using CrossAppsPhotoPlugin;
using CrossAppsPhotoPlugin.Android.Controls;
using CrossAppsPhotoPlugin.Android.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace CrossAppsPhotoPlugin.Android.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, PhotoTakerWidget>
    {
        PhotoTakerWidget photoTakerWidget;
        PhotoTakerView formsView;

        public PhotoTakerRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var name = Context.PackageName;

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