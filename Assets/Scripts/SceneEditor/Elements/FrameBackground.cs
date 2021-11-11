using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FrameBackground : FrameElement
{

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameBackground))]
    [CanEditMultipleObjects]
    public class FrameBackgroundCustomInspector : FrameElementCustomInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.SetElementInInspector<FrameBackgroundSO>();
        }
    }
#endif
}
