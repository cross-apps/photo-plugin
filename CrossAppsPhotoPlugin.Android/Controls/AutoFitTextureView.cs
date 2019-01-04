using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class AutoFitTextureView : TextureView
    {
        int ratioWidth = 0;
        int ratioWeight = 0;

        public AutoFitTextureView(Context context) : this(context, null)
        {

        }

        public AutoFitTextureView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {

        }

        public AutoFitTextureView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

        }

        public void SetAspectRatio(int width, int height)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException("Size cannot be negative.");
            }

            ratioWidth = width;
            ratioWeight = height;
            RequestLayout();
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            base.DispatchTouchEvent(e);
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return true;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);

            if (ratioWidth == 0 || ratioWeight == 0)
            {
                SetMeasuredDimension(width, height);
            }
            else
            {
                if (width < (float)height * ratioWidth / (float)ratioWeight)
                {
                    SetMeasuredDimension(height * ratioWidth / ratioWeight, height);
                }
                else
                {
                    SetMeasuredDimension(width, width * ratioWeight / ratioWidth);
                }
            }
        }
    }
}