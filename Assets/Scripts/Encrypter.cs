
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;

public static class Encrypter
{
    private static string hash = "@tank$";

    //Encode with Base64 and Encrypt With TDS algorithm
    public static string encrypt(string input)
    {
        byte[] data = UTF8Encoding.UTF8.GetBytes(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider() {  Key=key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform ict = tds.CreateEncryptor();
                byte[] results = ict.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(results, 0, results.Length);
            }
        }
    }

    //Decode with Base64 and Decrypt With TDS algorithm
    public static string decrypt(string input)
    {
        Debug.Log(input);
        byte[] data = Convert.FromBase64String(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform ict = tds.CreateDecryptor();
                byte[] results = ict.TransformFinalBlock(data, 0, data.Length);
                return UTF8Encoding.UTF8.GetString(results);
            }
        }
    }


}
