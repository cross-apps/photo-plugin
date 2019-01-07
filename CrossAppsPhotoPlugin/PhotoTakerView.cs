using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CrossAppsPhotoPlugin
{
    public class PhotoTakerView : View
    {
        /*
        public static BindableProperty CameraProperty = BindableProperty.Create(
            nameof(Camera), typeof(CameraOptions), typeof(PhotoTakerView), CameraOptions.Rear
        );
        */

        public static BindableProperty MaxImageCountProperty = BindableProperty.Create(
            nameof(MaxImageCount), typeof(int), typeof(PhotoTakerView), 0
        );

        public static BindableProperty CameraSwithVisibleProperty = BindableProperty.Create(
            nameof(CameraSwitchVisible), typeof(bool), typeof(PhotoTakerView), true
        );

        /*
        public static BindableProperty TakenImagesThunbmailVisibleProperty = BindableProperty.Create(
            nameof(TakenImagesThumbnailVisible), typeof(bool), typeof(PhotoTakerView), false
        );

        public static BindableProperty SliderVisibleProperty = BindableProperty.Create(
            nameof(SliderVisible), typeof(bool), typeof(PhotoTakerView), false
        );

        public bool SliderVisible
        {
            get { return (bool)GetValue(SliderVisibleProperty); }
            set { SetValue(SliderVisibleProperty, value); }
        }

        public bool TakenImagesThumbnailVisible
        {
            get { return (bool)GetValue(TakenImagesThunbmailVisibleProperty); }
            set { SetValue(TakenImagesThunbmailVisibleProperty, value); }
        }
        */

        public bool CameraSwitchVisible
        {
            get { return (bool)GetValue(CameraSwithVisibleProperty); }
            set { SetValue(CameraSwithVisibleProperty, value); }
        }

        public int MaxImageCount
        {
            get { return (int)GetValue(MaxImageCountProperty); }
            set { SetValue(MaxImageCountProperty, value); }
        }

        /*
        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }
        */

        public List<string> FileNames { get; set; } = new List<string>();

        public EventHandler FilesSaved { get; set; }

        public Command SaveFilesCommand { get; set; }
    }
}
