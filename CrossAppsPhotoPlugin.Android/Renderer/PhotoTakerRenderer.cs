using System;
using System.Linq;
using System.Reflection;
using Android.Content;
using CrossAppsPhotoPlugin;
using CrossAppsPhotoPlugin.Android.Controls;
using CrossAppsPhotoPlugin.Android.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace CrossAppsPhotoPlugin.Android.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, PhotoTakerWidget>
    {
        PhotoTakerWidget photoTakerWidget;
        PhotoTakerView formsView;

        public PhotoTakerRenderer(Context context) : base(context)
        {
            // placeholder
        }

        public static void Init()
        {
            // thx to https://lachlanwgordon.com/nugeting-a-custom-visual/
            var assembly = Assembly.GetAssembly(typeof(CrossAppsPhotoPlugin.Android.Renderer.PhotoTakerRenderer));//Get the assembly where the MaterialRenderers live

            var name = "CrossAppsPhotoPlugin.Android.Renderer";//investigate a type safe way
            var baseRendererTypes = assembly.ExportedTypes.Where(x => x.IsClass && x.Namespace == name && x.Name.Contains("Renderer"));//Get all the Material Renderers

            foreach (var baseRendererType in baseRendererTypes)//Iterate over every material renderer
            {
                var baseRendererElementProperty = baseRendererType.GetProperty("Element", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);   //Find the type of the XamarinForms View that the render looks after. In Android this is often protected and/or inherited
                if (baseRendererElementProperty != null)
                {
                    Xamarin.Forms.Internals.Registrar.Registered.Register(baseRendererElementProperty.PropertyType, baseRendererType, new[] { typeof(PhotoTakerRenderer) });//Register the renderer. This call is equivalent to the Export statements we usually put in our renderers.
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var name = Context.PackageName;
                
                formsView = e.NewElement;
                formsView.PropertyChanged += FormsView_PropertyChanged;
                photoTakerWidget = new PhotoTakerWidget(Context);
                photoTakerWidget.MaxImageCount = e.NewElement.MaxImageCount;
                photoTakerWidget.CloseButtonTapped += PhotoTakerWidget_CloseButtonTapped;
                photoTakerWidget.SetCloseVisibility(e.NewElement.CloseButtonVisible);
                // photoTakerWidget.TakenImagesThumbnailVisible = e.NewElement.TakenImagesThumbnailVisible;

                formsView.SaveFilesCommand = new Command(() =>
                {
                    var files = photoTakerWidget.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    // formsView.FilesSaved?.Invoke(this, new EventArgs());
                });

                photoTakerWidget.SendButtonTapped += PhotoTagerWidget_SendButtonTapped;
                SetNativeControl(photoTakerWidget);
            }

            if (e.NewElement != e.OldElement) 
            {
                if (e.OldElement != null)
                {
                    e.OldElement.PropertyChanged -= FormsView_PropertyChanged;
                }

                if (e.NewElement != null)
                {
                    e.NewElement.PropertyChanged += FormsView_PropertyChanged;
                }
            }
        }

        void PhotoTakerWidget_CloseButtonTapped(object sender, EventArgs e)
        {
            formsView.Closed?.Invoke(this, new EventArgs());
        }


        void FormsView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                /*
                case nameof(formsView.Camera):
                    photoTakerWidget.SwitchCamera();
                    break;
                    */
                case nameof(formsView.CameraSwitchVisible):
                    photoTakerWidget.SetCameraSwitchVisible(formsView.CameraSwitchVisible);
                    break;
                case nameof(formsView.MaxImageCount):
                    photoTakerWidget.MaxImageCount = formsView.MaxImageCount;
                    break;
            }

            System.Diagnostics.Debug.WriteLine(e.PropertyName);
        }

        void PhotoTagerWidget_SendButtonTapped(object sender, EventArgs e)
        {
            formsView?.SaveFilesCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}