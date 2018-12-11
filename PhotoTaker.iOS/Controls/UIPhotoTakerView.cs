using System;
using System.Collections.Generic;
using System.IO;
using CoreGraphics;
using PhotoTaker.Custom;
using SkiaSharp.Views.iOS;
using UIKit;

namespace PhotoTaker.iOS.Controls
{
    /// <summary>
    /// Used for holding each UI layer camera, controls, scroll list
    /// and controlling interaction between them.
    /// </summary>
    public class UIPhotoTakerView : UIView
    {
        UICameraPreview cameraPreview;
        UIControlsOverlayView controlsOverlayView;

        // UILatestPhotosOverlayView latestPhotosOverlayView = null;
        UICurrentTakenPhotosOverlayView takenPhotosOverlayView;
        UIPhotoEditorView photoEditorView = null;

        public int MaxImageCount { get; set; } = 6;

        public EventHandler SendButtonTapped { get; set; }

        public UIPhotoTakerView(CameraOptions options)
        {
            cameraPreview = new UICameraPreview(options);
            controlsOverlayView = new UIControlsOverlayView(Frame);
            // latestPhotosOverlayView = new UILatestPhotosOverlayView(this.Frame);
            takenPhotosOverlayView = new UICurrentTakenPhotosOverlayView(Frame);
            photoEditorView = new UIPhotoEditorView(Frame);
            photoEditorView.Hidden = true;

            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;

            AddSubview(cameraPreview);
            AddSubview(controlsOverlayView);
            // AddSubview(latestPhotosOverlayView);
            AddSubview(takenPhotosOverlayView);
            AddSubview(photoEditorView);

            // Register all events...
            controlsOverlayView.TakeButtonTouched += ControlsOverlayView_TakeButtonTouched;
            controlsOverlayView.FlashButtonTouched += ControlsOverlayView_FlashButtonTouched;
            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.CameraButtonTouched += ControlsOverlayView_CameraButtonTouched;
            controlsOverlayView.SendButtonTouched += ControlsOverlayView_SendButtonTouched;

            photoEditorView.CloseButtonTapped += PhotoEditorView_CloseButtonTapped;
            photoEditorView.TrashButtonTapped += PhotoEditorView_TrashButtonTapped;
        }

        void ControlsOverlayView_SendButtonTouched(object sender, EventArgs e)
        {
            SendButtonTapped?.Invoke(this, new EventArgs());
        }

        void PhotoEditorView_TrashButtonTapped(object sender, EventArgs e)
        {
            takenPhotosOverlayView.RemoveLastTappedCell();
            photoEditorView.Hidden = true;
            SetNeedsDisplay();

            cameraPreview.CaptureSession.StartRunning();
            cameraPreview.IsPreviewing = true;
        }

        void PhotoEditorView_CloseButtonTapped(object sender, EventArgs e)
        {
            photoEditorView.Hidden = true;
            SetNeedsDisplay();

            cameraPreview.CaptureSession.StartRunning();
            cameraPreview.IsPreviewing = true;
        }

        void TakenPhotosOverlayView_ImageTapped(object sender, UIImage image)
        {
            if (image != null) 
            {
                photoEditorView.Frame = Frame;
                photoEditorView.Hidden = false;
                photoEditorView.SetImage(image);

                cameraPreview.CaptureSession.StopRunning();
                cameraPreview.IsPreviewing = false;

                SetNeedsDisplay();
            }
        }

        void ControlsOverlayView_CameraButtonTouched(object sender, EventArgs e)
        {
            cameraPreview.CameraButtonTapped();
        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            // 
        }

        void ControlsOverlayView_FlashButtonTouched(object sender, EventArgs e)
        {
            cameraPreview.FlashButtonTapped();
        }

        async void ControlsOverlayView_TakeButtonTouched(object sender, EventArgs e)
        {
            var image = await cameraPreview.TakeButtonTapped();

            takenPhotosOverlayView.Photos.Add(image);
            takenPhotosOverlayView.ReloadData();

            SetNeedsDisplay();
        }

        public List<string> SaveFiles() 
        {
            List<string> fileNames = new List<string>();

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine(documents, "..", "tmp");

            foreach (var image in takenPhotosOverlayView.Photos)
            {
                var fileName = Path.Combine(tmp, Guid.NewGuid().ToString() + ".jpg");
                image.AsJPEG().Save( fileName, true);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        public override void Draw(CGRect rect)
        {
            cameraPreview.Draw(rect);
            controlsOverlayView.Draw(rect);
            takenPhotosOverlayView.Draw(new CGRect(0, rect.Height - 220f, rect.Width, 100f));
            photoEditorView.Draw(rect);
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
