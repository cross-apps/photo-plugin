using System;
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
    }
}
