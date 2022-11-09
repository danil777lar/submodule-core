using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Services.UI
{
    [CustomEditor(typeof(UIOpenCloseAnimation))]
    public class UIOpenCloseAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UIOpenCloseAnimation anim = (UIOpenCloseAnimation)target;
                //target.
        }
    } 
}
