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
        Camera2BasicFragment camera2BasicFragment;
        PhotoTakerControlsOverlayView controlsOverlayView;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            // SetBackgroundColor(Android.Graphics.Color.Gold);

            controlsOverlayView = new PhotoTakerControlsOverlayView(context);
            // camera2BasicFragment = Camera2BasicFragment.NewInstance();

            // AddView(controlsOverlayView);
            // AddView(camera2BasicFragment);

            var textView = new TextView(context);
            textView.Text = "asdasd";
            textView.TextSize = 20;
            textView.SetTextColor(Android.Graphics.Color.Red);
            textView.LayoutParameters = new FrameLayout.LayoutParams(200, 200);
            // textView.Gravity = GravityFlags.Top;

            this.AddView(textView);
            this.AddView(controlsOverlayView);

            // this.AddView(controlsOverlayView, 200, 200);
            // controlsOverlayView.Visibility = ViewStates.Visible;
            // this.Invalidate();

            if (textView.IsShown)
            {

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
    }
}
