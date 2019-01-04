using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CrossAppsPhotoPlugin.Android.Adapters
{
    public class ImageAdapter : RecyclerView.Adapter
    {
        Context context;
        ObservableCollection<Java.IO.File> photos;
        public EventHandler<int> ItemClick;

        public ImageAdapter(Context Context, ObservableCollection<Java.IO.File> Photos)
        {
            context = Context;
            photos = Photos;
        }

        public override int ItemCount => photos.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var options = new BitmapFactory.Options();
            options.InSampleSize = 4;

            var imageViewHoder = holder as ImageViewHolder;

            if (imageViewHoder.ImageView.Drawable != null && imageViewHoder.ImageView.Drawable is BitmapDrawable bitmapDrawable)
            {
                var bitmapImage = bitmapDrawable.Bitmap;
                if (!bitmapImage.IsRecycled)
                {
                    bitmapImage.Recycle();
                    bitmapImage.Dispose();
                    bitmapImage = null;
                }

                imageViewHoder.ImageView.SetImageBitmap(null);
            }

            var bitmap = BitmapFactory.DecodeFile(photos[position].AbsolutePath, options);
            imageViewHoder.ImageView.SetImageBitmap(bitmap);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var imageView = new ImageView(context);
            imageView.Rotation = 90f;
            imageView.LayoutParameters = new ViewGroup.LayoutParams(200, 200);
            var viewHolder = new ImageViewHolder(imageView, OnClick);
            return viewHolder;
        }
    }
}