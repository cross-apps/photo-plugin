using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;

namespace PhotoTaker.iOS.Controls
{
    /// <summary>
    /// Custom controlls for deleten item from series
    /// </summary>
    internal class UIMultiPhotoSelectorControlsOverlayView : SKCanvasView
    {
        readonly SKPaint paint = new SKPaint();

        SvgButton closeButton = new SvgButton("close_button.svg", "close_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        SvgButton trashButton = new SvgButton("trash_button.svg", "trash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        SvgButton sendButton = new SvgButton("send_button.svg", "send_button_touched.svg", SKMatrix.MakeScale(2.5f, 2.5f));

        List<SvgButton> buttons = new List<SvgButton>();

        bool timerActive = true;

        public EventHandler CloseButtonTouched { get; set; }
        public EventHandler TrashButtonTouched { get; set; }
        public EventHandler SendButtonTouched { get; set; }

        public UIMultiPhotoSelectorControlsOverlayView()
        {
            PaintSurface += Handle_PaintSurface;
            BackgroundColor = UIColor.Clear;

            paint.IsAntialias = true;

            buttons.AddRange(new[] { closeButton, trashButton, sendButton });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
                SetNeedsLayout();
                return timerActive;
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

                float scale = 1.5f;

                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                if (UIScreen.MainScreen.Scale > 2)
                {
                    scale *= 1.5f;
                }

                float y = 0;

                float xOffset = 0f;
                if (max > 800)
                {
                    y -= 100f;
                    xOffset = 30f;
                }

                var x = 0 + 30f + xOffset;

                float closePositionX = x;
                float closePositionY = xOffset + closeButton.SvgTouched.Picture.CullRect.Height;
                closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

                float trashPositionX = e.Info.Width - xOffset - trashButton.SvgTouched.Picture.CullRect.Width;
                float trashPositionY = xOffset + trashButton.SvgTouched.Picture.CullRect.Height;
                trashButton.Draw(surface.Canvas, trashPositionX, trashPositionY, paint);

                float sendPositionX = e.Info.Width - xOffset - 65f - sendButton.SvgTouched.Picture.CullRect.Width * scale;
                float sendPoisitonY = e.Info.Height - 300 - (sendButton.SvgTouched.Picture.CullRect.Height * scale);
                sendButton.Draw(surface.Canvas, sendPositionX, sendPoisitonY, paint);

                canvas.Flush();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)ContentScaleFactor * (float)cgPoint.X, (float)ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            buttons.ForEach((btn) => btn.CheckIntersection(rect));

            sendButton.CheckIntersection(rect);
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
            var point = new SKPoint((float)ContentScaleFactor * (float)cgPoint.X, (float)ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            if (closeButton.TouchUpInside(rect))
            {
                CloseButtonTouched?.Invoke(this, new EventArgs());
            }

            if (trashButton.TouchUpInside(rect))
            {
                TrashButtonTouched?.Invoke(this, new EventArgs());
            }

            buttons.ForEach((btn) => btn.Touched = false);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            buttons.ForEach((btn) => btn.Touched = false);
        }

        public override void Draw(CGRect rect)
        {
            this.Frame = rect;
            base.Draw(rect);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            timerActive = false;
        }
    }
}
