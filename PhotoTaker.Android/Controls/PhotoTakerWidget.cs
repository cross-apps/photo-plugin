using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Views;
using Android.Widget;

namespace PhotoTaker.Droid.Controls
{
    public class PhotoTakerWidget : FrameLayout
    {
        CameraWidget cameraWidget;
        PhotoTakerControlsOverlayView controlsOverlayView;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            // SetBackgroundColor(Android.Graphics.Color.Gold);

            controlsOverlayView = new PhotoTakerControlsOverlayView(context);
            controlsOverlayView.TakeButtonTouched += ControlsOverlayView_TakeButtonTouched;
            cameraWidget = new CameraWidget(context);
            // cameraWidget.OpenCamera(200, 200);
            // AddView(controlsOverlayView);
            // AddView(camera2BasicFragment);

            AddView(cameraWidget.mTextureView);
            // AddView(controlsOverlayView);
        }

        void ControlsOverlayView_TakeButtonTouched(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Take button touched");
        }

        public List<string> SaveFiles()
        {
            List<string> fileNames = new List<string>();

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine(documents, "..", "tmp");

            /*
            foreach (var image in takenPhotosOverlayView.Photos)
            {
                var fileName = Path.Combine(tmp, Guid.NewGuid().ToString() + ".jpg");
                image.AsJPEG().Save(fileName, true);
                fileNames.Add(fileName);
            }
            */

            return fileNames;
        }
    }
}
