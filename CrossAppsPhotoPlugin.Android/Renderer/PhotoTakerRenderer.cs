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
                formsView.PropertyChanged += FormsView_PropertyChanged;
                photoTakerWidget = new PhotoTakerWidget(Context);
                photoTakerWidget.MaxImageCount = e.NewElement.MaxImageCount;
                // photoTakerWidget.TakenImagesThumbnailVisible = e.NewElement.TakenImagesThumbnailVisible;

                formsView.SaveFilesCommand = new Command(() =>
                {
                    var files = photoTakerWidget.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });

                photoTakerWidget.SendButtonTapped += PhotoTagerWidget_SendButtonTapped;
                SetNativeControl(photoTakerWidget);
            }

            if (e.NewElement != e.OldElement) 
            {
                if (e.OldElement != null)
                {
                    e.OldElement.PropertyChanged -= FormsView_PropertyChanged;
                }

                if (e.NewElement != null)
                {
                    e.NewElement.PropertyChanged += FormsView_PropertyChanged;
                }
            }
        }

        void FormsView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                /*
                case nameof(formsView.Camera):
                    photoTakerWidget.SwitchCamera();
                    break;
                    */
                case nameof(formsView.CameraSwitchVisible):
                    photoTakerWidget.SetCameraSwitchVisible(formsView.CameraSwitchVisible);
                    break;
                case nameof(formsView.MaxImageCount):
                    photoTakerWidget.MaxImageCount = formsView.MaxImageCount;
                    break;
            }

            System.Diagnostics.Debug.WriteLine(e.PropertyName);
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