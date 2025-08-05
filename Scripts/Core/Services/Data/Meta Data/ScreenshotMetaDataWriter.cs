using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotMetaDataWriter : MonoBehaviour, IMetaDataWriter
{
    public void WriteMetaData(SaveMetaData metaData)
    {
        string fileName = $"{metaData.name}_screenshot";
        string filePath = Path.Combine("Screenshots");
        metaData.imagePath = Path.Combine(filePath, fileName);
        StartCoroutine(SaveScreenshotCo(filePath, fileName));
    }

    private IEnumerator SaveScreenshotCo(string filePath, string fileName)
    {
        yield return new WaitForEndOfFrame();
        
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        screenImage.SavePng(filePath, fileName);
        Destroy(screenImage);
    }
}
