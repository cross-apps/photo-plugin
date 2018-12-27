﻿using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Android.Graphics;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.Support.V13.App;
using Android.Support.V4.Content;
using PhotoTaker.Droid.Listeners;
using Java.IO;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using Boolean = Java.Lang.Boolean;
using Math = Java.Lang.Math;
using Orientation = Android.Content.Res.Orientation;
using System.Linq;

namespace PhotoTaker.Droid.Controls
{
    public class CameraWidget : View
    {
        static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
        static readonly string FRAGMENT_DIALOG = "dialog";
        public static readonly int REQUEST_CAMERA_PERMISSION = 1;

        // Tag for the {@link Log}.

        // Camera state: Showing camera preview.
        public const int STATE_PREVIEW = 0;

        // Camera state: Waiting for the focus to be locked.
        public const int STATE_WAITING_LOCK = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        public const int STATE_WAITING_PRECAPTURE = 2;

        //Camera state: Waiting for the exposure state to be something other than precapture.
        public const int STATE_WAITING_NON_PRECAPTURE = 3;

        // Camera state: Picture was taken.
        public const int STATE_PICTURE_TAKEN = 4;

        // Max preview width that is guaranteed by Camera2 API
        static readonly int MAX_PREVIEW_WIDTH = 1920;

        // Max preview height that is guaranteed by Camera2 API
        static readonly int MAX_PREVIEW_HEIGHT = 1080;

        // TextureView.ISurfaceTextureListener handles several lifecycle events on a TextureView
        Camera2BasicSurfaceTextureListener mSurfaceTextureListener;

        // ID of the current {@link CameraDevice}.
        string mCameraId = "0";

        // An AutoFitTextureView for camera preview
        public AutoFitTextureView mTextureView;

        // A {@link CameraCaptureSession } for camera preview.
        public CameraCaptureSession mCaptureSession;

        // A reference to the opened CameraDevice
        public CameraDevice mCameraDevice;

        // The size of the camera preview
        Size mPreviewSize;

        // CameraDevice.StateListener is called when a CameraDevice changes its state
        CameraStateListener mStateCallback;

        // An additional thread for running tasks that shouldn't block the UI.
        HandlerThread mBackgroundThread;

        // A {@link Handler} for running tasks in the background.
        public Handler mBackgroundHandler;

        // An {@link ImageReader} that handles still image capture.
        ImageReader mImageReader;

        // This is the output file for our picture.
        public File mFile;

        // This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
        // still image is ready to be saved.
        ImageAvailableListener mOnImageAvailableListener;

        //{@link CaptureRequest.Builder} for the camera preview
        public CaptureRequest.Builder mPreviewRequestBuilder;

        // {@link CaptureRequest} generated by {@link #mPreviewRequestBuilder}
        public CaptureRequest mPreviewRequest;

        // The current state of camera state for taking pictures.
        public int mState = STATE_PREVIEW;

        // A {@link Semaphore} to prevent the app from exiting before closing the camera.
        public Semaphore mCameraOpenCloseLock = new Semaphore(1);

        // Whether the current camera device supports Flash or not.
        bool mFlashSupported;

        // Orientation of the camera sensor
        int mSensorOrientation;

        // A {@link CameraCaptureSession.CaptureCallback} that handles events related to JPEG capture.
        public CameraCaptureListener mCaptureCallback;

        Context context;

        CaptureRequest.Builder stillCaptureBuilder;

        public CameraWidget(Context Context) : base(Context)
        {
            context = Context;
            mTextureView = new AutoFitTextureView(Context);
            mSurfaceTextureListener = new Camera2BasicSurfaceTextureListener(this);
            mTextureView.SurfaceTextureListener = mSurfaceTextureListener;

            mStateCallback = new CameraStateListener(this);
            StartBackgroundThread();

            mTextureView.SetBackgroundColor(Color.Black);
            // SetBackgroundColor(Color.Green);
            SetBackgroundColor(Color.Black);

            mCaptureCallback = new CameraCaptureListener(this);
        }

        static Size ChooseOptimalSize(Size[] choices, int textureViewWidth, 
                                      int textureViewHeight, int maxWidth, 
                                      int maxHeight, Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                Size option = choices[i];
                if ((option.Width <= maxWidth) && (option.Height <= maxHeight) && option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                        option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Count > 0)
            {
                return (Size)Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (Size)Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                Log.Error("camera view", "Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        // Sets up member variables related to camera.
        void SetUpCameraOutputs(int width, int height)
        {
            var manager = (CameraManager)context.GetSystemService(Context.CameraService);
            try
            {
                var firstId = manager.GetCameraIdList().FirstOrDefault(id => id == mCameraId);
                CameraCharacteristics characteristics = manager.GetCameraCharacteristics(mCameraId);

                // We don't use a front facing camera in this sample.
                /*
                var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                if (facing != null && facing == (Integer.ValueOf((int)LensFacing.Front)))
                {
                    continue;
                }
                */

                var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                if (map == null)
                {
                    return;
                }

                // For still image captures, we use the largest available size.
                Size largest = (Size)Collections.Max(
                    Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                    new CompareSizesByArea());

                mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                // Find out if we need to swap dimension to get the preview 
                // size relative to sensor coordinate.
                var activity = (Activity)context;
                var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
                //noinspection ConstantConditions
                mSensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                bool swappedDimensions = false;
                switch (displayRotation)
                {
                    case SurfaceOrientation.Rotation0:
                    case SurfaceOrientation.Rotation180:
                        if (mSensorOrientation == 90 || mSensorOrientation == 270)
                        {
                            swappedDimensions = true;
                        }
                        break;
                    case SurfaceOrientation.Rotation90:
                    case SurfaceOrientation.Rotation270:
                        if (mSensorOrientation == 0 || mSensorOrientation == 180)
                        {
                            swappedDimensions = true;
                        }
                        break;
                    default:
                        Log.Error("camera view", "Display rotation is invalid: " + displayRotation);
                        break;
                }

                Point displaySize = new Point();
                activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                var rotatedPreviewWidth = width;
                var rotatedPreviewHeight = height;
                var maxPreviewWidth = displaySize.X;
                var maxPreviewHeight = displaySize.Y;

                if (swappedDimensions)
                {
                    rotatedPreviewWidth = height;
                    rotatedPreviewHeight = width;
                    maxPreviewWidth = displaySize.Y;
                    maxPreviewHeight = displaySize.X;
                }

                if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                {
                    maxPreviewWidth = MAX_PREVIEW_WIDTH;
                }

                if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                {
                    maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                }

                // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                // garbage capture data.
                mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(
                    Class.FromType(typeof(SurfaceTexture))),
                    rotatedPreviewWidth, rotatedPreviewHeight, 
                    maxPreviewWidth, maxPreviewHeight, largest);

                // We fit the aspect ratio of TextureView to the size of preview we picked.
                var orientation = Resources.Configuration.Orientation;
                if (orientation == Orientation.Landscape)
                {
                    mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                }
                else
                {
                    mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                }

                // Check if the flash is supported.
                var available = (Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                if (available == null)
                {
                    mFlashSupported = false;
                }
                else
                {
                    mFlashSupported = (bool)available;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException)
            {
                // Currently an NPE is thrown when the Camera2API is used but not supported on the
                // device this code runs.
                // ErrorDialog.NewInstance("Error occured!").Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
        }

        void RequestCameraPermission()
        {
            var activity = (Activity)context;

            if (activity.ShouldShowRequestPermissionRationale(Manifest.Permission.Camera))
            {
                new ConfirmationDialog().Show(activity.FragmentManager, FRAGMENT_DIALOG);
            }
            else
            {
                activity.RequestPermissions(new string[] { Manifest.Permission.Camera }, REQUEST_CAMERA_PERMISSION);
            }
        }

        public void SwitchCamera() 
        {
            int cameraInteger = int.Parse(mCameraId);
            if (cameraInteger == (int)Android.Hardware.CameraFacing.Back) 
            {
                mCameraId = ((int)Android.Hardware.CameraFacing.Front).ToString();
            }
            else
            {
                mCameraId = ((int)Android.Hardware.CameraFacing.Back).ToString();
            }

            CloseCamera();
            ReopenCamera();

            // var manager = (CameraManager)context.GetSystemService(Context.CameraService);
            // CameraInfo.CameraFacingBack
        }

        private void ReopenCamera() 
        {
            if (mTextureView.IsAvailable) 
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
            else
            {

            }
        }

        // Opens the camera specified by {@link Camera2BasicFragment#mCameraId}.
        public void OpenCamera(int width, int height)
        {
            // check permissions...

            var activity = (Activity)context;
            if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestCameraPermission();
                return;
            }

            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);

            var manager = (CameraManager)context.GetSystemService(Context.CameraService);
            try
            {
                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }

                manager.OpenCamera(mCameraId, mStateCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
            catch (Java.Lang.Exception e) 
            {
                e.PrintStackTrace();
            }
        }

        // Closes the current {@link CameraDevice}.
        private void CloseCamera()
        {
            try
            {
                mCameraOpenCloseLock.Acquire();

                if (null != mCaptureSession)
                {
                    mCaptureSession.Close();
                    mCaptureSession = null;
                }

                if (null != mCameraDevice)
                {
                    mCameraDevice.Close();
                    mCameraDevice = null;
                }

                if (null != mImageReader)
                {
                    mImageReader.Close();
                    mImageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }

        // Starts a background thread and its {@link Handler}.
        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        // Creates a new {@link CameraCaptureSession} for camera preview.
        public void CreateCameraPreviewSession()
        {
            try
            {
                SurfaceTexture texture = mTextureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);

                // We set up a CaptureRequest.Builder with the output Surface.
                mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                mPreviewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                List<Surface> surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(mImageReader.Surface);
                mCameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public static T Cast<T>(Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        // Configures the necessary {@link android.graphics.Matrix}
        // transformation to `mTextureView`.
        // This method should be called after the camera preview size is determined in
        // setUpCameraOutputs and also the size of `mTextureView` is fixed.

        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (mTextureView == null || mPreviewSize == null)
            {
                return;
            }

            var activity = (Activity)context;
            var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, mPreviewSize.Height, mPreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();

            if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Math.Max((float)viewHeight / mPreviewSize.Height, (float)viewWidth / mPreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if ((int)SurfaceOrientation.Rotation180 == rotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }

            mTextureView.SetTransform(matrix);
        }

        // Initiate a still image capture.
        public void TakePicture()
        {
            LockFocus();
        }

        // Lock the focus as the first step for a still image capture.
        private void LockFocus()
        {
            try
            {
                // This is how to tell the camera to lock focus.

                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
                // Tell #mCaptureCallback to wait for the lock.
                mState = STATE_WAITING_LOCK;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), 
                                        mCaptureCallback,
                                        mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Run the precapture sequence for capturing a still image. This method should be called when
        // we get a response in {@link #mCaptureCallback} from {@link #lockFocus()}.
        public void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                mState = STATE_WAITING_PRECAPTURE;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Capture a still picture. This method should be called when we get a response in
        // {@link #mCaptureCallback} from both {@link #lockFocus()}.
        public void CaptureStillPicture()
        {
            try
            {
                if (mCameraDevice == null)
                {
                    return;
                }
                // This is the CaptureRequest.Builder that we use to take a picture.
                if (stillCaptureBuilder == null) 
                {
                    stillCaptureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                }
                
                stillCaptureBuilder.AddTarget(mImageReader.Surface);

                // Use the same AE and AF modes as the preview.
                stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                SetAutoFlash(stillCaptureBuilder);

                // Orientation
                var activity = (Activity)context;
                int rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                mCaptureSession.StopRepeating();
                mCaptureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Retrieves the JPEG orientation from the specified screen rotation.
        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }

        // Unlock the focus. This method should be called when still image capture sequence is
        // finished.
        public void UnlockFocus()
        {
            try
            {
                // Reset the auto-focus trigger
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                SetAutoFlash(mPreviewRequestBuilder);
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
                // After this, the camera will go back to the normal state of preview.
                mState = STATE_PREVIEW;
                mCaptureSession.SetRepeatingRequest(mPreviewRequest, mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void SetAutoFlash(CaptureRequest.Builder requestBuilder)
        {
            if (mFlashSupported)
            {
                requestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
            }
        }
    }
}
