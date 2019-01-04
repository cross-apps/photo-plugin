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

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class ErrorDialog : DialogFragment
    {
        private static readonly string ARG_MESSAGE = "message";
        private static Activity mActivity;

        private class PositiveListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                mActivity.Finish();
            }
        }

        public static ErrorDialog NewInstance(string message)
        {
            var args = new Bundle();
            args.PutString(ARG_MESSAGE, message);
            return new ErrorDialog { Arguments = args };
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            mActivity = Activity;
            return new AlertDialog.Builder(mActivity)
                .SetMessage(Arguments.GetString(ARG_MESSAGE))
                .SetPositiveButton("OK", new PositiveListener())
                .Create();
        }
    }
}