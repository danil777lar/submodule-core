using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 X(this Vector3 s) => new Vector3(s.x, 0f, 0f);
    public static Vector3 Y(this Vector3 s) => new Vector3(0f, s.y, 0f);
    public static Vector3 Z(this Vector3 s) => new Vector3(0f, 0f, s.z);

    public static Vector3 XY(this Vector3 s) => new Vector3(s.x, s.y, 0f);
    public static Vector3 YZ(this Vector3 s) => new Vector3(0f, s.y, s.z);
    public static Vector3 XZ(this Vector3 s) => new Vector3(s.x, 0f, s.z);

    public static Vector3 XXX(this Vector3 s) => new Vector3(s.x, s.x, s.x);
    public static Vector3 YYY(this Vector3 s) => new Vector3(s.y, s.y, s.y);
    public static Vector3 ZZZ(this Vector3 s) => new Vector3(s.z, s.z, s.z);
    
    public static Vector3 Parse(string s)
    {
        s = s.Replace("(", "").Replace(")", "");
        string[] parts = s.Split(',');
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }
}
