using System.IO;
using UnityEditor;
using UnityEngine;

public class GradientTextureGenerator : MonoBehaviour
{
    [SerializeField] private Gradient curve;
    [SerializeField] private Vector2Int size;
    
    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(size.x, size.y);
        for (int x = 0; x < size.x; x++)
        {
            float t = (float) x / size.x;
            for (int y = 0; y < size.y; y++)
            {
                texture.SetPixel(x, y, curve.Evaluate(t));
            }
        }
        texture.Apply();
        return texture;
    }
    
    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("save", "", "gradient.png", "png");
        File.WriteAllBytes(path, bytes);
    }

    [ContextMenu("Write Texture")]
    private void WriteTexture()
    {
        SaveTexture(GenerateTexture());
    }
}
