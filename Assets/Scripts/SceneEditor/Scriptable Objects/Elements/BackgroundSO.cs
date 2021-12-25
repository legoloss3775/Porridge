using UnityEngine;

namespace FrameCore {
    namespace ScriptableObjects {
        [CreateAssetMenu(fileName = "Background", menuName = "Редактор Сцен/Бэкграунд")]
        public class BackgroundSO : FrameElementSO {
            public enum BackgroundType {
                Default,
            }
            public BackgroundType type;

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
}
