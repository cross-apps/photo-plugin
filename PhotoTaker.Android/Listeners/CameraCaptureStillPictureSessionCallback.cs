using System;
using Android.Hardware.Camera2;
using PhotoTaker.Droid.Controls;
using Android.Util;
using Android.App;
using Java.IO;

namespace PhotoTaker.Droid.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly CameraWidget owner;

        public CameraCaptureStillPictureSessionCallback(CameraWidget owner)
        {
            this.owner = owner ?? throw new ArgumentNullException("owner");
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // If something goes wrong with the save (or the handler isn't even 
            // registered, this code will toast a success message regardless...)
            // owner.mFile = new File(((Activity)owner.Context).GetExternalFilesDir(null), Guid.NewGuid().ToString() + ".jpg");
            owner.UnlockFocus();
        }
    }
}
