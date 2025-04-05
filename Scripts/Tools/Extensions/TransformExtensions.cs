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

    public static void DestroyAllChildren(this Transform transform)
	{
		for (int t = transform.childCount - 1; t >= 0; t--)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
			}
		}
	}
}
