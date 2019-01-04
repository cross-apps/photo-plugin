using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CrossAppsPhotoPlugin.Android.Adapters
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