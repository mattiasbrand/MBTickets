﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace RavenDBMembership
{
    public static class PasswordUtil
    {
        public static string CreateRandomSalt()
        {
            var saltBytes = new Byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            var dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }
    }
}