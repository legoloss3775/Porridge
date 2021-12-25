using FrameCore.ScriptableObjects.UI;
using UnityEditor;
using UnityEngine;

namespace FrameCore {
    namespace UI {
        /// <summary>
        /// Для сериализации параметров
        /// <see cref="FrameElement">
        /// </summary>
        public class Window : FrameElement {
            public Canvas canvas;

            #region EDITOR
#if UNITY_EDITOR
            [CustomEditor(typeof(Window))]
            [CanEditMultipleObjects]
            public class FrameUIWindowCustomInspector : FrameElementCustomInspector {
                public override void OnInspectorGUI() {
                    base.OnInspectorGUI();
                    this.SetElementInInspector<WindowSO>();
                }
            }
#endif
            #endregion
        }
    }
}

