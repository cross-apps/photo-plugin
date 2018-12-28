using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace PhotoTaker.Droid.Adapters
{
	public class ImageAdapter : RecyclerView.Adapter
    {
        Context context;
        ObservableCollection<Java.IO.File> photos;

        public ImageAdapter(Context Context, ObservableCollection<Java.IO.File> Photos)
        {
            context = Context;
            photos = Photos;
        }

        public override int Count => photos.Count;

        public override int ItemCount => throw new NotImplementedException();

        public override Java.Lang.Object GetItem(int position)
        {
            return photos[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var imageView = new ImageView(context);

            var options = new BitmapFactory.Options();
            options.InSampleSize = 100;
            var bitmap = BitmapFactory.DecodeFile(photos[position].AbsolutePath, options);
            imageView.SetImageBitmap(bitmap);
            imageView.LayoutParameters = new ViewGroup.LayoutParams(100, 100);

            return imageView;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            throw new NotImplementedException();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            throw new NotImplementedException();
        }
    }
}
