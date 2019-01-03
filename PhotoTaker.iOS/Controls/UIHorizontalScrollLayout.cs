using System;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    internal class UIHorizontalScrollLayout : UICollectionViewFlowLayout
    {
        public UIHorizontalScrollLayout()
        {
            this.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            this.ItemSize = new CoreGraphics.CGSize(80, 142);
        }
    }
}
