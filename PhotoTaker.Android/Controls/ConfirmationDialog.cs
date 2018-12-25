using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V13.App;

namespace PhotoTaker.Droid.Controls
{
    public class ConfirmationDialog : DialogFragment
    {
        private static Fragment mParent;
        private class PositiveListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                FragmentCompat.RequestPermissions(mParent,
                                new string[] { Manifest.Permission.Camera }, Camera2BasicFragment.REQUEST_CAMERA_PERMISSION);
            }
        }

        private class NegativeListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                Activity activity = mParent.Activity;
                if (activity != null)
                {
                    activity.Finish();
                }
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            mParent = ParentFragment;
            return new AlertDialog.Builder(Activity)
                .SetMessage("Camera access is required.")
                .SetPositiveButton(Android.Resource.String.Ok, new PositiveListener())
                .SetNegativeButton(Android.Resource.String.Cancel, new NegativeListener())
                .Create();
        }
    }
}
