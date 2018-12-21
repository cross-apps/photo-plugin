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
        UIMultiPhotoSelectorControlsOverlayView controlsOverlayView = null;
        UIImageView currentImage = null;

        public UIMultiPhotoSelectorView(CGRect frame): base(frame)
        {
            controlsOverlayView = new UIMultiPhotoSelectorControlsOverlayView();
            currentImage = new UIImageView(frame);

            AddSubview(currentImage);
            AddSubview(controlsOverlayView);
        }


    }
}
