using System;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;

namespace PhotoTaker.iOS.Controls
{
    public class UIPhotoEditorControlsOverlayView : SKCanvasView
    {
        private SvgButton trashButton = new SvgButton("trash_button.svg", "trash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        private SvgButton closeButton = new SvgButton("close_button.svg", "close_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));

        public EventHandler CloseButtonTouched { get; set; }
        public EventHandler TrashButtonTouched { get; set; }

        public UIPhotoEditorControlsOverlayView()
        {
            PaintSurface += Handle_PaintSurface;
            BackgroundColor = UIColor.Clear;

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
                SetNeedsLayout();
                return true;
            });
        }

        void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (e.Surface != null) 
            {
                var surface = e.Surface;
                var surfaceWidth = e.Info.Width;
                var surfaceHeight = e.Info.Height;
                var canvas = surface.Canvas;

                canvas.Clear(SKColors.Transparent);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                float xOffset = 0f;
                if (max > 800)
                {
                    xOffset = 30f;
                }
                var x = 0 + 30f + xOffset;

                float closePositionX = x;
                float closePositionY = xOffset + closeButton.SvgTouched.Picture.CullRect.Height;
                closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

                float flashPositionX = e.Info.Width - xOffset - trashButton.SvgTouched.Picture.CullRect.Width;
                float flashPositionY = xOffset + trashButton.SvgTouched.Picture.CullRect.Height;
                trashButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

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

            trashButton.CheckIntersection(rect);
            closeButton.CheckIntersection(rect);
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

            // if touch ended within current viewbox!
            if (trashButton.TouchUpInside(rect))
            {
                TrashButtonTouched?.Invoke(this, new EventArgs());
                // fire Event
               // TakeButtonTouched?.Invoke(this, new EventArgs());

                System.Diagnostics.Debug.WriteLine("trash button touched!");
            }

            if (closeButton.TouchUpInside(rect))
            {
                CloseButtonTouched?.Invoke(this, new EventArgs());

                System.Diagnostics.Debug.WriteLine("close button touched!");
            }

            trashButton.Touched = false;
            closeButton.Touched = false;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            trashButton.Touched = false;
            closeButton.Touched = false;
        }

        public override void Draw(CGRect rect)
        {
            this.Frame = rect;
            base.Draw(rect);
        }
    }
}
