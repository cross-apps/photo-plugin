using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CrossAppsPhotoPlugin.Android.Adapters;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class CurrentTakenPhotosOverlayView : RecyclerView
    {
        int lastTappedIndex = -1;
        ObservableCollection<Java.IO.File> photos;
        ImageAdapter imageAdapter;

        public EventHandler<int> ImageTapped { get; set; }

        public CurrentTakenPhotosOverlayView(Context context, ObservableCollection<Java.IO.File> Photos) : base(context)
        {
            photos = Photos;

            var linearLayoutManager = new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false);
            imageAdapter = new ImageAdapter(context, photos);
            imageAdapter.ItemClick += ImageAdapter_ItemClick;

            SetLayoutManager(linearLayoutManager);
            SetAdapter(imageAdapter);
        }

        void ImageAdapter_ItemClick(object sender, int position)
        {
            lastTappedIndex = position;
            ImageTapped?.Invoke(this, position);
        }

        public void RemoveLastTappedCell()
        {
            if (lastTappedIndex > -1)
            {
                try
                {
                    photos.RemoveAt(lastTappedIndex);
                    imageAdapter.NotifyItemRemoved(lastTappedIndex);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void SelectLastItem()
        {
            this.PostDelayed(() =>
            {
                if (FindViewHolderForAdapterPosition(photos.Count - 1) != null) 
                {
                    FindViewHolderForAdapterPosition(photos.Count - 1).ItemView.PerformClick();
                }
            }, 50);
        }
    }
}