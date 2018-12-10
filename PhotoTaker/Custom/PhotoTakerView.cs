using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PhotoTaker.Custom
{
    public class PhotoTakerView : View
    {
        public static BindableProperty CameraProperty = BindableProperty.Create(
            propertyName: "Camera",
            returnType: typeof(CameraOptions),
            declaringType: typeof(PhotoTakerView),
            defaultValue: CameraOptions.Rear
        );

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public List<string> FileNames { get; set; } = new List<string>();

        public EventHandler FilesSaved { get; set; }

        public Command SaveFilesCommand { get; set; }
    }
}
