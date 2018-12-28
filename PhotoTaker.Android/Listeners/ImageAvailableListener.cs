using System;
using Java.IO;
using Java.Lang;
using Java.Nio;
using Android.Media;
using PhotoTaker.Droid.Controls;
using System.Collections.ObjectModel;
using Android.Content;
using Android.App;

namespace PhotoTaker.Droid.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        readonly Context context;
        readonly CameraWidget owner;
        readonly ObservableCollection<File> photos;

        public ImageAvailableListener(CameraWidget cameraWidget, ObservableCollection<File> photos, Context context)
        {
            this.owner = cameraWidget ?? throw new ArgumentNullException("cameraWidget");
            this.photos = photos;
            this.context = context;
        }

        //public File File { get; private set; }
        //public Camera2BasicFragment Owner { get; private set; }

        public void OnImageAvailable(ImageReader reader)
        {
            var file = new File(((Activity)context).GetExternalFilesDir(null), Guid.NewGuid().ToString() + ".jpg");
            owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), file, photos));
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        class ImageSaver : Java.Lang.Object, IRunnable
        {
            // The JPEG image
            File file;
            Image image;
            ObservableCollection<File> photos;

            public ImageSaver(Image image, File file, ObservableCollection<File> photos)
            {
                this.photos = photos;
                this.image = image ?? throw new ArgumentNullException("image");
                this.file = file ?? throw new ArgumentNullException("file");
            }

            public void Run()
            {
                ByteBuffer buffer = image.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);
                using (var output = new FileOutputStream(file))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        image.Close();
                    }

                    photos.Add(file);
                }
            }
        }
    }
}
