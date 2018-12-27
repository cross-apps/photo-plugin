﻿using System;
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
            Photos = photos;
            SetBackgroundColor(Android.Graphics.Color.Black);
            controlsOverlayView = new MultiPhotoSelectioControlsOverlayView(context);
            currentImage = new ImageView(context);

            takenPhotosOverlayView = new CurrentTakenPhotosOverlayView(context, photos);
            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;;

            AddView(currentImage);
            AddView(controlsOverlayView);
            AddView(takenPhotosOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.TrashButtonTouched += ControlsOverlayView_TrashButtonTouched;
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
