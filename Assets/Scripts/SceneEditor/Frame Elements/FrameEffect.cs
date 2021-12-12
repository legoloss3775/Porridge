using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;
using System;

namespace FrameCore {
    
    namespace Serialization {
        [Serializable]
        public class FrameEffectValues : Values {
            public FrameEffectData frameEffectData;
            public FrameEffectValues(FrameEffect effect) {
                transformData = new TransformData {
                    position = effect.position,
                    activeStatus = effect.activeStatus,
                    size = effect.size,
                };
                frameEffectData = new FrameEffectData {
                    animationSpeed = effect.animationSpeed,
                };
            }
            public FrameEffectValues() { }
            [Serializable]
            public struct SerializedFrameEffectValues {
                public TransformData transformData;
                public FrameEffectData frameEffectData;
            }
            [SerializeField]
            public SerializedFrameEffectValues serializedFrameEffectValues {
                get {
                    return new SerializedFrameEffectValues {
                        transformData = transformData,
                        frameEffectData = frameEffectData,
                    };
                }
            }
            public static void LoadSerializedFrameEffectValues (List<SerializedFrameEffectValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new FrameEffectValues {
                        transformData = svalue.transformData,
                        frameEffectData = svalue.frameEffectData
                    }
                    );
                }
            }
        }
    }
    public class FrameEffect : FrameElement {
        public float animationSpeed = 1f;

        #region VALUES_SETTINGS
        public override Values GetFrameKeyValuesType() {
            return new Serialization.FrameEffectValues(this);
        }
        public override void UpdateValuesFromKey(Values frameKeyValues) {
            var keyValues = (Serialization.FrameEffectValues)frameKeyValues;
            activeStatus = keyValues.transformData.activeStatus;
            position = keyValues.transformData.position;
            size = keyValues.transformData.size;

            animationSpeed = keyValues.frameEffectData.animationSpeed;

        }
        #endregion
    }
}
