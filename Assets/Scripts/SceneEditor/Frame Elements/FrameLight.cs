using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static FrameCore.FrameElement;

namespace FrameCore {
    namespace Serialization {
        public class FrameLightValues : Values {
            public FrameLightData lightData;
            public FrameLightValues(FrameLight light) {
                transformData = new TransformData {
                    position = light.position,
                    activeStatus = light.activeStatus,
                    rotation = light.rotation.eulerAngles,
                    size = light.size,
                };
                lightData = new FrameLightData {
                    intensity = light.lightData.intensity,
                    outerRange = light.lightData.outerRange,
                    innerRange = light.lightData.innerRange,
                    color = light.lightData.color,
                    outerAngle = light.lightData.outerAngle,
                    innerAngle = light.lightData.innerAngle,
                };
            }
            public FrameLightValues() { }
            [System.Serializable]
            public struct SerializedFrameLightValues {
                public TransformData transformData;
                public FrameLightData lightData;
            }
            [SerializeField]
            public SerializedFrameLightValues serializedFrameLightValues {
                get {
                    return new SerializedFrameLightValues {
                        transformData = transformData,
                        lightData = lightData,
                    };
                }
            }
            public static void LoadSerializedFrameLightValues(List<SerializedFrameLightValues> serializedFrameLightValues, List<Values> values) {
                foreach (var svalue in serializedFrameLightValues) {
                    values.Add(new FrameLightValues {
                        transformData = svalue.transformData,
                        lightData = svalue.lightData,
                    }
                    );
                }
            }
        }
    }
    public class FrameLight : FrameElement {
        public Serialization.FrameLightData lightData {
            get {
                return GetLightData();
            }
            set {
                component.intensity = value.intensity;
                component.pointLightOuterRadius = value.outerRange;
                component.pointLightInnerRadius = value.innerRange;
                component.color = value.color;
                component.pointLightOuterAngle = value.outerAngle;
                component.pointLightInnerAngle = value.innerAngle;
            }
        }
        public Light2D component { get { return this.gameObject.GetComponent<Light2D>(); } }

        #region LIGHT_SETUP
        public Serialization.FrameLightData GetLightData() {
            return new Serialization.FrameLightData {
                intensity = component.intensity,
                outerRange = component.pointLightOuterRadius,
                innerRange = component.pointLightInnerRadius,
                color = component.color,
                outerAngle = component.pointLightOuterAngle,
                innerAngle = component.pointLightInnerAngle,
            };
        }
        #endregion
        #region VALUES_SETTINGS
        public override Values GetFrameKeyValuesType() {
            return new Serialization.FrameLightValues(this);
        }
        public override void UpdateValuesFromKey(Values frameKeyValues) {
            var keyValues = (Serialization.FrameLightValues)frameKeyValues;
            activeStatus = keyValues.transformData.activeStatus;
            position = keyValues.transformData.position;
            rotation = Quaternion.Euler(keyValues.transformData.rotation);
            size = keyValues.transformData.size;

            lightData = keyValues.lightData;
        }
        #endregion
    }
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameLight))]
    [CanEditMultipleObjects]
    public class FrameLightCustomInspecotr : Editor {
        public override void OnInspectorGUI() {
            FrameLight light = (FrameLight)target;
            var keyValues = GetFrameKeyValues<Serialization.FrameLightValues>(light.id);

            if (keyValues != null) {
                light.position = light.gameObject.transform.position;
                light.size = light.gameObject.transform.localScale;
                light.rotation = light.gameObject.transform.rotation;

                light.lightData = light.GetLightData();

                light.SetKeyValuesWhileNotInPlayMode();

                if (targets.Length > 1) {
                    foreach (var target in targets) {
                        FrameLight mTarget = (FrameLight)target;
                        mTarget.position = mTarget.gameObject.transform.position;
                        mTarget.size = mTarget.gameObject.transform.localScale;
                        mTarget.rotation = mTarget.gameObject.transform.rotation;
                        mTarget.SetKeyValuesWhileNotInPlayMode();
                    }
                }
            }
        }
    }
#endif
    #endregion
}
