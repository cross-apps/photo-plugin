using System;
using SkiaSharp;

namespace PhotoTaker.iOS.Controls
{
    public class SvgButton
    {
        private SKMatrix scale = SKMatrix.MakeScale(0.8f, 0.8f);

        public bool Touched { get; set; } = false;

        public SkiaSharp.Extended.Svg.SKSvg SvgTouched { get; set; }
        public SkiaSharp.Extended.Svg.SKSvg SvgDefault { get; set; }

        public SKRect ViewBox { get; set; } = SKRect.Empty;

        public SvgButton(string DefaultFile, string TouchedFile, SKMatrix Scale)
        {
            SvgTouched = new SkiaSharp.Extended.Svg.SKSvg(190f);
            SvgTouched.Load(TouchedFile);

            SvgDefault = new SkiaSharp.Extended.Svg.SKSvg(190f);
            SvgDefault.Load(DefaultFile);

            scale = Scale;
        }

        public void Draw(SKCanvas canvas, float x, float y, SKPaint paint)
        {
            canvas.ResetMatrix();
            canvas.Translate(x, y);

            if (Touched)
            {
                canvas.DrawPicture(SvgTouched.Picture, ref scale, paint);
            }

            canvas.DrawPicture(SvgDefault.Picture, ref scale, paint);
        }

        public void CheckIntersection(SKRect rect) 
        {
            Touched = this.SvgDefault.ViewBox.IntersectsWithInclusive(rect);
        }

        public bool TouchUpInside(SKRect rect) 
        {
            return Touched && this.SvgDefault.ViewBox.IntersectsWithInclusive(rect);
        }
    }
}
