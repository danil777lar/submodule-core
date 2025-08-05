using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Texture2DExtensions
{
    public static Texture2D Copy(this Texture2D texture)
    {
        Texture2D copy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
        copy.LoadRawTextureData(texture.GetRawTextureData());
        copy.Apply();
        return copy;
    }
    
    public static void SavePng(this Texture2D texture, string dir, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Path.Combine(Application.persistentDataPath, dir);
        if(!Directory.Exists(fullPath)) 
        {
            Directory.CreateDirectory(fullPath);
        }
        string fileName = name + ".png";
        fullPath = Path.Combine(fullPath, fileName);
        File.WriteAllBytes(fullPath, bytes);
    }
    
    public static bool LoadPng(this Texture2D texture, string path)
    {
        try
        {
            string fullPath = Path.Combine(Application.persistentDataPath, path + ".png");
            byte[] bytes = File.ReadAllBytes(fullPath);
            return texture.LoadImage(bytes);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Texture2DExtensions: LoadPng failed: " + e.Message);
        }
        
        return false;
    }
}
