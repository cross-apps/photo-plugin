using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace PhotoTaker.Droid.Adapters
{
    public class ImageViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageView { get; set; }

        public ImageViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            ImageView = (ImageView)itemView;

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}
