using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        UIPhotoEditorView photoEditorView;
        UIPhotoTakerControlsOverlayView controlsOverlayView;

        // UILatestPhotosOverlayView latestPhotosOverlayView = null;

        /// <summary>
        /// Is displayed on top of take button as thumbnails.
        /// </summary>
        UICurrentTakenPhotosOverlayView takenPhotosOverlayView;

        /// <summary>
        /// Gets displayed after touching on current count button.
        /// </summary>
        UIMultiPhotoSelectorView multiPhotoSelectorView;

        UISlider slider;

        public int MaxImageCount { get; set; } = 60;

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        /// <summary>
        /// Temp for current taken images.
        /// </summary>
        /// <value>The images.</value>
        public ObservableCollection<UIImage> Photos { get; set; }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            slider.Frame = new CGRect(10, Frame.Height - 140, Frame.Width - 20, 20);
        }

        public UIPhotoTakerView(CameraOptions options)
        {
            Photos = new ObservableCollection<UIImage>();
            Photos.CollectionChanged += Images_CollectionChanged;

            cameraPreview = new UICameraPreview(options);
            controlsOverlayView = new UIPhotoTakerControlsOverlayView(Frame);
            // latestPhotosOverlayView = new UILatestPhotosOverlayView(this.Frame);

            takenPhotosOverlayView = new UICurrentTakenPhotosOverlayView(Frame, Photos);
            takenPhotosOverlayView.Hidden = true;
            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;

            photoEditorView = new UIPhotoEditorView(Frame);
            photoEditorView.Hidden = true;

            multiPhotoSelectorView = new UIMultiPhotoSelectorView(Frame, Photos);
            multiPhotoSelectorView.Hidden = true;

            slider = new UISlider();
            slider.ValueChanged += Slider_ValueChanged;
            slider.MinValue = 1f;
            slider.SendActionForControlEvents(UIControlEvent.TouchDragInside);

            AddSubview(cameraPreview);
            AddSubview(controlsOverlayView);
            AddSubview(slider);

            // - AddSubview(latestPhotosOverlayView);
            AddSubview(takenPhotosOverlayView);
            AddSubview(multiPhotoSelectorView);
            AddSubview(photoEditorView);

            // Register all events...
            controlsOverlayView.TakeButtonTouched += ControlsOverlayView_TakeButtonTouched;
            controlsOverlayView.FlashButtonTouched += ControlsOverlayView_FlashButtonTouched;
            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
            controlsOverlayView.CameraButtonTouched += ControlsOverlayView_CameraButtonTouched;
            controlsOverlayView.SendButtonTouched += ControlsOverlayView_SendButtonTouched;
            controlsOverlayView.CounterButtonTouched += ControlsOverlayView_CounterButtonTouched;

            photoEditorView.CloseButtonTapped += PhotoEditorView_CloseButtonTapped;
            photoEditorView.TrashButtonTapped += PhotoEditorView_TrashButtonTapped;

            multiPhotoSelectorView.CloseButtonTapped += MultiPhotoSelectorView_CloseButtonTapped;

            var panGestureRecognizer = new UIPinchGestureRecognizer((gesture) =>
            {
                if (gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed)
                {
                    var result = cameraPreview.SetZoom((float)gesture.Scale);
                    slider.ValueChanged -= Slider_ValueChanged;
                    slider.Value = result;
                    slider.ValueChanged += Slider_ValueChanged;
                }
                gesture.Scale = 1.0f;
            });

            slider.MaxValue = cameraPreview.GetMaxZoomFactor();

            UserInteractionEnabled = true;

            AddGestureRecognizer(panGestureRecognizer);
        }

        void Slider_ValueChanged(object sender, EventArgs e)
        {
            cameraPreview.SetZoomAbsolute(slider.Value);
        }

        void Images_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            controlsOverlayView.Counter = Photos.Count;
        }

        void MultiPhotoSelectorView_CloseButtonTapped(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Hidden = true;
            SetNeedsDisplay();

            cameraPreview.CaptureSession.StartRunning();
            cameraPreview.IsPreviewing = true;
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

            controlsOverlayView.SetSendVisibility(takenPhotosOverlayView.Photos.Count > 0);
            controlsOverlayView.SetTakeVisibility(takenPhotosOverlayView.Photos.Count < MaxImageCount);

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

        void ControlsOverlayView_CounterButtonTouched(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Frame = Frame;
            multiPhotoSelectorView.Hidden = false;
            multiPhotoSelectorView.SetNeedsDisplay();
            multiPhotoSelectorView.SelectLastItem();

            cameraPreview.CaptureSession.StopRunning();
            cameraPreview.IsPreviewing = false;

            SetNeedsDisplay();
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

            Photos.Add(image);

            takenPhotosOverlayView.Hidden = !TakenImagesThumbnailVisible;
            takenPhotosOverlayView.ReloadData();
            multiPhotoSelectorView.ReloadData();

            SetNeedsDisplay();

            controlsOverlayView.SetSendVisibility(takenPhotosOverlayView.Photos.Count > 0);
            controlsOverlayView.SetTakeVisibility(takenPhotosOverlayView.Photos.Count < MaxImageCount);
        }

        public List<string> SaveFiles() 
        {
            List<string> fileNames = new List<string>();

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine(documents, "..", "tmp");

            foreach (var image in takenPhotosOverlayView.Photos)
            {
                var fileName = Path.Combine(tmp, Guid.NewGuid().ToString() + ".jpg");
                image.AsJPEG().Save(fileName, true);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        public override void Draw(CGRect rect)
        {
            cameraPreview.Draw(rect);
            controlsOverlayView.Draw(rect);
            // slider.Draw(new CGRect(0, rect.Height - 220f, rect.Width, 100f));
            takenPhotosOverlayView.Draw(new CGRect(0, rect.Height - 220f, rect.Width, 100f));
            photoEditorView.Draw(rect);
            multiPhotoSelectorView.Draw(rect);
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
