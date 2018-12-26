using System;
using Android.App;
using Android.Hardware.Camera2;
using PhotoTaker.Droid.Controls;

namespace PhotoTaker.Droid.Listeners
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        readonly CameraWidget owner;

        public CameraStateListener(CameraWidget owner)
        {
            this.owner = owner ?? throw new ArgumentNullException("owner");
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.mCameraOpenCloseLock.Release();
            owner.mCameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
            if (owner == null) 
            {
                return;
            }

            /*
            Activity activity = owner.Activity;
            if (activity != null)
            {
                activity.Finish();
            }
            */
        }
    }
}
