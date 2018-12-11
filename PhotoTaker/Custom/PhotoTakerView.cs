using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PhotoTaker.Custom
{
    public class PhotoTakerView : View
    {
        public static BindableProperty CameraProperty = BindableProperty.Create(
            nameof(Camera), typeof(CameraOptions), typeof(PhotoTakerView), CameraOptions.Rear
        );

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public static BindableProperty MaxImagesProperty = BindableProperty.Create(
            nameof(MaxImageCount), typeof(int), typeof(PhotoTakerView), 6
        );

        public int MaxImageCount 
        {
            get { return (int)GetValue(MaxImagesProperty); }
            set { SetValue(MaxImagesProperty, value); }
        }

        public List<string> FileNames { get; set; } = new List<string>();

        public EventHandler FilesSaved { get; set; }

        public Command SaveFilesCommand { get; set; }
    }
}
