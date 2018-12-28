using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Views;
using Android.Widget;
using PhotoTaker.Droid.Adapters;

namespace PhotoTaker.Droid.Controls
{
    public class CurrentTakenPhotosOverlayView : GridView
    {
        int lastTappedIndex = 0;
        ObservableCollection<Java.IO.File> photos;

        public EventHandler ImageTapped { get; set; }

        public CurrentTakenPhotosOverlayView(Context context, ObservableCollection<Java.IO.File> Photos) : base(context)
        {
            photos = Photos;
            Adapter = new ImageAdapter(context, Photos);
            ItemClick += CurrentTakenPhotosOverlayView_ItemClick;
            photos.CollectionChanged += Photos_CollectionChanged;
        }

        void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NumColumns = photos.Count;
        }

        public void RemoveLastTappedCell() 
        {

        }

        void CurrentTakenPhotosOverlayView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
