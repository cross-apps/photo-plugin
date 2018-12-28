using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Widget;

namespace PhotoTaker.Droid.Controls
{
	public class MultiPhotoSelectorView : FrameLayout
    {
        ImageView currentImage;
        ObservableCollection<Java.IO.File> Photos;
        CurrentTakenPhotosOverlayView takenPhotosOverlayView;
        MultiPhotoSelectioControlsOverlayView controlsOverlayView;

        public EventHandler CloseButtonTouched { get; set; }

        public MultiPhotoSelectorView(Context context, ObservableCollection<Java.IO.File> photos) : base(context)
        {
            Photos = photos;
            SetBackgroundColor(Android.Graphics.Color.Black);
            controlsOverlayView = new MultiPhotoSelectioControlsOverlayView(context);
            currentImage = new ImageView(context);

            takenPhotosOverlayView = new CurrentTakenPhotosOverlayView(context, photos);
            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;
            takenPhotosOverlayView.SetBackgroundColor(Android.Graphics.Color.Green);

            AddView(currentImage);
            AddView(controlsOverlayView);
            AddView(takenPhotosOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.TrashButtonTouched += ControlsOverlayView_TrashButtonTouched;
        }

        public void SetLayoutParameters() 
        {
            takenPhotosOverlayView.LayoutParameters = new FrameLayout.LayoutParams(1080, 200, Android.Views.GravityFlags.Bottom);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            // takenPhotosOverlayView.LayoutParameters = new FrameLayout.LayoutParams(Width, 200, Android.Views.GravityFlags.Bottom);

            /*
            if (Width > 0) 
            {
                var parameters = new FrameLayout.LayoutParams(Width, 200, Android.Views.GravityFlags.Bottom);
                UpdateViewLayout(takenPhotosOverlayView, parameters);
                takenPhotosOverlayView.RequestLayout();
                this.RequestLayout();
            }
            */
        }

        void ControlsOverlayView_TrashButtonTouched(object sender, EventArgs e)
        {
            takenPhotosOverlayView.RemoveLastTappedCell();

            if (Photos.Count == 0) 
            {
                CloseButtonTouched?.Invoke(this, new EventArgs());
            }
            else
            {
                SelectLastItem();
            }
        }

        void TakenPhotosOverlayView_ImageTapped(object sender, EventArgs e)
        {
            // 

        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            CloseButtonTouched?.Invoke(this, new EventArgs());
        }

        public void SelectLastItem() 
        {
            if (Photos.Count > 0) 
            {

            }
        }
    }
}
