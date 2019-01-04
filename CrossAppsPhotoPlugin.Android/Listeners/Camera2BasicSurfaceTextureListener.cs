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
using CrossAppsPhotoPlugin.Android.Controls;
using graphics = Android.Graphics;

namespace CrossAppsPhotoPlugin.Android.Listeners
{
    public class Camera2BasicSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly CameraWidget owner;

        public Camera2BasicSurfaceTextureListener(CameraWidget owner)
        {
            this.owner = owner ?? throw new ArgumentNullException("owner");
        }

        public void OnSurfaceTextureAvailable(graphics.SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(graphics.SurfaceTexture surface, int width, int height)
        {
            owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(graphics.SurfaceTexture surface)
        {

        }
    }
}