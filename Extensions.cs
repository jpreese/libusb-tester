﻿using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace libusb
{
    public static class Extensions
    {
        public static Span<byte> AsSpan(this ECPublicKeyParameters pub)
        {
            return pub.Q.GetEncoded().AsSpan(1);
        }

        public static Memory<byte> AsMemory(this ECPublicKeyParameters pub)
        {
            return pub.Q.GetEncoded().AsMemory(1);
        }

        public static byte[] ToByteArrayFixed(this BigInteger num, int size = 32)
        {
            var ret = num.ToByteArrayUnsigned();
            if(ret.Length != size)
            {
                var bytes = new byte[size];
                ret.CopyTo(bytes, size - ret.Length);
                return bytes;
            }
            return ret;
        }

        public static void BlockUpdate(this IDigest digest, ReadOnlySpan<byte> input)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(input.Length);
            input.CopyTo(bytes);
            digest.BlockUpdate(bytes, 0, input.Length);
            ArrayPool<byte>.Shared.Return(bytes);
        }

        public static void BlockUpdate(this IMac mac, ReadOnlySpan<byte> input)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(input.Length);
            input.CopyTo(bytes);
            mac.BlockUpdate(bytes, 0, input.Length);
            ArrayPool<byte>.Shared.Return(bytes);
        }

        public static void BlockUpdate(this IMac mac, ECPublicKeyParameters input)
        {
            var bytes = input.Q.GetEncoded();
            mac.BlockUpdate(bytes, 1, bytes.Length - 1);
        }

        public static void BlockUpdate(this IMac mac, MemoryStream input)
        {
            mac.BlockUpdate(input.GetBuffer(), 0, (int)input.Length);
        }

        public static Span<byte> AsSpan(this MemoryStream s)
        {
            return s.GetBuffer().AsSpan(0, (int)s.Length);
        }

        public static void Write(this Stream s, ushort value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(2);
            BinaryPrimitives.WriteUInt16BigEndian(bytes, value);
            s.Write(bytes, 0, 2);
            ArrayPool<byte>.Shared.Return(bytes);
        }

        public static void Write(this Stream s, uint value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(4);
            BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
            s.Write(bytes, 0, 4);
            ArrayPool<byte>.Shared.Return(bytes);
        }

        public static Span<byte> AsSpan(this IWriteable w)
        {
            var s = new MemoryStream();
            w.WriteTo(s);
            return s.AsSpan();
        }
    }
}