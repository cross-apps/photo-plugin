using System;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UIHorizontalScrollLayout : UICollectionViewFlowLayout
    {
        public UIHorizontalScrollLayout()
        {
            this.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
        }
    }
}
