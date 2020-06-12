﻿using System;
using System.IO;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace libusb
{
    public class Scp11Session : Scp03CryptoSession
    {
        public Scp11Session(Scp11Context context, Session session, ushort key_id)
        {
            var pair = context.generator.GenerateKeyPair();

            var epk_oce = (ECPublicKeyParameters)pair.Public;

            var esk_oce = AgreementUtilities.GetBasicAgreement("ECDH");
            esk_oce.Init(pair.Private);

            var req = new CreateSessionReq
            {
                key_id = key_id,
                buf = epk_oce.AsMemory()
            };
            var resp = session.SendCmd(req);

            session_id = resp[0];
            var epk_sd = resp.Slice(1, 64);
            var receipt = resp.Slice(1 + 64);

            var shs_oce = context.CalculateShs(esk_oce, context.DecodePoint(epk_sd), 4 * 16).ToArray();

            var receipt_key = new KeyParameter(shs_oce, 0, 16);
            key_enc = new KeyParameter(shs_oce, 16, 16);
            var key_mac = new KeyParameter(shs_oce, 32, 16);
            var key_rmac = new KeyParameter(shs_oce, 48, 16);

            var cmac = new CMac(new AesEngine());
            cmac.Init(receipt_key);
            cmac.BlockUpdate(epk_sd);
            cmac.BlockUpdate(epk_oce);
            var receipt_oce = new byte[16];
            cmac.DoFinal(receipt_oce, 0);

            if (!receipt.SequenceEqual(receipt_oce))
            {
                throw new IOException("The card receipt was invalid");
            }

            this.session = new Scp03CMacSession(session, key_mac, key_rmac, receipt_oce);
        }
    }
}