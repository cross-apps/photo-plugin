using System;
using CoreGraphics;
using Foundation;
using Photos;
using UIKit;

namespace CrossAppsPhotoPlugin.iOS.Controls
{
    internal class UILatestPhotosOverlayView : UICollectionView, IUICollectionViewDataSource
    {
        static NSString imageViewCellId = new NSString("UIImageViewCell");
        PHFetchResult fetchResults = null;
        PHImageManager manager = null;

        public UILatestPhotosOverlayView(CGRect frame) : base(frame, new UIHorizontalScrollLayout())
        {
            DataSource = this;
            RegisterClassForCell(typeof(UIImageViewCell), imageViewCellId);
            fetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            manager = new PHImageManager();

            var tapRecognizer = new UITapGestureRecognizer(() => Console.Write("test"));
            AddGestureRecognizer(tapRecognizer);
            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            this.Frame = rect;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UIImageViewCell cell = (UIImageViewCell)collectionView.DequeueReusableCell(imageViewCellId, indexPath);
            manager.RequestImageForAsset((PHAsset)fetchResults[indexPath.Item], new CGSize(240f, 240f),
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
