﻿using System;
using Android.Content;
using Android.Util;
using Android.Views;

namespace PhotoTaker.Droid.Controls
{
    public class AutoFitTextureView : TextureView
    {
        private int mRatioWidth = 0;
        private int mRatioHeight = 0;

        public AutoFitTextureView(Context context)
            : this(context, null)
        {

        }
        public AutoFitTextureView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {

        }
        public AutoFitTextureView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }

        public void SetAspectRatio(int width, int height)
        {
            if (width == 0 || height == 0) 
            {
                throw new ArgumentException("Size cannot be negative.");
            }
                
            mRatioWidth = width;
            mRatioHeight = height;
            RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            if (mRatioWidth == 0 || mRatioHeight == 0)
            {
                SetMeasuredDimension(width, height);
            }
            else
            {
                if (width < (float)height * mRatioWidth / (float)mRatioHeight)
                {
                    SetMeasuredDimension(height * mRatioWidth / mRatioHeight, height);
                }
                else
                {
                    SetMeasuredDimension(width, width * mRatioHeight / mRatioWidth);
                }
            }
        }
    }
}
