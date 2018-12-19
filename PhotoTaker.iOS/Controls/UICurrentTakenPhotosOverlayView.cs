using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UICurrentTakenPhotosOverlayView : UICollectionView, IUICollectionViewDataSource
    {
        NSIndexPath lastTappedIndex = null;

        public List<UIImage> Photos { get; set; } = new List<UIImage>();
        public EventHandler<UIImage> ImageTapped { get; set; }

        public UICurrentTakenPhotosOverlayView(CGRect frame) : base(frame, new UIHorizontalScrollLayout())
        {
            DataSource = this;
            RegisterClassForCell(typeof(UIImageViewCell), UIImageViewCell.CellId);

            UITapGestureRecognizer tapRecognizer = new UITapGestureRecognizer(cellTapped);

            AddGestureRecognizer(tapRecognizer);
            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.Clear;
        }

        public void RemoveLastTappedCell() 
        {
            if (lastTappedIndex != null) 
            {
                var cell = CellForItem(lastTappedIndex) as UIImageViewCell;
                Photos.Remove(cell.ImageView.Image);
                ReloadData();
            }
        } 

        private void cellTapped(UITapGestureRecognizer recognizer) 
        {
            var location = recognizer.LocationInView(this);
            lastTappedIndex = IndexPathForItemAtPoint(location);

            if (lastTappedIndex != null) 
            {
                var cell = CellForItem(lastTappedIndex) as UIImageViewCell;
                ImageTapped?.Invoke(this, cell.ImageView.Image);
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            Frame = rect;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UIImageViewCell cell = (UIImageViewCell)collectionView.DequeueReusableCell(UIImageViewCell.CellId, indexPath);
            cell.ImageView.Image = Photos[indexPath.Row];
            return cell;
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Photos.Count;
        }
    }
}
