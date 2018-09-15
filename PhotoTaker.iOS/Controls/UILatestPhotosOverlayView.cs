using System;
using CoreGraphics;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UILatestPhotosOverlayView : UICollectionView
    {
        public UILatestPhotosOverlayView(CGRect frame) : base(frame, new UICollectionViewFlowLayout())
        {
            this.BackgroundColor = UIColor.Red;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            this.Frame = rect;
        }
    }
}
