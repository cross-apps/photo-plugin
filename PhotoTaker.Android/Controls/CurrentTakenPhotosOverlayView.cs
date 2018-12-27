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
        ObservableCollection<string> photos;

        public EventHandler ImageTapped { get; set; }

        public CurrentTakenPhotosOverlayView(Context context, ObservableCollection<string> Photos) : base(context)
        {
            photos = Photos;
            Adapter = new ImageAdapter(context, Photos);
            ItemClick += CurrentTakenPhotosOverlayView_ItemClick;
        }

        public void RemoveLastTappedCell() 
        {

        }

        void CurrentTakenPhotosOverlayView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
