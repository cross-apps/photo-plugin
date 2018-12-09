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

        private SvgButton flashButton = new SvgButton("flash_button.svg", "flash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        private SvgButton closeButton = new SvgButton("close_button.svg", "close_button.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        private SvgButton cameraButton = new SvgButton("camera_button.svg", "camera_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        private SvgButton galleryButton = new SvgButton("gallery_button.svg", "gallery_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));

        public EventHandler TakeButtonTouched { get; set; }

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
                svgButtonTouched.Load("take_button_touched.svg");

                var svgTakeButton = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgTakeButton.Load("take_button.svg");

                // get the size of the canvas
                float canvasMin = Math.Min(e.Info.Width, e.Info.Height);

                // get the size of the picture
                float svgMax = Math.Max(svgTakeButton.Picture.CullRect.Width, svgTakeButton.Picture.CullRect.Height);

                // get the scale to fill the screen
                float scale = 1.5f;//  canvasMin / svgMax;

                // create a scale matrix
                var matrix = SKMatrix.MakeScale(scale, scale);
                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                var x = e.Info.Width / 2 - (svgTakeButton.Picture.CullRect.Width * scale) / 2;
                var y = e.Info.Height - 2 * svgTakeButton.Picture.CullRect.Height;

                float xOffset = 0f;
                if (max > 800) 
                {
                    y -= 100f;
                    xOffset = 30f;
                }

                surface.Canvas.Translate(x, y);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                if (takeButtonTouched) 
                {
                    surface.Canvas.DrawPicture(svgButtonTouched.Picture, ref matrix, paint);
                }

                surface.Canvas.DrawPicture(svgTakeButton.Picture, ref matrix, paint);

                viewBox = new SKRect(x, y, x + 100f, y + 100f);

                x = 0 + 30f + xOffset;

                float galleryPositionX = x;
                float galleryPositionY = y + (galleryButton.SvgTouched.Picture.CullRect.Height * scale);
                galleryButton.Draw(surface.Canvas, galleryPositionX, galleryPositionY, paint);

                float cameraPositionX = e.Info.Width - xOffset - 65f - cameraButton.SvgTouched.Picture.CullRect.Width * scale;
                float cameraPoisitonY = y + (cameraButton.SvgTouched.Picture.CullRect.Height * scale);
                cameraButton.Draw(surface.Canvas, cameraPositionX, cameraPoisitonY, paint);

                float flashPositionX = e.Info.Width - xOffset - flashButton.SvgTouched.Picture.CullRect.Width;
                float flashPositionY = xOffset + flashButton.SvgTouched.Picture.CullRect.Height;
                flashButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

                float closePositionX = x;
                float closePositionY = xOffset + closeButton.SvgTouched.Picture.CullRect.Height;
                closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

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

            var rect = new SKRect(point.X, point.Y, point.X + 2f, point.Y + 2f);

            cameraButton.CheckIntersection(rect);
            flashButton.CheckIntersection(rect);

            takeButtonTouched = viewBox.IntersectsWithInclusive(new SKRect(point.X, point.Y, point.X + 2f, point.Y + 2f));
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)this.ContentScaleFactor * (float)cgPoint.X, (float)this.ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X, point.Y, point.X + 2f, point.Y + 2f);

            var touchEnded = viewBox.IntersectsWithInclusive(rect);

            // if touch ended within current viewbox!
            if (touchEnded) 
            {
                System.Diagnostics.Debug.WriteLine("touched Take button!");
            }

            if (flashButton.TouchUpInside(rect)) 
            {
                System.Diagnostics.Debug.WriteLine("Flash button touched!");
            }

            takeButtonTouched = false;
            cameraButton.Touched = false;
            flashButton.Touched = false;
            galleryButton.Touched = false;
        }


        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            takeButtonTouched = false;
            cameraButton.Touched = false;
            flashButton.Touched = false;
        }


        public UIControlsOverlayView(CGRect frame)// : base(new CGRect(0,0, 100, 100))
        {
            this.PaintSurface += Handle_PaintSurface;
            this.BackgroundColor = UIColor.Clear;

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
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
