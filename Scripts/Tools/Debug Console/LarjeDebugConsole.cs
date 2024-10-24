using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Tools.LarjeDebugConsole
{
    public partial class LarjeDebugConsole : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_InputField inputField;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote))
            {
                SetActiveConsole(!canvas.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Return) && canvas.activeSelf)
            {
                ExecuteCommand();
            }
        }

        private void SetActiveConsole(bool arg)
        {
            canvas.gameObject.SetActive(arg);
            inputField.text = "";
        }

        private void ExecuteCommand()
        {
            List<string> input = inputField.text.Split(" ")
                .ToList()
                .FindAll(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x));
            inputField.text = "";
            for (int i = 0; i < input.Count; i++)
            {
                input[i] = new String(input[i].Where(x => Char.IsLetter(x) || Char.IsDigit(x)).ToArray());
            }
                
            if (input.Count > 0)
            {
                string methodName = input[0];
                List<string> methodParams = input.GetRange(1, input.Count - 1);
                MethodInfo method = typeof(LarjeDebugConsole).GetMethod(methodName);
                if (method != null)
                {
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    if (parameterInfo.Length == methodParams.Count)
                    {
                        method.Invoke(this, methodParams.ToArray());
                    }
                }
            }
        }
    }
}