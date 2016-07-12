using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Utilities
{
    public class String
    {
        public static void setSecureString(SecureString lhs, System.String rhs)
        {
            int length = rhs.Length;
            lhs.Clear();
            for (int i = 0; i < length; i++)
            {
                lhs.AppendChar(rhs[i]);
            }
        }

        //public static SecureString ConvertToSecureString(string password)
        //{
        //    if (password == null)
        //        throw new ArgumentNullException("password");

        //    unsafe
        //    {
        //        fixed (char* passwordChars = password)
        //        {
        //            var securePassword = new SecureString(passwordChars, password.Length);
        //            securePassword.MakeReadOnly();
        //            return securePassword;
        //        }
        //    }
        //}

        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
