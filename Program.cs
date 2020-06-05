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
                            usb_session.GetStringDescriptor(descriptor.iManufacturer, 0, out var manufacturer);
                            usb_session.GetStringDescriptor(descriptor.iProduct, 0, out var product);
                            usb_session.GetStringDescriptor(descriptor.iSerialNumber, 0, out var serial);
                            Console.WriteLine($"Manufacturer '{manufacturer}' Product '{product}' Serial '{serial}'");

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
