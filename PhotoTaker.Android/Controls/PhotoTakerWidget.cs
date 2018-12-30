using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Views;
using Android.Widget;

namespace PhotoTaker.Droid.Controls
{
    public class PhotoTakerWidget : FrameLayout
    {
        /// <summary>
        /// Getting camera preview and capture images.
        /// </summary>
        CameraWidget cameraWidget;

        /// <summary>
        /// preview of all captured images in one view,
        /// with delete option.
        /// </summary>
        MultiPhotoSelectorView multiPhotoSelectorView;

        /// <summary>
        /// Controls for default camera, take, switch camera,
        /// focus 
        /// </summary>
        PhotoTakerControlsOverlayView controlsOverlayView;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public ObservableCollection<Java.IO.File> Photos { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            Photos = new ObservableCollection<Java.IO.File>();
            Photos.CollectionChanged += Photos_CollectionChanged;

            controlsOverlayView = new PhotoTakerControlsOverlayView(context);
            controlsOverlayView.TakeButtonTouched += ControlsOverlayView_TakeButtonTouched;
            controlsOverlayView.FlashButtonTouched += ControlsOverlayView_FlashButtonTouched;
            controlsOverlayView.CameraButtonTouched += ControlsOverlayView_CameraButtonTouched;
            controlsOverlayView.CounterButtonTouched += ControlsOverlayView_CounterButtonTouched;

            cameraWidget = new CameraWidget(context, Photos);

            multiPhotoSelectorView = new MultiPhotoSelectorView(context, Photos);
            multiPhotoSelectorView.Visibility = ViewStates.Invisible;
            multiPhotoSelectorView.CloseButtonTouched += MultiPhotoSelectorView_CloseButtonTouched;

            AddView(cameraWidget.mTextureView);
            AddView(controlsOverlayView);
            AddView(multiPhotoSelectorView);
        }

        void MultiPhotoSelectorView_CloseButtonTouched(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Visibility = ViewStates.Invisible;
        }

        void ControlsOverlayView_CounterButtonTouched(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Visibility = ViewStates.Visible;
            multiPhotoSelectorView.SetLayoutParameters();
        }

        void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            controlsOverlayView.Counter = Photos.Count;
        }

        void ControlsOverlayView_CameraButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.SwitchCamera();
        }

        void ControlsOverlayView_FlashButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.IsFlashEnabled = !cameraWidget.IsFlashEnabled;
        }

        void ControlsOverlayView_TakeButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.TakePicture();
            System.Diagnostics.Debug.WriteLine("Take button touched");
        }

        public List<string> SaveFiles()
        {
            List<string> fileNames = new List<string>();

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine(documents, "..", "tmp");

            /*
            foreach (var image in takenPhotosOverlayView.Photos)
            {
                var fileName = Path.Combine(tmp, Guid.NewGuid().ToString() + ".jpg");
                image.AsJPEG().Save(fileName, true);
                fileNames.Add(fileName);
            }
            */

            return fileNames;
        }
    }
}
