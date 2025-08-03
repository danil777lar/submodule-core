using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public static class AESUtility
{
    public static string GenerateKey()
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }
    
    public static string GenerateIV()
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }
    }
    
    public static string Encrypt(string textStr, string keyStr, string ivStr)
    {
        Aes aes = Aes.Create();
        byte[] key = Convert.FromBase64String(keyStr);
        byte[] iv = Convert.FromBase64String(ivStr);
        
        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        StreamWriter sw = new StreamWriter(cs);
        sw.Write(textStr);
        sw.Close();
        
        return Convert.ToBase64String(ms.ToArray());
    }
    
    public static string Decrypt(string textStr, string keyStr, string ivStr)
    {
        byte[] text = Convert.FromBase64String(textStr);
        byte[] key = Convert.FromBase64String(keyStr);
        byte[] iv = Convert.FromBase64String(ivStr);

        Aes aes = Aes.Create();
        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        MemoryStream ms = new MemoryStream(text);
        CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}
