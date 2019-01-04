using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CrossAppsPhotoPlugin.Android.Controls;

namespace CrossAppsPhotoPlugin.Android.Listeners
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