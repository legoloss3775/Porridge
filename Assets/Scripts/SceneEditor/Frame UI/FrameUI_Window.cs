using UnityEditor;
using UnityEngine;

public class FrameUI_Window : FrameElement {
    public Canvas canvas;

#if UNITY_EDITOR

    [CustomEditor(typeof(FrameUI_Window))]
    [CanEditMultipleObjects]
    public class FrameUIWindowCustomInspector : FrameElementCustomInspector {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            this.SetElementInInspector<FrameUI_WindowSO>();
        }
    }
#endif
}
