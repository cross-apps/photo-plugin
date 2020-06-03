using System;
using System.ComponentModel;
using Foundation;
using CrossAppsPhotoPlugin;
using CrossAppsPhotoPlugin.iOS.Controls;
using CrossAppsPhotoPlugin.iOS.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Reflection;
using System.Linq;

[assembly: ExportRenderer(typeof(PhotoTakerView), typeof(PhotoTakerRenderer))]
namespace CrossAppsPhotoPlugin.iOS.Renderer
{
    public class PhotoTakerRenderer : ViewRenderer<PhotoTakerView, UIPhotoTakerView>
    {
        UIPhotoTakerView photoTakerView;
        PhotoTakerView formsView;

        public PhotoTakerRenderer()
        {
            // placeholder
        }

        public static void Init()
        {
            // thx to https://lachlanwgordon.com/nugeting-a-custom-visual/
            var assembly = Assembly.GetAssembly(typeof(CrossAppsPhotoPlugin.iOS.Renderer.PhotoTakerRenderer));//Get the assembly where the MaterialRenderers live

            var name = "CrossAppsPhotoPlugin.iOS.Renderer";//investigate a type safe way
            var baseRendererTypes = assembly.ExportedTypes.Where(x => x.IsClass && x.Namespace == name && x.Name.Contains("Renderer"));//Get all the Material Renderers

            foreach (var baseRendererType in baseRendererTypes)//Iterate over every material renderer
            {
                var baseRendererElementProperty = baseRendererType.GetRuntimeProperties().FirstOrDefault(x => x.Name == "Element");//Find the type of the XamarinForms View that the render looks after
                Xamarin.Forms.Internals.Registrar.Registered.Register(baseRendererElementProperty.PropertyType, baseRendererType, new[] { typeof(PhotoTakerRenderer) });//Register the renderer. This call is equivalent to the Export statements we usually put in our renderers.
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PhotoTakerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                formsView = e.NewElement;
                photoTakerView = new UIPhotoTakerView(CameraOptions.Front);
                photoTakerView.MaxImageCount = e.NewElement.MaxImageCount;
                photoTakerView.SetCloseVisibility(e.NewElement.CloseButtonVisible);
                // photoTakerView.TakenImagesThumbnailVisible = e.NewElement.TakenImagesThumbnailVisible;

                formsView.SaveFilesCommand = new Command(() =>
                {
                    var files = photoTakerView.SaveFiles();
                    formsView.FileNames.AddRange(files);
                    formsView.FilesSaved?.Invoke(this, new EventArgs());
                });


                photoTakerView.CloseButtonTapped += PhotoTakerView_CloseButtonTapped;
                photoTakerView.SendButtonTapped += PhotoTakerView_SendButtonTapped;
                SetNativeControl(photoTakerView);
            }

            if (e.OldElement != null)
            {
                // photoTakerView.RemoveTouchEvents();
            }

            if (e.NewElement != null)
            {
                // photoTakerView.AddTouchEvents();
            }
        }

        void PhotoTakerView_CloseButtonTapped(object sender, EventArgs e)
        {
            formsView.Closed?.Invoke(this, new EventArgs());
        }


        void PhotoTakerView_SendButtonTapped(object sender, EventArgs e)
        {
            formsView?.SaveFilesCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}