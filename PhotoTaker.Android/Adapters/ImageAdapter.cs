using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace PhotoTaker.Droid.Adapters
{
	public class ImageAdapter : BaseAdapter
    {
        Context context;
        ObservableCollection<string> photos;

        public ImageAdapter(Context Context, ObservableCollection<string> Photos)
        {
            context = Context;
            photos = Photos;
        }

        public override int Count => photos.Count;

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
            throw new NotImplementedException();
        }
    }
}
