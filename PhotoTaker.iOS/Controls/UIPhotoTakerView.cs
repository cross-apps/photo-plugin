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

        public UIPhotoTakerView(CameraOptions options)
        {
            cameraPreview = new UICameraPreview(options);
            controlsOverlayView = new UIControlsOverlayView(this.Frame);
        
            this.AddSubview(cameraPreview);
            this.AddSubview(controlsOverlayView);
        }

        public override void Draw(CGRect rect)
        {
            cameraPreview.Draw(rect);
            controlsOverlayView.Draw(rect);

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
