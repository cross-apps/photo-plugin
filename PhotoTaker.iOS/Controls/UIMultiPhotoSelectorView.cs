using System;
using CoreGraphics;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    /// <summary>
    /// Dsiplay all taken images as thumbnails and allow deletion and send
    /// </summary>
    public class UIMultiPhotoSelectorView : UIView
    {
        UIImageView currentImage = null;
        UICurrentTakenPhotosOverlayView takenPhotosOverlayView;
        UIMultiPhotoSelectorControlsOverlayView controlsOverlayView = null;

        public EventHandler CloseButtonTapped { get; set; }

        public UIMultiPhotoSelectorView(CGRect frame): base(frame)
        {
            controlsOverlayView = new UIMultiPhotoSelectorControlsOverlayView();
            currentImage = new UIImageView(frame);
            takenPhotosOverlayView = new UICurrentTakenPhotosOverlayView(frame);
            takenPhotosOverlayView.Hidden = false;
            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;

            AddSubview(currentImage);
            AddSubview(controlsOverlayView);
            AddSubview(takenPhotosOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.TrashButtonTouched += ControlsOverlayView_TrashButtonTouched;
        }

        void ControlsOverlayView_TrashButtonTouched(object sender, EventArgs e)
        {
            takenPhotosOverlayView.RemoveLastTappedCell();
        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            CloseButtonTapped?.Invoke(this, new EventArgs());
        }

        void TakenPhotosOverlayView_ImageTapped(object sender, UIImage image)
        {
            setImage(image);
        }

        private void setImage(UIImage image) 
        {
            currentImage.Frame = Frame;
            currentImage.ContentMode = UIViewContentMode.ScaleAspectFill;
            SetNeedsDisplay();
            currentImage.Image = image;
            currentImage.ClipsToBounds = true;
        }

        public void AddPhoto(UIImage image) 
        {
            takenPhotosOverlayView.Photos.Add(image);
            takenPhotosOverlayView.ReloadData();

            setImage(image);
        }

        public override void Draw(CGRect rect)
        {
            currentImage.Draw(rect);
            controlsOverlayView.Draw(rect);
            takenPhotosOverlayView.Draw(new CGRect(0, rect.Height - 120f, rect.Width, 100f));
        }
    }
}
