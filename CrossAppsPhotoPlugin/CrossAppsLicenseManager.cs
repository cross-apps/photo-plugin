using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CrossAppsPhotoPlugin
{
    public static class CrossAppsLicenseManager
    {
        public static string LicenseKey { get; set; }
        public static string AppName { get; set; }

        public static bool IsValid()
        {
            bool isValid = false;

            byte[] bytes = Encoding.ASCII.GetBytes(AppName);
            byte[] secret = Encoding.ASCII.GetBytes("B73819F82CBBA9D38781EF495B36F");
            var hashString = new HMACSHA256(secret);
            byte[] hash = hashString.ComputeHash(bytes);
            string result = string.Empty;

            foreach (var x in hash)
            {
                result += String.Format("{0:x2}", x);
            }

            if (result.ToLowerInvariant() == LicenseKey.ToLowerInvariant())
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
