using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        AVCaptureDeviceInput captureDeviceInput = null;
        AVCaptureStillImageOutput captureStillImageOutput = null;

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
            // PhotoOutput = new AVCapturePhotoOutput();
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

            captureDeviceInput = new AVCaptureDeviceInput(device, out NSError error);
            CaptureSession.AddInput(captureDeviceInput);
            this.Layer.AddSublayer(previewLayer);

            captureStillImageOutput = new AVCaptureStillImageOutput()
            {
                OutputSettings = new NSDictionary()
            };

            CaptureSession.AddOutput(captureStillImageOutput);
            CaptureSession.StartRunning();
            IsPreviewing = true;
        }

        public async Task<UIImage> TakeButtonTapped() 
        {
            var videoConnection = captureStillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
            var sampleBuffer = await captureStillImageOutput.CaptureStillImageTaskAsync(videoConnection);

            var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
            var jpegAsByteArray = jpegImageAsNsData.ToArray();

            return new UIImage(jpegImageAsNsData);
        }

        public void FlashButtonTapped()
        {
            var device = captureDeviceInput.Device;

            var error = new NSError();
            if (device.HasFlash)
            {
                if (device.FlashMode == AVCaptureFlashMode.On)
                {
                    device.LockForConfiguration(out error);
                    device.FlashMode = AVCaptureFlashMode.Off;
                    device.UnlockForConfiguration();
                }
                else
                {
                    device.LockForConfiguration(out error);
                    device.FlashMode = AVCaptureFlashMode.On;
                    device.UnlockForConfiguration();
                }
            }
        }

        public void CameraButtonTapped()
        {
            var devicePosition = captureDeviceInput.Device.Position;
            if (devicePosition == AVCaptureDevicePosition.Front)
            {
                devicePosition = AVCaptureDevicePosition.Back;
            }
            else
            {
                devicePosition = AVCaptureDevicePosition.Front;
            }

            var device = GetCameraForOrientation(devicePosition);
            // ConfigureCameraForDevice(device);

            CaptureSession.BeginConfiguration();
            CaptureSession.RemoveInput(captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(device);
            CaptureSession.AddInput(captureDeviceInput);
            CaptureSession.CommitConfiguration();
        }

        public AVCaptureDevice GetCameraForOrientation(AVCaptureDevicePosition orientation)
        {
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            foreach (var device in devices)
            {
                if (device.Position == orientation)
                {
                    return device;
                }
            }

            return null;
        }
    }
}
