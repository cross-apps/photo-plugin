using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class MultiPhotoSelectorView : FrameLayout
    {
        ImageView currentImage;
        ObservableCollection<Java.IO.File> Photos;
        CurrentTakenPhotosOverlayView takenPhotosOverlayView;
        MultiPhotoSelectioControlsOverlayView controlsOverlayView;

        public EventHandler CloseButtonTouched { get; set; }
        public EventHandler SendButtonTouched { get; set; }

        public MultiPhotoSelectorView(Context context, ObservableCollection<Java.IO.File> photos) : base(context)
        {
            Photos = photos;
            SetBackgroundColor(Color.Black);
            controlsOverlayView = new MultiPhotoSelectioControlsOverlayView(context);
            currentImage = new ImageView(context);
            currentImage.SetAdjustViewBounds(true);
            currentImage.SetScaleType(ImageView.ScaleType.CenterCrop);

            takenPhotosOverlayView = new CurrentTakenPhotosOverlayView(context, photos);
            takenPhotosOverlayView.ImageTapped += TakenPhotosOverlayView_ImageTapped;

            currentImage.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            AddView(currentImage);
            AddView(controlsOverlayView);
            AddView(takenPhotosOverlayView);

            controlsOverlayView.CloseButtonTouched += ControlsOverlayView_CloseButtonTouched;
         
            controlsOverlayView.TrashButtonTouched += ControlsOverlayView_TrashButtonTouched;
            controlsOverlayView.SendButtonTouched += ControlsOverlayView_SendButtonTouched;
        }

        void ControlsOverlayView_SendButtonTouched(object sender, EventArgs e)
        {
            SendButtonTouched?.Invoke(this, new EventArgs());
        }

        public void SetLayoutParameters()
        {
            takenPhotosOverlayView.LayoutParameters = new FrameLayout.LayoutParams(1080, 200, GravityFlags.Bottom);
            currentImage.LayoutParameters = new FrameLayout.LayoutParams(Width, Height, GravityFlags.Bottom);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        void ControlsOverlayView_TrashButtonTouched(object sender, EventArgs e)
        {
            takenPhotosOverlayView.RemoveLastTappedCell();

            if (Photos.Count == 0)
            {
                CloseButtonTouched?.Invoke(this, new EventArgs());
            }
            else
            {
                takenPhotosOverlayView.SelectLastItem();
            }
        }

        void TakenPhotosOverlayView_ImageTapped(object sender, int position)
        {
            if (currentImage.Drawable != null)
            {
                if (currentImage.Drawable is BitmapDrawable bitmapDrawable)
                {
                    var bitmapImage = bitmapDrawable.Bitmap;

                    if (!bitmapImage.IsRecycled)
                    {
                        currentImage.SetImageBitmap(null);
                        bitmapImage.Recycle();
                        bitmapImage.Dispose();
                        bitmapImage = null;
                    }
                }

                /*
                rotatedBitmap.Recycle();
                rotatedBitmap.Dispose();
                rotatedBitmap = null;
                */
            }

            var bitmap = BitmapFactory.DecodeFile(Photos[position].AbsolutePath);

            var matrix = new Matrix();
            matrix.PostRotate(90);

            var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);

            bitmap.Recycle();
            bitmap.Dispose();
            bitmap = null;

            currentImage.SetImageBitmap(rotatedBitmap);

            /*
            rotatedBitmap.Recycle();
            rotatedBitmap.Dispose();
            rotatedBitmap = null;
            */
        }

        void ControlsOverlayView_CloseButtonTouched(object sender, EventArgs e)
        {
            CloseButtonTouched?.Invoke(this, new EventArgs());
        }
    }
}