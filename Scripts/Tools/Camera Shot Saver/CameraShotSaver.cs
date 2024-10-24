using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Larje.Core.Tools.CameraShotSaver
{
    public class CameraShotSaver : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shot();
            }
        }
        
        [ContextMenu("Shot")]
        private void Shot()
        {
            #if UNITY_EDITOR
            Camera cam = Camera.main;
            Vector2Int size = new Vector2Int(Screen.width, Screen.height);
            
            Debug.Log($"Screen width: {size.x}, height: {size.y}");
            
            RenderTexture texture = new RenderTexture(size.x, size.y, 24);
            cam.targetTexture = texture;
            cam.Render();
            RenderTexture.active = texture;
            
            Texture2D texture2D = new Texture2D(size.x, size.y);
            texture2D.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            
            cam.targetTexture = null;
 
            byte[] bytes = texture2D.EncodeToPNG();

            string path = EditorUtility.SaveFilePanel("save", "", "shot.png", "png");
            File.WriteAllBytes(path, bytes);
            #endif
        }
    }
}