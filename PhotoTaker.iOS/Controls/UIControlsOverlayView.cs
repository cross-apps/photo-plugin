using System;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;

namespace PhotoTaker.iOS.Controls
{
    public class UIControlsOverlayView : SKCanvasView 
    {
        SKRect viewBox = SKRect.Empty;

        private bool takeButtonTouched = false;

        public void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (e.Surface != null)
            {
                var surface = e.Surface;
                var surfaceWidth = e.Info.Width;
                var surfaceHeight = e.Info.Height;

                var canvas = surface.Canvas;
                canvas.Clear(SkiaSharp.SKColors.Transparent);

                var svgButtonTouched = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgButtonTouched.Load("button_touched.svg");

                var svgTakeButton = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgTakeButton.Load("photo_button.svg");

                var svgGalleryButton = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgGalleryButton.Load("button_gallery.svg");

                var svgCameraButton = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgCameraButton.Load("camera_button.svg");

                // get the size of the canvas
                float canvasMin = Math.Min(e.Info.Width, e.Info.Height);

                // get the size of the picture
                float svgMax = Math.Max(svgTakeButton.Picture.CullRect.Width, svgTakeButton.Picture.CullRect.Height);

                // get the scale to fill the screen
                float scale = 1.5f;//  canvasMin / svgMax;

                // create a scale matrix
                var matrix = SKMatrix.MakeScale(scale, scale);

                var x = e.Info.Width / 2 - (svgTakeButton.Picture.CullRect.Width * scale) / 2;
                var y = e.Info.Height - 2 * svgTakeButton.Picture.CullRect.Height;
                surface.Canvas.Translate(x, y);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                if (takeButtonTouched) 
                {
                    surface.Canvas.DrawPicture(svgButtonTouched.Picture, ref matrix, paint);
                }

                surface.Canvas.DrawPicture(svgTakeButton.Picture, ref matrix, paint);

                viewBox = new SKRect(x, y, x + 100f, y + 100f);

                x = 0 + 30f;

                var matrix2 = SKMatrix.MakeScale(2.5f, 2.5f);

                surface.Canvas.ResetMatrix();
                surface.Canvas.Translate(x, y + (svgGalleryButton.Picture.CullRect.Height * scale));
                surface.Canvas.DrawPicture(svgGalleryButton.Picture, ref matrix2, paint);

                surface.Canvas.ResetMatrix();
                surface.Canvas.Translate(e.Info.Width - 65f - svgCameraButton.Picture.CullRect.Width * scale, y + (svgCameraButton.Picture.CullRect.Height * scale));
                surface.Canvas.DrawPicture(svgCameraButton.Picture, ref matrix2, paint);

                // draw on the canvas
                canvas.Flush();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)this.ContentScaleFactor * (float)cgPoint.X, (float)this.ContentScaleFactor * (float)cgPoint.Y);
            takeButtonTouched = viewBox.IntersectsWithInclusive(new SKRect(point.X, point.Y, point.X + 2f, point.Y + 2f));
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            takeButtonTouched = false;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            takeButtonTouched = false;
        }

        public UIControlsOverlayView(CGRect frame)// : base(new CGRect(0,0, 100, 100))
        {
            this.PaintSurface += Handle_PaintSurface;
            this.BackgroundColor = UIColor.Clear;

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
             {
                // this.InvalidateSurface();
                this.SetNeedsLayout();
                return true;
             });

        }

        
        public override void Draw(CGRect rect)
        {
            this.Frame = rect;
            base.Draw(rect);
        }
    }
}
