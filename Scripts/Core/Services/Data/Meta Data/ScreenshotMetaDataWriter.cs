using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScreenshotMetaDataWriter : MonoBehaviour, IMetaDataWriter
{
    [SerializeField] private Vector2Int screenshotSize = new Vector2Int(480, 270);
    
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

        Vector2Int size = screenshotSize;
        
        Camera renderCamera = Camera.main;
        List<Camera> childCameras = renderCamera.GetComponentsInChildren<Camera>()
            .ToList().FindAll(x => x != renderCamera);
        childCameras.ForEach(x => x.enabled = false);

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

        childCameras.ForEach(x => x.enabled = true);
    }
}
