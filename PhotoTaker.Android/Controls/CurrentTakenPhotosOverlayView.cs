using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PhotoTaker.Droid.Adapters;

namespace PhotoTaker.Droid.Controls
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
            photos.CollectionChanged += Photos_CollectionChanged;

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

        void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // NumColumns = photos.Count;
        }

        public void RemoveLastTappedCell() 
        {
            if (lastTappedIndex > -1) 
            {
                photos.RemoveAt(lastTappedIndex);
                imageAdapter.NotifyItemRemoved(lastTappedIndex);
            }
        }

        public void SelectLastItem() 
        {
            this.FindViewHolderForAdapterPosition(photos.Count - 1).ItemView.PerformClick();
        }
    }
}
