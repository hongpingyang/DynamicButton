using DynamicButton.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicButton
{
    public  class HelpClass
    {

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pvd, [In] ref uint pcFonts);

        public enum MouseState
        {
            HOVER,
            DOWN,
            OUT
        }

        public static Color Background => Color.FromArgb(255, 0, 51, 255);

        public static Color BorderColor => Color.FromArgb(255, 124, 125, 133);

        public static Color ForeColor => Color.FromArgb(222, 0, 0, 0);

        public static Color ForeColorwhite => Color.FromArgb(222, 255,255,255);

        public static Font ROBOTO_MEDIUM_20
        {
            get
            {
                return new Font(LoadFont(Resources.Roboto_Medium), 20f);
            }
         }
        public static Font ROBOTO_MEDIUM_12 {
            get
            {
                return new Font(LoadFont(Resources.Roboto_Medium), 12f);
            }
        }
        public static Font ROBOTO_REGULAR_11
        {
            get
            {
                return new Font(LoadFont(Resources.Roboto_Regular), 11f);
            }
        } 

        private static readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private static FontFamily LoadFont(byte[] fontResource)
        {
            int dataLength = fontResource.Length;
            IntPtr fontPtr = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontResource, 0, fontPtr, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)fontResource.Length, IntPtr.Zero, ref cFonts);
            privateFontCollection.AddMemoryFont(fontPtr, dataLength);

            return privateFontCollection.Families.Last();
        }
    }
}
