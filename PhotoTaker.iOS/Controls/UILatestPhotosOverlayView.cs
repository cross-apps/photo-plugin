using System;
using CoreGraphics;
using Foundation;
using Photos;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UILatestPhotosOverlayView : UICollectionView, IUICollectionViewDataSource
    {
        static NSString imageViewCellId = new NSString("UIImageViewCell");
        PHFetchResult fetchResults = null;
        PHImageManager manager = null;

        public UILatestPhotosOverlayView(CGRect frame) : base(frame, new UIHorizontalScrollLayout())
        {
            this.BackgroundColor = UIColor.LightGray;
            this.DataSource = this;
            this.RegisterClassForCell(typeof(UIImageViewCell), imageViewCellId);
            this.fetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            this.manager = new PHImageManager();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            this.Frame = rect;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UIImageViewCell cell = (UIImageViewCell)collectionView.DequeueReusableCell(imageViewCellId, indexPath);
            manager.RequestImageForAsset((PHAsset)fetchResults[indexPath.Item], new CGSize(150f, 150f),
            PHImageContentMode.AspectFill, new PHImageRequestOptions(), (img, info) => {
                cell.ImageView.Image = img;
            });

            return cell;
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return fetchResults.Count;
        }
    }
}
