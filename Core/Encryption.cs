﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backup_Generator.Core
{
    class Encryption
    {
        //public keys 
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        //amount of rounds of encryption 
        private const int Rounds = 8;
        private Random r = new Random(Environment.TickCount);

        public byte[] Encrypt(byte[] data, int round = 0)
        {
            /* keys */
            byte[] rtn;
            byte[] key = new byte[256];
            int modifier = r.Next(0, 256);

            //Generate key = private key ^ public key 
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (byte)((int)PrivateKey[i % PrivateKey.Length] ^ (int)PublicKey[i % PublicKey.Length]);
            }

            shift(ref data);
            rtn = new byte[data.Length + 1]; //create the output array for this rounds 

            //loop through the data to encrypt 
            for (int i = 0; i < data.Length; i++)
            {
                rtn[i] = (byte)(data[i] ^ (key[i % key.Length] ^ (modifier % (i + 1))));
                rtn[i] = (byte)(rtn[i] + modifier);
            }
            //append the key 
            rtn[rtn.Length - 1] = (byte)(modifier ^ key[0]);

            round++;

            if (round == Rounds)
                return rtn;
            else
                return Encrypt(rtn, round);
        }

        public byte[] Decrypt(byte[] data, int rounds = 0)
        {
            //vars for decryption 
            byte[] rtn;
            byte[] key = new byte[256];
            int modifier = 0;

            //Generate key = private key ^ public key 
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (byte)((int)PrivateKey[i % PrivateKey.Length] ^ (int)PublicKey[i % PublicKey.Length]);
            }
            shift(ref data);
            modifier = (int)data[data.Length - 1] ^ key[0]; //grab the key from the end of the array 
            rtn = new byte[data.Length - 1]; //create the output array for this rounds 

            //loop through the data to decrypt 
            for (int i = 0; i < data.Length - 1; i++)
            {
                //rtn[i] = (byte)(data[i] ^ (key[i % key.Length] ^ (modifier % (i + 1))));
                rtn[i] = (byte)(data[i] - modifier);
                rtn[i] = (byte)(rtn[i] ^ (key[i % key.Length] ^ (modifier % (i + 1))));
            }

            rounds++;

            if (rounds == Rounds)
                return rtn;
            else
                return Decrypt(rtn, rounds);
        }

        public static void shift(ref byte[] data)
        {
            int j;

            for (int i = 0; i < data.Length; i++)
            {
                j = data[data.Length - 1 - i];
                data[data.Length - 1 - i] = data[i];
                data[i] = (byte)j;
            }
        }
    }
}
