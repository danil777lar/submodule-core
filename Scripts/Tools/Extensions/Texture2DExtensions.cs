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
        string dirPath = Application.persistentDataPath + dir;
        if(!Directory.Exists(dirPath)) 
        {
            Directory.CreateDirectory(dirPath);
        }
        string photoName = name + ".png";
        File.WriteAllBytes(dirPath + photoName, bytes);
    }
    
    public static Texture2D LoadPng(this Texture2D texture, string path)
    {
        texture.LoadImage(File.ReadAllBytes(Application.persistentDataPath + path + ".png"));
        return texture;
    }
}
