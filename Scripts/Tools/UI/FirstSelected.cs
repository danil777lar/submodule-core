using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelected : MonoBehaviour
{   
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
