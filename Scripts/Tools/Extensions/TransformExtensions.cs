using UnityEngine;

public static class TransformExtensions
{
    public static string GetPath(this Transform transform)
    {
        string path = transform.gameObject.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.gameObject.name + "/" + path;
        }
        return path;
    }
}
