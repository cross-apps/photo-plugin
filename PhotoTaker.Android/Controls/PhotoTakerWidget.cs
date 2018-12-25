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
        Camera2BasicFragment camera2BasicFragment;
        PhotoTakerControlsOverlayView controlsOverlayView;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            controlsOverlayView = new PhotoTakerControlsOverlayView(context);
            camera2BasicFragment = Camera2BasicFragment.NewInstance();

            AddView(controlsOverlayView);
            AddView(camera2BasicFragment);
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
