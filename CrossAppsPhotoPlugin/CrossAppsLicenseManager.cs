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

            byte[] bytes = Encoding.UTF8.GetBytes(AppName.ToLowerInvariant());
            byte[] key = Encoding.UTF8.GetBytes("B73819F82CBBA9D38781EF495B36F".ToLowerInvariant());
            var hashString = new HMACSHA256(key);
            byte[] hash = hashString.ComputeHash(bytes);

            var result = Convert.ToBase64String(hash);

            if (result.ToLowerInvariant() == LicenseKey.ToLowerInvariant())
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
