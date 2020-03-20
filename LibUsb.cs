﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace usblib_tester
{
    public class LibUsb : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct device_descriptor
        {
            /** Size of this descriptor (in bytes) */
            public byte bLength;

            /** Descriptor type. Will have value
             * \ref libusb_descriptor_type::LIBUSB_DT_DEVICE LIBUSB_DT_DEVICE in this
             * context. */
            public byte bDescriptorType;

            /** USB specification release number in binary-coded decimal. A value of
             * 0x0200 indicates USB 2.0, 0x0110 indicates USB 1.1, etc. */
            public ushort bcdUSB;

            /** USB-IF class code for the device. See \ref libusb_class_code. */
            public byte bDeviceClass;

            /** USB-IF subclass code for the device, qualified by the bDeviceClass
             * value */
            public byte bDeviceSubClass;

            /** USB-IF protocol code for the device, qualified by the bDeviceClass and
             * bDeviceSubClass values */
            public byte bDeviceProtocol;

            /** Maximum packet size for endpoint 0 */
            public byte bMaxPacketSize0;

            /** USB-IF vendor ID */
            public ushort idVendor;

            /** USB-IF product ID */
            public ushort idProduct;

            /** Device release number in binary-coded decimal */
            public ushort bcdDevice;

            /** Index of string descriptor describing manufacturer */
            public byte iManufacturer;

            /** Index of string descriptor describing product */
            public byte iProduct;

            /** Index of string descriptor containing device serial number */
            public byte iSerialNumber;

            /** Number of possible configurations */
            public byte bNumConfigurations;
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_init_t(out IntPtr ctx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void libusb_exit_t(IntPtr ctx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long libusb_get_device_list_t(IntPtr ctx, out IntPtr device_list);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void libusb_free_device_list_t(IntPtr device_list, int unref_devices);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_get_device_descriptor_t(IntPtr device, ref device_descriptor descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr libusb_ref_device_t(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr libusb_unref_device_t(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_open_t(IntPtr device, out IntPtr device_handle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void libusb_close_t(IntPtr device_handle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_claim_interface_t(IntPtr device_handle, int interface_number);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_release_interface_t(IntPtr device_handle, int interface_number);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_bulk_transfer_t(IntPtr device_handle, byte endpoint, IntPtr data, int length, out int actual_length, uint timeout);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_control_transfer_t(IntPtr device_handle, byte request_type, byte request, ushort value, ushort index, IntPtr data, ushort length, uint timeout);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int libusb_get_string_descriptor_ascii_t(IntPtr device_handle, byte desc_index, IntPtr data, int length);

        public libusb_init_t init;
        public libusb_exit_t exit;
        public libusb_get_device_list_t get_device_list;
        public libusb_free_device_list_t free_device_list;
        public libusb_get_device_descriptor_t get_device_descriptor;
        public libusb_ref_device_t ref_device;
        public libusb_unref_device_t unref_device;
        public libusb_open_t open;
        public libusb_close_t close;
        public libusb_claim_interface_t claim_interface;
        public libusb_release_interface_t release_interface;
        public libusb_bulk_transfer_t bulk_transfer;
        public libusb_control_transfer_t control_transfer;
        public libusb_get_string_descriptor_ascii_t get_string_descriptor_ascii;

        private IntPtr libusb;

        public LibUsb(string libraryPath = "/usr/local/lib/libusb-1.0.dylib")
        {
            libusb = NativeLibrary.Load(libraryPath);
            init = Marshal.GetDelegateForFunctionPointer<libusb_init_t>(NativeLibrary.GetExport(libusb, "libusb_init"));
            exit = Marshal.GetDelegateForFunctionPointer<libusb_exit_t>(NativeLibrary.GetExport(libusb, "libusb_exit"));
            get_device_list = Marshal.GetDelegateForFunctionPointer<libusb_get_device_list_t>(NativeLibrary.GetExport(libusb, "libusb_get_device_list"));
            free_device_list = Marshal.GetDelegateForFunctionPointer<libusb_free_device_list_t>(NativeLibrary.GetExport(libusb, "libusb_free_device_list"));
            get_device_descriptor = Marshal.GetDelegateForFunctionPointer<libusb_get_device_descriptor_t>(NativeLibrary.GetExport(libusb, "libusb_get_device_descriptor"));
            ref_device = Marshal.GetDelegateForFunctionPointer<libusb_ref_device_t>(NativeLibrary.GetExport(libusb, "libusb_ref_device"));
            unref_device = Marshal.GetDelegateForFunctionPointer<libusb_unref_device_t>(NativeLibrary.GetExport(libusb, "libusb_unref_device"));
            open = Marshal.GetDelegateForFunctionPointer<libusb_open_t>(NativeLibrary.GetExport(libusb, "libusb_open"));
            close = Marshal.GetDelegateForFunctionPointer<libusb_close_t>(NativeLibrary.GetExport(libusb, "libusb_close"));
            claim_interface = Marshal.GetDelegateForFunctionPointer<libusb_claim_interface_t>(NativeLibrary.GetExport(libusb, "libusb_claim_interface"));
            release_interface = Marshal.GetDelegateForFunctionPointer<libusb_release_interface_t>(NativeLibrary.GetExport(libusb, "libusb_release_interface"));
            bulk_transfer = Marshal.GetDelegateForFunctionPointer<libusb_bulk_transfer_t>(NativeLibrary.GetExport(libusb, "libusb_bulk_transfer"));
            control_transfer = Marshal.GetDelegateForFunctionPointer<libusb_control_transfer_t>(NativeLibrary.GetExport(libusb, "libusb_control_transfer"));
            get_string_descriptor_ascii = Marshal.GetDelegateForFunctionPointer<libusb_get_string_descriptor_ascii_t>(NativeLibrary.GetExport(libusb, "libusb_get_string_descriptor_ascii"));
        }

        public void Dispose()
        {
            NativeLibrary.Free(libusb);
            libusb = IntPtr.Zero;
        }

        public IEnumerable<IntPtr> GetUsbDevices(IntPtr ctx)
        {
            var ret = get_device_list(ctx, out var device_list);
            for (int i = 0; i < ret; i++)
            {
                yield return Marshal.ReadIntPtr(device_list, i * IntPtr.Size);
            }
            free_device_list(device_list, 1);
        }

        public int WriteUsb(IntPtr device_handle, byte[] data)
        {
            IntPtr mem = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, mem, data.Length);
            int ret = bulk_transfer(device_handle, 0x01, mem, data.Length, out var actual_length, 0);
            if (ret < 0)
            {
                Marshal.FreeHGlobal(mem);
                return ret;
            }
            if (data.Length % 64 == 0)
            {
                ret = bulk_transfer(device_handle, 0x01, mem, 0, out _, 0);
                if (ret < 0)
                {
                    Marshal.FreeHGlobal(mem);
                    return ret;
                }
            }
            Marshal.FreeHGlobal(mem);
            return actual_length;
        }

        public int ReadUsb(IntPtr device_handle, out byte[] data, int max = 2048)
        {
            IntPtr mem = Marshal.AllocHGlobal(max);
            int ret = bulk_transfer(device_handle, 0x81, mem, max, out var actual_length, 0);
            if (ret < 0)
            {
                Marshal.FreeHGlobal(mem);
                data = null;
                return ret;
            }
            data = new byte[actual_length];
            Marshal.Copy(mem, data, 0, actual_length);
            Marshal.FreeHGlobal(mem);
            return actual_length;
        }

        public int GetStringDescriptor(IntPtr device_handle, byte index, out string descriptor, int max = 1024)
        {
            var mem = Marshal.AllocHGlobal(max);
            var ret = get_string_descriptor_ascii(device_handle, index, mem, max);
            if (ret < 0)
            {
                descriptor = null;
                Marshal.FreeHGlobal(mem);
                return ret;
            }
            descriptor = Marshal.PtrToStringAnsi(mem, ret);
            Marshal.FreeHGlobal(mem);
            return ret;
        }
    }
}