using UnityEngine;

public class GUIDHolder : MonoBehaviour
{
    public string GUID { get; private set; }

    public void GenerateGUID()
    {
        GUID = System.Guid.NewGuid().ToString();
    }
}
