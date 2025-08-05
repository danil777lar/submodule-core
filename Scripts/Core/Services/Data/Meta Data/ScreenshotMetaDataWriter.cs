using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotMetaDataWriter : MonoBehaviour, IMetaDataWriter
{
    public void WriteMetaData(SaveMetaData metaData)
    {
        string fileName = $"{metaData.name}_screenshot.png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        metaData.imagePath = filePath;
        StartCoroutine(SaveScreenshotCo(filePath));
    }

    private IEnumerator SaveScreenshotCo(string filePath)
    {
        yield return new WaitForEndOfFrame();
        
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        
        byte[] imageBytes = screenImage.EncodeToPNG();
        File.WriteAllBytes(filePath, imageBytes);
        
        Debug.Log("Screenshot saved to: " + filePath);
    }
}
