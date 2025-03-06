using System.Collections.Generic;
using UnityEngine;

public class GUIDHolder : MonoBehaviour
{
    private static List<GUIDHolder> _guidHolders = new List<GUIDHolder>();

    public static bool TryGetInstance<T>(string guid, out T component) where T : Component
    {
        GameObject go = _guidHolders.Find(x => x.GUID == guid)?.gameObject;
        component = go?.GetComponent<T>();
        return component != null;
    }


    [SerializeField] private string guid;

    public string GUID => guid;

    public void GenerateGUID()
    {
        guid = System.Guid.NewGuid().ToString();
    }
    
    private void Awake()
    {
        _guidHolders.Add(this);
    }
    
    private void OnDestroy()
    {
        _guidHolders.Remove(this);
    }
}
