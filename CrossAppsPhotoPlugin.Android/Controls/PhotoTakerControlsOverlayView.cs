using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Xamarin.Forms;
using graphics = Android.Graphics;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class PhotoTakerControlsOverlayView : SKCanvasView
    {
        SvgButton closeButton;
        SvgButton takeButton;
        SvgButton flashButton;
        SvgButton galleryButton;
        SvgButton sendButton;
        SvgButton counterButton;
        SvgButton cameraButton;

        public int Counter { get; set; } = 0;
        bool timerActive = true;

        List<SvgButton> buttons = new List<SvgButton>();

        #region EventHandlers

        public event EventHandler CloseButtonTouched
        {
            add
            {
                closeButton.Handler += value;
            }
            remove
            {
                closeButton.Handler -= value;
            }
        }

        public event EventHandler TakeButtonTouched
        {
            add
            {
                takeButton.Handler += value;
            }
            remove
            {
                takeButton.Handler -= value;
            }
        }

        public event EventHandler FlashButtonTouched
        {
            add
            {
                flashButton.Handler += value;
            }
            remove
            {
                flashButton.Handler -= value;
            }
        }

        public event EventHandler CameraButtonTouched
        {
            add
            {
                cameraButton.Handler += value;
            }
            remove
            {
                cameraButton.Handler -= value;
            }
        }

        public event EventHandler SendButtonTouched
        {
            add
            {
                sendButton.Handler += value;
            }
            remove
            {
                sendButton.Handler -= value;
            }
        }

        public event EventHandler CounterButtonTouched
        {
            add
            {
                counterButton.Handler += value;
            }
            remove
            {
                counterButton.Handler -= value;
            }
        }

        #endregion

        public PhotoTakerControlsOverlayView(Context Context) : base(Context)
        {
            closeButton = new SvgButton("close_button.svg", "close_button_touched.svg",
                                        SKMatrix.MakeScale(0.8f, 0.8f), Context);

            takeButton = new SvgButton("take_button.svg", "take_button_touched.svg",
                                       SKMatrix.MakeScale(1.5f, 1.5f), Context);

            galleryButton = new SvgButton("gallery_button.svg", "gallery_button.svg",
                                          SKMatrix.MakeScale(2.5f, 2.5f), Context);

            sendButton = new SvgButton("send_button.svg", "send_button_touched.svg",
                                       SKMatrix.MakeScale(2.5f, 2.5f), Context);

            counterButton = new SvgButton("counter_button.svg", "counter_button.svg",
                                          SKMatrix.MakeScale(2.5f, 2.5f), Context);

            flashButton = new SvgButton("flash_button.svg", "flash_button_touched.svg",
                                        "flash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f),
                                        Context);

            cameraButton = new SvgButton("camera_button.svg", "camera_button_front.svg",
                                         "camera_button_front.svg", SKMatrix.MakeScale(2.5f, 2.5f),
                                         Context);

            flashButton.IsToggleButton = true;
            cameraButton.IsToggleButton = true;
            sendButton.IsVisible = false;
            counterButton.IsVisible = true;
            galleryButton.IsVisible = false;

            buttons.AddRange(new[] {
                closeButton, takeButton, galleryButton,
                cameraButton, counterButton, flashButton, sendButton });

            PaintSurface += Handle_PaintSurface;
            SetBackgroundColor(graphics.Color.Transparent);

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
                Invalidate();
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

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                float scale = 1.5f;

                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                /*
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                if (UIScreen.MainScreen.Scale > 2)
                {
                    scale *= 1.5f;
                }
                */

                var x = e.Info.Width / 2 - (takeButton.SvgDefault.Picture.CullRect.Width * scale) / 2;
                var y = e.Info.Height - 2 * takeButton.SvgDefault.Picture.CullRect.Height;
                float xOffset = 0f;

                /*

                if (max > 800)
                {
                    y -= 100f;
                    xOffset = 30f;
                }
                */

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
                    TextSize = 48
                };

                var textBounds = new SKRect();
                paintText.MeasureText(Counter.ToString(), ref textBounds);

                surface.Canvas.ResetMatrix();
                var halfButton = (galleryButton.SvgDefault.Picture.CullRect.Width * 2.5f) / 2;
                var halfHeightButton = (galleryButton.SvgDefault.Picture.CullRect.Height * 2.5f) / 2;

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

                float flashPositionX = e.Info.Width - xOffset - flashButton.SvgTouched.Picture.CullRect.Width;
                float flashPositionY = xOffset + flashButton.SvgTouched.Picture.CullRect.Height;
                flashButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

                canvas.Flush();
            }
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            base.DispatchTouchEvent(e);
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            System.Diagnostics.Debug.WriteLine(e.ToString());

            var point = new SKPoint(e.RawX, e.RawY);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            if (e.Action == MotionEventActions.Down)
            {
                buttons.ForEach((btn) => btn.CheckIntersection(rect));
            }
            else if (e.Action == MotionEventActions.Up)
            {
                buttons.ForEach((btn) => btn.TouchUpInside(rect));
                buttons.ForEach((btn) => btn.Touched = false);
            }

            return true;
        }

    }
}