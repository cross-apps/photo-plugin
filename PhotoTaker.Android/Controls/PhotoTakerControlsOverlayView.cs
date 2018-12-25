using System;
using Android.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Xamarin.Forms;

namespace PhotoTaker.Droid.Controls
{
	public class PhotoTakerControlsOverlayView : SKCanvasView
    {
        SvgButton closeButton;

        private bool timerActive = true;

        public PhotoTakerControlsOverlayView(Android.Content.Context Context) : base(Context)
        {
            closeButton = new SvgButton("close_button.svg", 
                                        "close_button_touched.svg",
                                        SKMatrix.MakeScale(0.8f, 0.8f),
                                        Context);

            PaintSurface += Handle_PaintSurface;
            SetBackgroundColor(Android.Graphics.Color.Transparent);

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

                canvas.Clear(SKColors.Green);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                float closePositionX = 100;
                float closePositionY = 100;
                closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

                canvas.Flush();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        public override void Layout(int l, int t, int r, int b)
        {
            base.Layout(l, t, r, b);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
        }
    }
}
