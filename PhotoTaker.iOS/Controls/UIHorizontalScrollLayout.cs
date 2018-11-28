using System;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UIHorizontalScrollLayout : UICollectionViewFlowLayout
    {
        public UIHorizontalScrollLayout()
        {
            this.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            this.ItemSize = new CoreGraphics.CGSize(90, 140);
        }
    }
}
