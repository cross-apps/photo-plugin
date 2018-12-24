using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Hardware;
using Android.Views;

namespace PhotoTaker.Droid.Controls
{
    public class PhotoTakerWidget : ViewGroup
    {
        Camera camera = null;
        CameraPreviewWidget cameraPreview;
        PhotoTakerControlsOverlayView controlsOverlayView;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            initializeCamera();
            cameraPreview = new CameraPreviewWidget(context, camera);
            controlsOverlayView = new PhotoTakerControlsOverlayView(context);

            AddView(cameraPreview);
            AddView(controlsOverlayView);
        }

        public void initializeCamera() 
        {
            try
            {

            
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {

            // throw new NotImplementedException();
        }
    }
}
