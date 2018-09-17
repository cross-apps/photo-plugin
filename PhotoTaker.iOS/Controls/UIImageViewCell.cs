using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UIImageViewCell : UICollectionViewCell
    {
        public UIImageViewCell(IntPtr handle) : base(handle)
        {
            ImageView = new UIImageView(new RectangleF(0, 0, 150, 150));
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            this.ContentView.AddSubview(ImageView);
            this.ContentView.LayoutMargins = new UIEdgeInsets(10, 10, 10, 10);
        }

        public override void Draw(CGRect rect)
        {
            this.ImageView.Draw(rect);
            base.Draw(rect);
        }

        public string ImageFileName { get; set; }

        public UIImageView ImageView { get; set; }
    }
}
