using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleMethodPanel : MonoBehaviour
{
    [SerializeField] private Button executeButton;
    [SerializeField] private TextMeshProUGUI methodName;

    public void Init(MethodInfo methodInfo)
    {
        methodName.text = methodInfo.Name;
    }
}
