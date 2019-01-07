using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CrossAppsPhotoPlugin.Android.Controls;

namespace CrossAppsPhotoPlugin.Android.Listeners
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraWidget owner;

        public CameraCaptureSessionCallback(CameraWidget owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // owner.ShowToast("Failed");
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            try
            {
                this.owner.mCameraOpenCloseLock.Acquire();

                // The camera is already closed
                if (null == owner.mCameraDevice)
                {
                    return;
                }

                // When the session is ready, we start displaying the preview.
                owner.mCaptureSession = session;
                try
                {
                    // Auto focus should be continuous for camera preview.
                    owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                    //# TODO Torch Mode Android
                    owner.mPreviewRequestBuilder.Set(CaptureRequest.FlashMode, (int)FlashMode.Torch);
                    // Flash is automatically enabled when necessary.
                    owner.SetAutoFlash(owner.mPreviewRequestBuilder);
                    owner.mPreviewRequestBuilder.Set(CaptureRequest.ScalerCropRegion, new Rect(10, 10, 10, 10));

                    // Finally, we start displaying the camera preview.
                    owner.mPreviewRequest = owner.mPreviewRequestBuilder.Build();
                    owner.mCaptureSession.SetRepeatingRequest(owner.mPreviewRequest,
                            owner.mCaptureCallback, owner.mBackgroundHandler);
                }
                catch (CameraAccessException e)
                {
                    e.PrintStackTrace();
                }
            }
            catch (Exception ex)
            {

            }
            finally 
            {
                this.owner.mCameraOpenCloseLock.Release();
            }
        }
    }
}