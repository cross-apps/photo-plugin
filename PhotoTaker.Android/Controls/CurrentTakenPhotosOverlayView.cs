using System;
using Android.Content;
using Android.Views;

namespace PhotoTaker.Droid.Controls
{
    public class CurrentTakenPhotosOverlayView : View
    {
        public EventHandler ImageTapped { get; set; }

        public CurrentTakenPhotosOverlayView(Context context) : base(context)
        {
        }
    }
}
