using System;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    /// <summary>
    /// Used for editing an image or deleting it from series.
    /// </summary>
    public class UIPhotoEditorView : UIView
    {
        UIPhotoEditorControlsOverlayView controlsOverlayView = null;
        UIImageView currentImage = null;

        public EventHandler CloseButtonTapped { get; set; }
        public EventHandler TrashButtonTapped { get; set; }

        public UIPhotoEditorView(CGRect frame): base(frame)
        {
            controlsOverlayView = new UIPhotoEditorControlsOverlayView();
            currentImage = new UIImageView(frame);

            AddSubview(currentImage);
            AddSubview(controlsOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.TrashButtonTouched += ControlsOverlayView_TrashButtonTouched;
        }

        void ControlsOverlayView_TrashButtonTouched(object sender, EventArgs e)
        {
            TrashButtonTapped?.Invoke(this, new EventArgs());
        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            CloseButtonTapped?.Invoke(this, new EventArgs());
        }

        public void SetImage(UIImage image) 
        {
            currentImage.Frame = Frame;
            SetNeedsDisplay();
            currentImage.Image = image;
        }

        public override void Draw(CGRect rect)
        {
            currentImage.Draw(rect);
            controlsOverlayView.Draw(rect);
        }
    }
}
