﻿using System.Security.Cryptography;

namespace Elektrifikatsiya.Utilities;

public static class SecureStringGenerator
{
    public static string CreateCryptographicRandomString(int count)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }

    public static string CreateCryptographicRandomString(int count, int uid)
    {
        List<byte> bytes = RandomNumberGenerator.GetBytes(count).ToList();

        bytes.AddRange(BitConverter.GetBytes(uid));

        return Convert.ToBase64String(bytes.ToArray());
    }
}