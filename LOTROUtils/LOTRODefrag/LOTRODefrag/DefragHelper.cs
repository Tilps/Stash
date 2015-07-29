using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace LOTRODefrag
{
    public class DefragHelper
    {

        //
        // CreateFile constants
        //
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint FILE_SHARE_DELETE = 0x00000004;
        const uint OPEN_EXISTING = 3;

        const uint GENERIC_READ = (0x80000000);
        const uint GENERIC_WRITE = (0x40000000);

        const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        const uint FILE_READ_ATTRIBUTES = (0x0080);
        const uint FILE_WRITE_ATTRIBUTES = 0x0100;
        const uint ERROR_INSUFFICIENT_BUFFER = 122;

        [DllImport("kernel32.dll", SetLastError = true, CharSet=CharSet.Unicode)]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        private static IntPtr OpenVolume(string deviceName)
        {
            IntPtr hDevice;
            hDevice = CreateFile(
                @"\\.\" + deviceName,
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_WRITE | FILE_SHARE_READ,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            if ((int)hDevice == -1)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            return hDevice;
        }

        private static IntPtr OpenFile(string path)
        {
            IntPtr hFile;
            hFile = CreateFile(
                        path,
                        FILE_READ_ATTRIBUTES | FILE_WRITE_ATTRIBUTES,
                        FILE_SHARE_READ | FILE_SHARE_WRITE,
                        IntPtr.Zero,
                        OPEN_EXISTING,
                        0,
                        IntPtr.Zero);
            if ((int)hFile == -1)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            return hFile;
        }


        /// <summary>
        /// Get cluster usage for a device
        /// </summary>
        /// <param name="deviceName">use "c:"</param>
        /// <returns>a bitarray for each cluster</returns>
        public static BitArray GetVolumeMap(string deviceName)
        {
            IntPtr pAlloc = IntPtr.Zero;
            IntPtr hDevice = IntPtr.Zero;

            try
            {
                hDevice = OpenVolume(deviceName);

                Int64 i64 = 0;

                GCHandle handle = GCHandle.Alloc(i64, GCHandleType.Pinned);
                IntPtr p = handle.AddrOfPinnedObject();

                // 64 megs == 67108864 bytes == 536870912 bits == cluster count
                // NTFS 4k clusters == 2147483648 k of storage == 2097152 megs == 2048 gig disk storage
                // This may not be enough for some devices, and is more than needed for many.  Could use drive info on device name to determine required size.
                uint q = 1024 * 1024 * 64; // 1024 bytes == 1k * 1024 == 1 meg * 64 == 64 megs

                uint size = 0;
                pAlloc = Marshal.AllocHGlobal((int)q);
                IntPtr pDest = pAlloc;

                bool fResult = DeviceIoControl(
                    hDevice,
                    FSConstants.FSCTL_GET_VOLUME_BITMAP,
                    p,
                    (uint)Marshal.SizeOf(i64),
                    pDest,
                    q,
                    ref size,
                    IntPtr.Zero);

                if (!fResult)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                handle.Free();

                /*
                object returned was...
          typedef struct
          {
           LARGE_INTEGER StartingLcn;
           LARGE_INTEGER BitmapSize;
           BYTE Buffer[1];
          } VOLUME_BITMAP_BUFFER, *PVOLUME_BITMAP_BUFFER;
                */
                Int64 startingLcn = (Int64)Marshal.PtrToStructure(pDest, typeof(Int64));

                Debug.Assert(startingLcn == 0);

                pDest = (IntPtr)((Int64)pDest + 8);
                Int64 bitmapSize = (Int64)Marshal.PtrToStructure(pDest, typeof(Int64));

                Int32 byteSize = (int)(bitmapSize / 8);
                byteSize++; // round up - even with no remainder

                IntPtr bitmapBegin = (IntPtr)((Int64)pDest + 8);

                byte[] byteArr = new byte[byteSize];

                Marshal.Copy(bitmapBegin, byteArr, 0, (Int32)byteSize);

                BitArray retVal = new BitArray(byteArr);
                retVal.Length = (int)bitmapSize; // truncate to exact cluster count
                return retVal;
            }
            finally
            {
                CloseHandle(hDevice);
                hDevice = IntPtr.Zero;

                Marshal.FreeHGlobal(pAlloc);
                pAlloc = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Get cluster usage for a device
        /// </summary>
        /// <param name="DeviceName">use "c:"</param>
        /// <returns>a bitarray for each cluster</returns>
        public static void TryOpenDevice(string deviceName)
        {
            IntPtr hDevice = IntPtr.Zero;
            try
            {
                hDevice = OpenVolume(deviceName);
            }
            finally
            {
                CloseHandle(hDevice);
                hDevice = IntPtr.Zero;
            }
        }


        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
             [In] IntPtr hProcess,
             [Out] out bool wow64Process
             );

        /// <summary>
        /// returns a 2*number of extents array -
        /// the vcn and the lcn as pairs
        /// </summary>
        /// <param name="path">file to get the map for ex: "c:\windows\explorer.exe" </param>
        /// <returns>An array of [virtual cluster, physical cluster]</returns>
        public static List<KeyValuePair<long, long>> GetFileMap(string path)
        {
            IntPtr hFile = IntPtr.Zero;
            IntPtr pAlloc = IntPtr.Zero;

            try
            {
                hFile = OpenFile(path);

                Int64 i64 = 0;

                GCHandle handle = GCHandle.Alloc(i64, GCHandleType.Pinned);
                IntPtr p = handle.AddrOfPinnedObject();

                // Supports files of up to about 4million fragments.  This seems like overkill.  Probably should size it dynamically based off the number of clusters the file takes up.
                uint q = 1024 * 1024 * 64; // 1024 bytes == 1k * 1024 == 1 meg * 64 == 64 megs

                uint size = 0;
                pAlloc = Marshal.AllocHGlobal((int)q);
                IntPtr pDest = pAlloc;
                bool fResult = DeviceIoControl(
                    hFile,
                    FSConstants.FSCTL_GET_RETRIEVAL_POINTERS,
                    p,
                    (uint)Marshal.SizeOf(i64),
                    pDest,
                    q,
                    ref size,
                    IntPtr.Zero);

                if (!fResult)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                handle.Free();

                /*
                returned back one of...
     typedef struct RETRIEVAL_POINTERS_BUFFER { 
     DWORD ExtentCount; 
     LARGE_INTEGER StartingVcn; 
     struct {
         LARGE_INTEGER NextVcn;
      LARGE_INTEGER Lcn;
        } Extents[1];
     } RETRIEVAL_POINTERS_BUFFER, *PRETRIEVAL_POINTERS_BUFFER;
    */

                Int32 extentCount = (Int32)Marshal.PtrToStructure(pDest, typeof(Int32));

                // on 64bit environment we need 8byte offset, however we need it even if we are running inside of wow64 as a 32bit process.
                // Therefore we need to pass that work off to some smarter code than usual...
                pDest = (IntPtr)((Int64)pDest + KernelAllignment());

                Int64 startingVcn = (Int64)Marshal.PtrToStructure(pDest, typeof(Int64));

                Debug.Assert(startingVcn == 0);

                pDest = (IntPtr)((Int64)pDest + 8);

                // now pDest points at an array of pairs of Int64s.

                List<KeyValuePair<long, long>> retVal = new List<KeyValuePair<long, long>>(extentCount);

                for (int i = 0; i < extentCount; i++)
                {
                    long key = -1;
                    long value = -1;
                    for (int j = 0; j < 2; j++)
                    {
                        Int64 v = (Int64)Marshal.PtrToStructure(pDest, typeof(Int64));
                        if (j == 0)
                            key = v;
                        else
                            value = v;
                        pDest = (IntPtr)((Int64)pDest + 8);
                    }
                    retVal.Add(new KeyValuePair<long, long>(key, value));
                }

                return retVal;
            }
            finally
            {
                CloseHandle(hFile);
                hFile = IntPtr.Zero;

                Marshal.FreeHGlobal(pAlloc);
                pAlloc = IntPtr.Zero;
            }
        }

        private static long KernelAllignment()
        {
            int intPtrSize = Marshal.SizeOf(typeof(IntPtr));
            if (intPtrSize > 4)
                return (long)intPtrSize;
            bool wow64Process = IsWow64();
            if (wow64Process)
            {
                return 8;
            }
            return 4;
        }

        private static bool IsWow64()
        {
            bool wow64Process = false;
            try
            {
                using (Process current = Process.GetCurrentProcess())
                {
                    if (IsWow64Process(current.Handle, out wow64Process))
                    {
                    }
                }
            }
            catch
            {
            }
            return wow64Process;
        }

        /// <summary>
        /// input structure for use in MoveFile
        /// </summary>
        private struct MoveFileData
        {
            public IntPtr hFile;
            public Int64 StartingVCN;
            public Int64 StartingLCN;
            public Int32 ClusterCount;
        }

        /// <summary>
        /// move a virtual cluster for a file to a logical cluster on disk, repeat for count clusters
        /// </summary>
        /// <param name="deviceName">device to move on"c:"</param>
        /// <param name="path">file to muck with "c:\windows\explorer.exe"</param>
        /// <param name="VCN">cluster number in file to move</param>
        /// <param name="LCN">cluster on disk to move to</param>
        /// <param name="count">for how many clusters</param>
        static public void MoveFile(string deviceName, string path, Int64 VCN, Int64 LCN, Int32 count)
        {
            IntPtr hVol = IntPtr.Zero;
            IntPtr hFile = IntPtr.Zero;
            try
            {
                hVol = OpenVolume(deviceName);

                hFile = OpenFile(path);

                MoveFileData mfd = new MoveFileData();
                mfd.hFile = hFile;
                mfd.StartingVCN = VCN;
                mfd.StartingLCN = LCN;
                mfd.ClusterCount = count;

                GCHandle handle = GCHandle.Alloc(mfd, GCHandleType.Pinned);
                uint bufSize = (uint)Marshal.SizeOf(mfd);

                IntPtr p = handle.AddrOfPinnedObject();
                uint size = 0;

                bool fResult = DeviceIoControl(
                    hVol,
                    FSConstants.FSCTL_MOVE_FILE,
                    p,
                    bufSize,
                    IntPtr.Zero, // no output data from this FSCTL
                    0,
                    ref size,
                    IntPtr.Zero);

                if (!fResult)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                handle.Free();
            }
            finally
            {
                CloseHandle(hVol);
                CloseHandle(hFile);
            }
        }

        /// <summary>
        /// constants lifted from winioctl.h from platform sdk
        /// </summary>
        private class FSConstants
        {
            const uint FILE_DEVICE_FILE_SYSTEM = 0x00000009;

            const uint METHOD_NEITHER = 3;
            const uint METHOD_BUFFERED = 0;

            const uint FILE_ANY_ACCESS = 0;
            const uint FILE_SPECIAL_ACCESS = FILE_ANY_ACCESS;

            public static readonly uint FSCTL_GET_VOLUME_BITMAP = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 27, METHOD_NEITHER, FILE_ANY_ACCESS);
            public static readonly uint FSCTL_GET_RETRIEVAL_POINTERS = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 28, METHOD_NEITHER, FILE_ANY_ACCESS);
            public static readonly uint FSCTL_MOVE_FILE = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 29, METHOD_BUFFERED, FILE_SPECIAL_ACCESS);

            static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
            {
                return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method);
            }
        }
    }


}
