using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FrameCore.ScriptableObjects;

namespace FrameCore {
    /// <summary>
    /// Для сериализации параметров
    /// <see cref="FrameElement">
    /// </summary>
    public class Background : FrameElement {

        #region EDITOR
#if UNITY_EDITOR
        [CustomEditor(typeof(Background))]
        [CanEditMultipleObjects]
        public class FrameBackgroundCustomInspector : FrameElementCustomInspector {
            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                this.SetElementInInspector<BackgroundSO>();
            }
        }
#endif
        #endregion
    }
}
