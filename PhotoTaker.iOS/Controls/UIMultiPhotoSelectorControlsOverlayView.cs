using System;
using SkiaSharp;
using SkiaSharp.Views.iOS;

namespace PhotoTaker.iOS.Controls
{
    /// <summary>
    /// Custom controlls for deleten item from series
    /// </summary>
    public class UIMultiPhotoSelectorControlsOverlayView : SKCanvasView
    {
        public UIMultiPhotoSelectorControlsOverlayView()
        {
            PaintSurface += Handle_PaintSurface;
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



                canvas.Flush();
            }
        }
    }
}
