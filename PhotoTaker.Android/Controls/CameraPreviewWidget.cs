using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;

namespace PhotoTaker.Droid.Controls
{
    public class CameraPreviewWidget : SurfaceView, ISurfaceHolderCallback
    {
        Android.Hardware.Camera camera;
        static bool stopped;

        public CameraPreviewWidget(Android.Content.Context context, Android.Hardware.Camera Camera) : base(context)
        {
            camera = Camera;
            camera.SetDisplayOrientation(90);

            Holder.AddCallback(this);
            Holder.SetType(SurfaceType.PushBuffers);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            if (Holder.Surface == null) 
            {
                return;
            }

            try
            {
                camera.StopPreview();
            }
            catch (Exception)
            {
                // 
            }

            try
            {
                camera.SetPreviewDisplay(Holder);
                camera.StartPreview();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                camera.SetPreviewDisplay(holder);
                camera.StartPreview();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //You could handle release of camera and holder here, but I did it already in the CameraFragment.
        }
    }
}
