using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class SvgButton
    {
        SKMatrix scale = SKMatrix.MakeScale(0.8f, 0.8f);

        public bool Touched { get; set; } = false;

        public SkiaSharp.Extended.Svg.SKSvg SvgTouched { get; set; }

        public SkiaSharp.Extended.Svg.SKSvg SvgDefault { get; set; }

        public SkiaSharp.Extended.Svg.SKSvg SvgToggled { get; set; }

        public SKRect ViewBox { get; set; } = SKRect.Empty;

        public bool IsToggleButton { get; set; } = false;

        public bool IsToggled { get; set; } = false;

        public bool IsVisible { get; set; } = true;

        public EventHandler Handler { get; set; }

        public SvgButton(string DefaultFile, string ToggledFile, string TouchedFile, SKMatrix Scale, Context Context)
            : this(DefaultFile, TouchedFile, Scale, Context)
        {
            var assets = Context.Assets;
            SvgToggled = new SkiaSharp.Extended.Svg.SKSvg(190f);
            using (var stream = new StreamReader(assets.Open(ToggledFile)))
            {
                SvgToggled.Load(stream.BaseStream);
            }
        }

        public SvgButton(string DefaultFile, string TouchedFile, SKMatrix Scale, Context context)
        {
            var assets = context.Assets;
            SvgTouched = new SkiaSharp.Extended.Svg.SKSvg(190f);
            using (var stream = new StreamReader(assets.Open(TouchedFile)))
            {
                SvgTouched.Load(stream.BaseStream);
            }

            // SvgTouched.Load(TouchedFile);
            SvgDefault = new SkiaSharp.Extended.Svg.SKSvg(190f);

            using (var stream = new StreamReader(assets.Open(DefaultFile)))
            {
                SvgDefault.Load(stream.BaseStream);
            }

            // SvgDefault.Load(DefaultFile);

            scale = Scale;

            /* #TODO
            if (UIScreen.MainScreen.Scale > 2)
            {
                scale = SKMatrix.MakeScale(Scale.ScaleX * 1.5f, Scale.ScaleY * 1.5f);
            }
            else
            {
                scale = Scale;
            }
            */
        }

        public void Draw(SKCanvas canvas, float x, float y, SKPaint paint)
        {
            if (IsVisible)
            {
                canvas.ResetMatrix();
                canvas.Translate(x, y);

                ViewBox = new SKRect(x, y, x + 150f, y + 150f);

                if (IsToggled)
                {
                    canvas.DrawPicture(SvgToggled.Picture, ref scale, paint);
                }
                else if (Touched)
                {
                    canvas.DrawPicture(SvgTouched.Picture, ref scale, paint);
                    canvas.DrawPicture(SvgDefault.Picture, ref scale, paint);
                }
                else
                {
                    canvas.DrawPicture(SvgDefault.Picture, ref scale, paint);
                }
            }
        }

        public void CheckIntersection(SKRect rect)
        {
            Touched = ViewBox.IntersectsWithInclusive(rect);
        }

        public bool TouchUpInside(SKRect rect)
        {
            bool touchUpInsideButton = Touched && ViewBox.IntersectsWithInclusive(rect) && IsVisible;

            if (IsToggleButton && touchUpInsideButton)
            {
                IsToggled = !IsToggled;
            }

            if (touchUpInsideButton)
            {
                Handler?.Invoke(this, new EventArgs());
            }

            return touchUpInsideButton;
        }
    }
}