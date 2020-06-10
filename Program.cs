﻿using System;

namespace libusb
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var usb_ctx = new UsbContext())
            {
                foreach (var device in usb_ctx.GetDeviceList())
                {
                    var descriptor = new device_descriptor();
                    usb_ctx.GetDeviceDescriptor(device, ref descriptor);
                    Console.WriteLine($"Vendor 0x{descriptor.idVendor:x} Product 0x{descriptor.idProduct:x}");
                    if (descriptor.idVendor == 0x1050 && descriptor.idProduct == 0x30)
                    {
                        using (var usb_session = usb_ctx.CreateSession(device))
                        {
                            var manufacturer = usb_session.GetStringDescriptor(descriptor.iManufacturer);
                            var product = usb_session.GetStringDescriptor(descriptor.iProduct);
                            var serial = usb_session.GetStringDescriptor(descriptor.iSerialNumber);
                            Console.WriteLine($"Manufacturer '{manufacturer}' Product '{product}' Serial '{serial}'");

                            var def_key = new byte[] {0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x0b, 0x47, 0xdb, 0xed, 0x59, 0x56, 0x54, 0x90, 0x1d, 0xee, 0x1c, 0xc6, 0x55, 0xe4, 0x20,
     0x59, 0x2f, 0xd4, 0x83, 0xf7, 0x59, 0xe2, 0x99, 0x09, 0xa0, 0x4c, 0x45, 0x05, 0xd2, 0xce, 0x0a};

                            usb_session.SendCmd(HsmCommand.SetInformation, def_key);

                            using (var scp03_session = new Scp03Context("password").CreateSession(usb_session, 1))
                            {
                                var info = scp03_session.SendCmd(HsmCommand.DeviceInfo);
                                Console.WriteLine("DeviceInfo over scp03_session");
                                foreach (var b in info)
                                    Console.Write($"{b:x2}");
                                Console.WriteLine();
                                var rand1 = scp03_session.SendCmd(new GetPseudoRandomReq { length = 64 });
                                Console.WriteLine("GetPseudoRandom over scp03_session");
                                foreach (var b in rand1)
                                    Console.Write($"{b:x2}");
                                Console.WriteLine();
                                using (var scp11_session = new Scp11Context(scp03_session, 2).CreateSession(usb_session, 2))
                                {
                                    var info2 = scp11_session.SendCmd(HsmCommand.DeviceInfo);
                                    Console.WriteLine("DeviceInfo over scp11_session");
                                    foreach (var b in info2)
                                        Console.Write($"{b:x2}");
                                    Console.WriteLine();
                                    var rand2 = scp11_session.SendCmd(new GetPseudoRandomReq { length = 64 });
                                    Console.WriteLine("GetPseudoRandom over scp11_session");
                                    foreach (var b in rand2)
                                        Console.Write($"{b:x2}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
