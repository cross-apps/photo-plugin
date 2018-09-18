using System;
using CoreGraphics;
using PhotoTaker.Custom;
using SkiaSharp.Views.iOS;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UIPhotoTakerView : SKCanvasView
    {
        UICameraPreview cameraPreview = null;
        UIControlsOverlayView controlsOverlayView = null;
        UILatestPhotosOverlayView latestPhotosOverlayView = null;

        public UIPhotoTakerView(CameraOptions options)
        {
            cameraPreview = new UICameraPreview(options);
            controlsOverlayView = new UIControlsOverlayView(this.Frame);
            latestPhotosOverlayView = new UILatestPhotosOverlayView(this.Frame);
            //-------------------------//
            this.AddSubview(cameraPreview);
            this.AddSubview(controlsOverlayView);
            this.AddSubview(latestPhotosOverlayView);
        }

        public override void Draw(CGRect rect)
        {
            cameraPreview.Draw(rect);
            controlsOverlayView.Draw(rect);
            latestPhotosOverlayView.Draw(new CGRect(0, rect.Height - 220f, rect.Width, 100f));

            base.Draw(rect);
        }

        public void AddTouchEvents() 
        {
            cameraPreview.Tapped += OnCameraPreviewTapped;
        }

        public void RemoveTouchEvents() 
        {
            cameraPreview.Tapped -= OnCameraPreviewTapped;
        }

        void OnCameraPreviewTapped(object sender, EventArgs e)
        {
            if (cameraPreview.IsPreviewing)
            {
                cameraPreview.CaptureSession.StopRunning();
                cameraPreview.IsPreviewing = false;
            }
            else
            {
                cameraPreview.CaptureSession.StartRunning();
                cameraPreview.IsPreviewing = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) 
            {
                cameraPreview.CaptureSession.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
