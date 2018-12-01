using System;
using SkiaSharp;

namespace PhotoTaker.iOS.Controls
{
    public class SvgButton
    {
        public bool Touched { get; set; } = true;

        public SkiaSharp.Extended.Svg.SKSvg SvgTouched { get; set; }
        public SkiaSharp.Extended.Svg.SKSvg SvgDefault { get; set; }

        private SKMatrix scale = SKMatrix.MakeScale(0.8f, 0.8f);

        public SvgButton(string DefaultFile, string TouchedFile, SKMatrix Scale)
        {
            SvgTouched = new SkiaSharp.Extended.Svg.SKSvg(190f);
            SvgTouched.Load(TouchedFile);

            SvgDefault = new SkiaSharp.Extended.Svg.SKSvg(190f);
            SvgDefault.Load(DefaultFile);

            scale = Scale;
        }

        public void Draw(SKCanvas canvas, SKPaint paint)
        {
            if (Touched)
            {
                canvas.DrawPicture(SvgTouched.Picture, ref scale, paint);
            }

            canvas.DrawPicture(SvgDefault.Picture, ref scale, paint);
        }
    }
}
