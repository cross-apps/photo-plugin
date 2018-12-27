using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Xamarin.Forms;

namespace PhotoTaker.Droid.Controls
{
    public class MultiPhotoSelectioControlsOverlayView : SKCanvasView
    {
        readonly SKPaint paint = new SKPaint();

        SvgButton closeButton;
        SvgButton trashButton;
        SvgButton sendButton;

        List<SvgButton> buttons = new List<SvgButton>();

        bool timerActive = true;

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

        public EventHandler TrashButtonTouched { get; set; }
        public EventHandler SendButtonTouched { get; set; }

        public MultiPhotoSelectioControlsOverlayView(Context context) : base(context)
        {
            PaintSurface += Handle_PaintSurface;
            SetBackgroundColor(Android.Graphics.Color.Transparent);

            paint.IsAntialias = true;

            closeButton = new SvgButton("close_button.svg", "close_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f), context);
            trashButton = new SvgButton("trash_button.svg", "trash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f), context);
            sendButton = new SvgButton("send_button.svg", "send_button_touched.svg", SKMatrix.MakeScale(2.5f, 2.5f), context);

            buttons.AddRange(new[] { closeButton, trashButton, sendButton });

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

                float scale = 1.5f;

                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                /*
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                if (UIScreen.MainScreen.Scale > 2)
                {
                    scale *= 1.5f;
                }
                */

                float y = 0;
                float xOffset = 0f;
                /*
                if (max > 800)
                {
                    y -= 100f;
                    xOffset = 30f;
                }
                */

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
