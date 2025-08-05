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
        
        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        
        Camera renderCamera = Camera.main;
        RenderTexture rt = new RenderTexture(size.x, size.y, 24);
        renderCamera.targetTexture = rt;
        Texture2D screenImage = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        renderCamera.Render();

        RenderTexture.active = rt;
        screenImage.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        screenImage.Apply();
        screenImage.SavePng(filePath, fileName);

        renderCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Destroy(screenImage);
    }
}
