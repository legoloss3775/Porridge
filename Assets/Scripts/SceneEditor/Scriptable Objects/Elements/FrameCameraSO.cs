using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.ScriptableObjects {
    [CreateAssetMenu(fileName = "FrameCamera", menuName = "Редактор Сцен/Камера")]
    public class FrameCameraSO : FrameElementSO {
        public override void OnAfterDeserialize() {
            base.OnAfterDeserialize();
        }

        public override void OnBeforeSerialize() {
            base.OnBeforeSerialize();
        }
        public override void OnEnable() {
            base.OnEnable();
        }
    }
}