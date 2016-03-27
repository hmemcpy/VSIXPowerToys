using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VSIXPowerToys
{
    internal sealed class SystemCursor
    {
        internal enum OCR_SYSTEM_CURSORS : uint
        {
            /// <summary>
            /// Standard arrow
            /// </summary>
            OCR_NORMAL = 32512,
            /// <summary>
            /// I-beam
            /// </summary>
            OCR_IBEAM = 32513,
            /// <summary>
            /// Hourglass
            /// </summary>
            OCR_WAIT = 32514,
            /// <summary>
            /// Crosshair
            /// </summary>
            OCR_CROSS = 32515,
            /// <summary>
            /// Vertical arrow
            /// </summary>
            OCR_UP = 32516,
            OCR_SIZE = 32640,
            OCR_ICON = 32641,
            /// <summary>
            /// Double-pointed arrow pointing northwest and southeast
            /// </summary>
            OCR_SIZENWSE = 32642,
            /// <summary>
            /// Double-pointed arrow pointing northeast and southwest
            /// </summary>
            OCR_SIZENESW = 32643,
            /// <summary>
            /// Double-pointed arrow pointing west and east
            /// </summary>
            OCR_SIZEWE = 32644,
            /// <summary>
            /// Double-pointed arrow pointing north and south
            /// </summary>
            OCR_SIZENS = 32645,
            /// <summary>
            /// Four-pointed arrow pointing north, south, east, and west
            /// </summary>
            OCR_SIZEALL = 32646,
            /// <summary>
            /// Slashed circle
            /// </summary>
            OCR_NO = 32648,
            /// <summary>
            /// Windows 2000/XP: Hand
            /// </summary>
            OCR_HAND = 32649,
            /// <summary>
            /// Standard arrow and small hourglass
            /// </summary>
            OCR_APPSTARTING = 32650,
            /// <summary>
            /// Arrow and question mark
            /// </summary>
            OCR_HELP = 32651
        }

        internal enum GetClassLongIndex
        {
            GCL_CBCLSEXTRA = -20,
            GCL_CBWNDEXTRA = -18,
            GCL_HBRBACKGROUND = -10,
            GCL_HCURSOR = -12,
            GCL_HICON = -14,
            GCL_HMODULE = -16,
            GCL_MENUNAME = -8,
            GCL_STYLE = -26,
            GCL_WNDPROC = -24
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure.
                                        // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                        //    0             The cursor is hidden.
                                        //    0x00000001    The cursor is showing.
            public IntPtr hCursor;      // Handle to the cursor.
            public Point ptScreenPos;   // A POINT structure that receives the screen coordinates of the cursor.
        }

        internal enum FUFlags : uint
        {
            /// <summary>
            /// The default flag; it does nothing. All it means is "not LR_MONOCHROME".
            /// </summary>
            LR_DEFAULTCOLOR = 0x0000,

            /// <summary>
            /// Loads the image in black and white.
            /// </summary>
            LR_MONOCHROME = 0x0001,

            /// <summary>
            /// Returns the original hImage if it satisfies the criteria for the copy—that is, correct dimensions and color depth—in
            /// which case the LR_COPYDELETEORG flag is ignored. If this flag is not specified, a new object is always created.
            /// </summary>
            LR_COPYRETURNORG = 0x0004,

            /// <summary>
            /// Deletes the original image after creating the copy.
            /// </summary>
            LR_COPYDELETEORG = 0x0008,

            /// <summary>
            /// Specifies the image to load. If the hinst parameter is non-NULL and the fuLoad parameter omits LR_LOADFROMFILE,
            /// lpszName specifies the image resource in the hinst module. If the image resource is to be loaded by name,
            /// the lpszName parameter is a pointer to a null-terminated string that contains the name of the image resource.
            /// If the image resource is to be loaded by ordinal, use the MAKEINTRESOURCE macro to convert the image ordinal
            /// into a form that can be passed to the LoadImage function.
            ///  
            /// If the hinst parameter is NULL and the fuLoad parameter omits the LR_LOADFROMFILE value,
            /// the lpszName specifies the OEM image to load. The OEM image identifiers are defined in Winuser.h and have the following prefixes.
            ///
            /// OBM_ OEM bitmaps
            /// OIC_ OEM icons
            /// OCR_ OEM cursors
            ///
            /// To pass these constants to the LoadImage function, use the MAKEINTRESOURCE macro. For example, to load the OCR_NORMAL cursor,
            /// pass MAKEINTRESOURCE(OCR_NORMAL) as the lpszName parameter and NULL as the hinst parameter.
            ///
            /// If the fuLoad parameter includes the LR_LOADFROMFILE value, lpszName is the name of the file that contains the image.
            /// </summary>
            LR_LOADFROMFILE = 0x0010,

            /// <summary>
            /// Retrieves the color value of the first pixel in the image and replaces the corresponding entry in the color table
            /// with the default window color (COLOR_WINDOW). All pixels in the image that use that entry become the default window color.
            /// This value applies only to images that have corresponding color tables.
            /// Do not use this option if you are loading a bitmap with a color depth greater than 8bpp.
            ///
            /// If fuLoad includes both the LR_LOADTRANSPARENT and LR_LOADMAP3DCOLORS values, LRLOADTRANSPARENT takes precedence.
            /// However, the color table entry is replaced with COLOR_3DFACE rather than COLOR_WINDOW.
            /// </summary>
            LR_LOADTRANSPARENT = 0x0020,

            /// <summary>
            /// Uses the width or height specified by the system metric values for cursors or icons,
            /// if the cxDesired or cyDesired values are set to zero. If this flag is not specified and cxDesired and cyDesired are set to zero,
            /// the function uses the actual resource size. If the resource contains multiple images, the function uses the size of the first image.
            /// </summary>
            LR_DEFAULTSIZE = 0x0040,

            /// <summary>
            /// Uses true VGA colors.
            /// </summary>
            LR_VGACOLOR = 0x0080,

            /// <summary>
            /// Searches the color table for the image and replaces the following shades of gray with the corresponding 3-D color: Color Replaced with
            /// Dk Gray, RGB(128,128,128) COLOR_3DSHADOW
            /// Gray, RGB(192,192,192) COLOR_3DFACE
            /// Lt Gray, RGB(223,223,223) COLOR_3DLIGHT
            /// Do not use this option if you are loading a bitmap with a color depth greater than 8bpp.
            /// </summary>
            LR_LOADMAP3DCOLORS = 0x1000,

            /// <summary>
            /// When the uType parameter specifies IMAGE_BITMAP, causes the function to return a DIB section bitmap rather than a compatible bitmap.
            /// This flag is useful for loading a bitmap without mapping it to the colors of the display device.
            /// </summary>
            LR_CREATEDIBSECTION = 0x2000,

            /// <summary>
            /// Tries to reload an icon or cursor resource from the original resource file rather than simply copying the current image.
            /// This is useful for creating a different-sized copy when the resource file contains multiple sizes of the resource.
            /// Without this flag, CopyImage stretches the original image to the new size. If this flag is set, CopyImage uses the size
            /// in the resource file closest to the desired size. This will succeed only if hImage was loaded by LoadIcon or LoadCursor,
            /// or by LoadImage with the LR_SHARED flag.
            /// </summary>
            LR_COPYFROMRESOURCE = 0x4000,

            /// <summary>
            /// Shares the image handle if the image is loaded multiple times. If LR_SHARED is not set, a second call to LoadImage for the
            /// same resource will load the image again and return a different handle.
            /// When you use this flag, the system will destroy the resource when it is no longer needed.
            ///
            /// Do not use LR_SHARED for images that have non-standard sizes, that may change after loading, or that are loaded from a file.
            ///
            /// When loading a system icon or cursor, you must use LR_SHARED or the function will fail to load the resource.
            ///
            /// Windows 95/98/Me: The function finds the first image with the requested resource name in the cache, regardless of the size requested.
            /// </summary>
            LR_SHARED = 0x8000
        }

        internal enum CopyImageType : uint
        {
            /// <summary>
            /// Loads a bitmap.
            /// </summary>
            IMAGE_BITMAP = 0,

            /// <summary>
            /// Loads an icon.
            /// </summary>
            IMAGE_ICON = 1,

            /// <summary>
            /// Loads a cursor.
            /// </summary>
            IMAGE_CURSOR = 2,

            /// <summary>
            /// Loads an enhanced metafile.
            /// </summary>
            IMAGE_ENHMETAFILE = 3
        }

        private static OCR_SYSTEM_CURSORS _previousCursorType;
        private static IntPtr _previousCursor;

        public static void SetSystemCursor(Cursor cursor)
        {
            // Get previous cursor
            var pci = new CURSORINFO();
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(out pci);

            // Get previous cursor type (the one to be replaced)
            _previousCursorType = GetCursorType(pci);

            // Copy previous cursor because next SetSystemCursor might destroy it
            _previousCursor = CopyImage(pci.hCursor,
                                        (uint)CopyImageType.IMAGE_CURSOR,
                                        Cursors.Default.Size.Width,
                                        Cursors.Default.Size.Height,
                                        (uint)FUFlags.LR_COPYFROMRESOURCE);

            // Copy the cursor to set
            IntPtr newCursor = CopyImage(cursor.Handle,
                                         (uint)CopyImageType.IMAGE_CURSOR,
                                         cursor.Size.Width,
                                         cursor.Size.Height,
                                         (uint)FUFlags.LR_COPYFROMRESOURCE);

            // Set the cursor in replacement of the current one
            bool r = SetSystemCursor(newCursor, (uint)_previousCursorType);

            if (!r)
                Console.WriteLine("Error: " + Marshal.GetLastWin32Error());
        }

        public static void RestoreSystemCursor()
        {
            bool r = SetSystemCursor(_previousCursor, (uint)_previousCursorType);

            if (!r)
                Console.WriteLine("Error: " + Marshal.GetLastWin32Error());
        }

        private static OCR_SYSTEM_CURSORS GetCursorType(CURSORINFO pci)
        {
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_APPSTARTING))
                return OCR_SYSTEM_CURSORS.OCR_APPSTARTING;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_CROSS))
                return OCR_SYSTEM_CURSORS.OCR_CROSS;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_HAND))
                return OCR_SYSTEM_CURSORS.OCR_HAND;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_HELP))
                return OCR_SYSTEM_CURSORS.OCR_HELP;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_IBEAM))
                return OCR_SYSTEM_CURSORS.OCR_IBEAM;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_ICON))
                return OCR_SYSTEM_CURSORS.OCR_ICON;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_NO))
                return OCR_SYSTEM_CURSORS.OCR_NO;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_NORMAL))
                return OCR_SYSTEM_CURSORS.OCR_NORMAL;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZE))
                return OCR_SYSTEM_CURSORS.OCR_SIZE;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZEALL))
                return OCR_SYSTEM_CURSORS.OCR_SIZEALL;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZENESW))
                return OCR_SYSTEM_CURSORS.OCR_SIZENESW;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZENS))
                return OCR_SYSTEM_CURSORS.OCR_SIZENS;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZENWSE))
                return OCR_SYSTEM_CURSORS.OCR_SIZENWSE;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_SIZEWE))
                return OCR_SYSTEM_CURSORS.OCR_SIZEWE;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_UP))
                return OCR_SYSTEM_CURSORS.OCR_UP;
            if (pci.hCursor == LoadCursor(IntPtr.Zero, (int)OCR_SYSTEM_CURSORS.OCR_WAIT))
                return OCR_SYSTEM_CURSORS.OCR_WAIT;

            // If the cursor has not been recognized, use the NORMAL/DEFAULT one
            return OCR_SYSTEM_CURSORS.OCR_NORMAL;
        }

        #region Native imports

        [DllImport("user32.dll")]
        static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        static extern IntPtr GetCursor();

        [DllImport("user32.dll")]
        static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        private static IntPtr SetClassLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
                return SetClassLongPtr64(hWnd, nIndex, dwNewLong);

            return new IntPtr(SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        }

        [DllImport("user32.dll", EntryPoint = "SetClassLong", SetLastError = true)]
        static extern uint SetClassLongPtr32(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetClassLongPtr", SetLastError = true)]
        static extern IntPtr SetClassLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong", SetLastError = true)]
        static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr", SetLastError = true)]
        static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "CopyCursor")]
        static extern IntPtr CopyCursor(IntPtr hCursor);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuFlags);

        #endregion
    }
}