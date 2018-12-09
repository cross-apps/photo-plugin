using System;
using System.Linq;
using AVFoundation;
using CoreGraphics;
using Foundation;
using PhotoTaker.Custom;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    public class UICameraPreview: UIView
    {
        AVCaptureVideoPreviewLayer previewLayer;
        CameraOptions cameraOptions;

        public AVCaptureSession CaptureSession { get; private set; }
        public AVCapturePhotoOutput PhotoOutput { get; private set; }
        AVCaptureDeviceInput captureDeviceInput = null;

        public bool IsPreviewing { get; set; }

        public UICameraPreview(CameraOptions options)
        {
            cameraOptions = options;
            IsPreviewing = false;
            SetupLiveStream();
        }

        public override void Draw(CGRect rect)
        {
            previewLayer.Frame = rect;
            base.Draw(rect);
        }

        void SetupLiveStream()
        {
            CaptureSession = new AVCaptureSession();
            PhotoOutput = new AVCapturePhotoOutput();
            previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
            {
                Frame = Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };

            var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
            var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);

            if (device == null)
            {
                return;
            }

            NSError error;
            captureDeviceInput = new AVCaptureDeviceInput(device, out error);
            CaptureSession.AddInput(captureDeviceInput);
            this.Layer.AddSublayer(previewLayer);
            CaptureSession.StartRunning();
            IsPreviewing = true;

            CaptureSession.AddOutput(PhotoOutput);
        }

        public void Capture() {
            
        }
    }
}
