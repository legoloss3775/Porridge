using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FrameUIWindow : FrameElement
{
    public Canvas canvas;

#if UNITY_EDITOR

    [CustomEditor(typeof(FrameUIWindow))]
    [CanEditMultipleObjects]
    public class FrameUIWindowCustomInspector : FrameElementCustomInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.SetElementInInspector<FrameUIWindowSO>();
        }
    }
#endif
}
