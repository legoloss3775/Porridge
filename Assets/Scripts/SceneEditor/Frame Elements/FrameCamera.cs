using UnityEditor;
using static FrameCore.FrameElement;

namespace FrameCore {
    public class FrameCamera : FrameElement {

    }
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameCamera))]
    [CanEditMultipleObjects]
    public class FrameBackgroundCustomInspector : FrameElementCustomInspector {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            this.SetElementInInspector<ScriptableObjects.FrameCameraSO>();
        }
    }
#endif
    #endregion
}
