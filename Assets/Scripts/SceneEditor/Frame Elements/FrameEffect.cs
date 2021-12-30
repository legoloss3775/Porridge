
using FrameCore.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore {

    namespace Serialization {
        [System.Serializable]
        public class FrameEffectValues : Values, IKeyTransition {
            public FrameEffectData frameEffectData;
            public CameraTurnAnimationData cameraTurnAnimationData;
            public KeySequenceData keySequenceData { get { return _keySequenceData; } set { _keySequenceData = value; } }
            [SerializeField]
            private KeySequenceData _keySequenceData;
            public FrameEffectValues(FrameEffect effect) {
                transformData = new TransformData {
                    position = effect.position,
                    activeStatus = effect.activeStatus,
                    size = effect.size,
                };
                frameEffectData = new FrameEffectData {
                    animationSpeed = effect.animationSpeed,
                    animationDelay = effect.animationDelay,
                };
                cameraTurnAnimationData = new CameraTurnAnimationData {
                    degreesX = effect.cameraTurnAnimationData.degreesX,
                    degreesY = effect.cameraTurnAnimationData.degreesY,
                    moveTo = effect.cameraTurnAnimationData.moveTo,
                };
                keySequenceData = new KeySequenceData {
                    nextKeyID = effect.keySequenceData.nextKeyID,
                    previousKeyID = effect.keySequenceData.previousKeyID,
                };
            }
            public FrameEffectValues() { }
            [System.Serializable]
            public struct SerializedFrameEffectValues {
                public TransformData transformData;
                public FrameEffectData frameEffectData;
                public CameraTurnAnimationData cameraTurnAnimationData;
                public KeySequenceData keySequenceData;
            }
            [SerializeField]
            public SerializedFrameEffectValues serializedFrameEffectValues {
                get {
                    return new SerializedFrameEffectValues {
                        transformData = transformData,
                        frameEffectData = frameEffectData,
                        cameraTurnAnimationData = cameraTurnAnimationData,
                        keySequenceData = keySequenceData,
                    };
                }
            }
            public static void LoadSerializedFrameEffectValues(List<SerializedFrameEffectValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new FrameEffectValues {
                        transformData = svalue.transformData,
                        frameEffectData = svalue.frameEffectData,
                        cameraTurnAnimationData = svalue.cameraTurnAnimationData,
                        keySequenceData = svalue.keySequenceData,
                    }
                    );
                }
            }
        }
    }
    public class FrameEffect : FrameElement, IKeyTransition {
        public float animationSpeed = 1f;
        public float animationDelay = 0;
        public Serialization.CameraTurnAnimationData cameraTurnAnimationData;

        public FrameEffects.EffectPrefab effectPrefab { get { return GetComponent<FrameEffects.EffectPrefab>(); } }

        public KeySequenceData keySequenceData { get { return _keySequenceData; } set { _keySequenceData = value; } }
        [SerializeField]
        private KeySequenceData _keySequenceData;

        public override void OnKeyChanged() {
            if (effectPrefab.gameObject.activeSelf) effectPrefab.OnFrameKeyChanged();
        }

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
            animationDelay = keyValues.frameEffectData.animationDelay;

            cameraTurnAnimationData = keyValues.cameraTurnAnimationData;

            keySequenceData = keyValues.keySequenceData;

        }

        public void KeyTransitionInput() {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
