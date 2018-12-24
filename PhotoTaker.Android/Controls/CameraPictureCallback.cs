using System;
using Android.Content;
using Android.Hardware;

namespace PhotoTaker.Droid.Controls
{
    public class CameraPictureCallback : Java.Lang.Object, Camera.IPictureCallback
    {
        const string APP_NAME = "testApp";
        Context context;

        public CameraPictureCallback(Context Context)
        {
            context = Context;
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {

        }
    }
}
