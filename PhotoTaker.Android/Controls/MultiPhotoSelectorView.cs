using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Widget;

namespace PhotoTaker.Droid.Controls
{
	public class MultiPhotoSelectorView : FrameLayout
    {
        ImageView currentImage;
        CurrentTakenPhotosOverlayView takenPhotosOverlayView;
        MultiPhotoSelectioControlsOverlayView controlsOverlayView;
        ObservableCollection<string> Photos;

        public EventHandler CloseButtonTouched { get; set; }

        public MultiPhotoSelectorView(Context context, ObservableCollection<string> photos) : base(context)
        {
            SetBackgroundColor(Android.Graphics.Color.Black);
            Photos = photos;
            controlsOverlayView = new MultiPhotoSelectioControlsOverlayView(context);
            currentImage = new ImageView(context);

            takenPhotosOverlayView = new CurrentTakenPhotosOverlayView(context);
            takenPhotosOverlayView.ImageTapped += null;

            AddView(currentImage);
            AddView(controlsOverlayView);
            AddView(takenPhotosOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            CloseButtonTouched?.Invoke(this, new EventArgs());
        }
    }
}
