﻿using System;
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

        public event EventHandler<EventArgs> Tapped;

        public AVCaptureSession CaptureSession { get; private set; }

        public bool IsPreviewing { get; set; }

        public UICameraPreview(CameraOptions options)
        {
            cameraOptions = options;
            IsPreviewing = false;
            Initialize();
        }

        public override void Draw(CGRect rect)
        {
            previewLayer.Frame = rect;
            base.Draw(rect);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            OnTapped();
        }

        protected virtual void OnTapped()
        {
            var eventHandler = Tapped;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgs());
            }
        }

        void Initialize()
        {
            CaptureSession = new AVCaptureSession();
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
            var input = new AVCaptureDeviceInput(device, out error);
            CaptureSession.AddInput(input);
            this.Layer.AddSublayer(previewLayer);
            CaptureSession.StartRunning();
            IsPreviewing = true;
        }
    }
}
