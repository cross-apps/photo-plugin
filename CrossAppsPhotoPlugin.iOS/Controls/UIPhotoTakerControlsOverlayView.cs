using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;

namespace CrossAppsPhotoPlugin.iOS.Controls
{
    internal class UIPhotoTakerControlsOverlayView : SKCanvasView 
    {
        SvgButton closeButton = new SvgButton("close_button.svg", "close_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        SvgButton galleryButton = new SvgButton("gallery_button.svg", "gallery_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        SvgButton takeButton = new SvgButton("take_button.svg", "take_button_touched.svg", SKMatrix.MakeScale(1.5f, 1.5f));
        SvgButton sendButton = new SvgButton("send_button.svg", "send_button_touched.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        SvgButton counterButton = new SvgButton("counter_button.svg", "counter_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        SvgButton flashButton = new SvgButton("flash_button.svg", "flash_button_touched.svg", "flash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        SvgButton cameraButton = new SvgButton("camera_button.svg", "camera_button_front.svg", "camera_button_front.svg", SKMatrix.MakeScale(2.5f, 2.5f));

        public EventHandler TakeButtonTouched { get; set; }
        public EventHandler FlashButtonTouched { get; set; }
        public EventHandler CloseButtonTouched { get; set; }
        public EventHandler CameraButtonTouched { get; set; }
        public EventHandler SendButtonTouched { get; set; }
        public EventHandler CounterButtonTouched { get; set; }

        public int Counter { get; set; } = 0;

        private bool timerActive = true;

        List<SvgButton> buttons = new List<SvgButton>();

        public UIPhotoTakerControlsOverlayView(CGRect frame)
        {
            PaintSurface += Handle_PaintSurface;
            BackgroundColor = UIColor.Clear;
            flashButton.IsToggleButton = true;
            cameraButton.IsToggleButton = true;
            sendButton.IsVisible = false;
            counterButton.IsVisible = true;
            galleryButton.IsVisible = false;

            buttons.AddRange(new [] { 
                closeButton, galleryButton, takeButton, sendButton, 
                counterButton,flashButton, cameraButton });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
                SetNeedsLayout();
                return timerActive;
            });
        }

        public void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
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

                float scale = 1.5f;
                float fontScale = 1f;

                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                if (UIScreen.MainScreen.Scale > 2) 
                {
                    fontScale = 1.5f;
                    scale *= 1.5f;
                }

                var x = e.Info.Width / 2 - (takeButton.SvgTouched.Picture.CullRect.Width * scale) / 2;
                var y = e.Info.Height - takeButton.SvgTouched.Picture.CullRect.Height * takeButton.scale.ScaleY - 50f;

                float xOffset = 0f;
                if (max > 800) 
                {
                    y -= 100f;
                    xOffset = 30f;
                }

                surface.Canvas.Translate(x, y);

                takeButton.Draw(surface.Canvas, x, y, paint);


                x = 0 + 30f + xOffset;

                float galleryPositionX = x;
                float galleryPositionY = y + (galleryButton.SvgTouched.Picture.CullRect.Height * scale);
                galleryButton.Draw(surface.Canvas, galleryPositionX, galleryPositionY, paint);

                var paintText = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true,
                    StrokeWidth = 2f,
                    Style = SKPaintStyle.Fill,
                    TextAlign = SKTextAlign.Center,
                    TextSize = 48 * fontScale
                };

                var textBounds = new SKRect();
                paintText.MeasureText(Counter.ToString(), ref textBounds);

                surface.Canvas.ResetMatrix();
                var halfButton = (galleryButton.SvgDefault.Picture.CullRect.Width *  galleryButton.scale.ScaleX) / 2;
                var halfHeightButton = (galleryButton.SvgDefault.Picture.CullRect.Height * galleryButton.scale.ScaleY) / 2;

                var coord = new SKPoint(galleryPositionX + textBounds.Left + halfButton,
                                        galleryPositionY + halfHeightButton + textBounds.Top * -1 - textBounds.Height / 2);
                                        
                canvas.DrawText(Counter.ToString(), coord, paintText);
                counterButton.Draw(surface.Canvas, galleryPositionX, galleryPositionY, paint);

                float cameraPositionX = e.Info.Width - xOffset - 65f - cameraButton.SvgTouched.Picture.CullRect.Width * scale;
                float cameraPoisitonY = y + (cameraButton.SvgTouched.Picture.CullRect.Height * scale);
                cameraButton.Draw(surface.Canvas, cameraPositionX, cameraPoisitonY, paint);

                float sendPositionX = e.Info.Width - xOffset - 65f - sendButton.SvgTouched.Picture.CullRect.Width * scale;
                float sendPoisitonY = y + (cameraButton.SvgTouched.Picture.CullRect.Height * scale);
                sendButton.Draw(surface.Canvas, sendPositionX, sendPoisitonY, paint);

                float flashPositionX = e.Info.Width - xOffset - flashButton.SvgTouched.Picture.CullRect.Width * flashButton.scale.ScaleX;
                float flashPositionY = xOffset + flashButton.SvgTouched.Picture.CullRect.Height;
                flashButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

                // counterButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

                //float closePositionX = x;
                //float closePositionY = xOffset + closeButton.SvgTouched.Picture.CullRect.Height;
                //closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

                // handle the device screen density
                // canvas.Scale(scale);

                canvas.Flush();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)this.ContentScaleFactor * (float)cgPoint.X, (float)this.ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            buttons.ForEach((btn) => btn.CheckIntersection(rect));
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
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            // if touch ended within current viewbox!
            if (takeButton.TouchUpInside(rect)) 
            {
                TakeButtonTouched?.Invoke(this, new EventArgs());
                Counter++;
            }

            if (flashButton.TouchUpInside(rect)) 
            {
                FlashButtonTouched?.Invoke(this, new EventArgs());
            }

            if (cameraButton.TouchUpInside(rect))
            {
                CameraButtonTouched?.Invoke(this, new EventArgs());
            }

            if (closeButton.TouchUpInside(rect)) 
            {
                CloseButtonTouched?.Invoke(this, new EventArgs());
            }

            if (sendButton.TouchUpInside(rect)) 
            {
                SendButtonTouched?.Invoke(this, new EventArgs());
            }

            if (counterButton.TouchUpInside(rect)) 
            {
                CounterButtonTouched?.Invoke(this, new EventArgs());
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

        public void SetSendVisibility(bool isVisible) 
        {

            // sendButton.IsVisible = isVisible;
        }

        public void SetTakeVisibility(bool isVisible) 
        {
            takeButton.IsVisible = isVisible;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            timerActive = false;
        }
    }
}
