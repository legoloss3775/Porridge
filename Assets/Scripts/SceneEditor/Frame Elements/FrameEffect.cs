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
            public CameraTurnAnimationData cameraTurnAnimationData;
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
            }
            public FrameEffectValues() { }
            [Serializable]
            public struct SerializedFrameEffectValues {
                public TransformData transformData;
                public FrameEffectData frameEffectData;
                public CameraTurnAnimationData cameraTurnAnimationData;
            }
            [SerializeField]
            public SerializedFrameEffectValues serializedFrameEffectValues {
                get {
                    return new SerializedFrameEffectValues {
                        transformData = transformData,
                        frameEffectData = frameEffectData,
                        cameraTurnAnimationData = cameraTurnAnimationData,
                    };
                }
            }
            public static void LoadSerializedFrameEffectValues (List<SerializedFrameEffectValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new FrameEffectValues {
                        transformData = svalue.transformData,
                        frameEffectData = svalue.frameEffectData,
                        cameraTurnAnimationData = svalue.cameraTurnAnimationData,
                    }
                    );
                }
            }
        }
    }
    public class FrameEffect : FrameElement {
        public float animationSpeed = 1f;
        public float animationDelay = 0;
        public Serialization.CameraTurnAnimationData cameraTurnAnimationData;

        public override void OnKeyChanged() {

            if (GetComponent<FrameEffects.BlackScreenFadeout>() != null && activeStatus != false) {
                var blackoutScreenFadeout = GetComponent<FrameEffects.BlackScreenFadeout>();
                Color objectColor = blackoutScreenFadeout.GetComponent<SpriteRenderer>().color;
                if (blackoutScreenFadeout.toBlack)
                    blackoutScreenFadeout.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
                else
                    blackoutScreenFadeout.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);

                FrameController.AddAnimationToQueue(blackoutScreenFadeout.gameObject.name, true);
                blackoutScreenFadeout.StartCoroutine(blackoutScreenFadeout.FadeBlackOut(blackoutScreenFadeout.toBlack, blackoutScreenFadeout.speed));
            }
            if (GetComponent<FrameEffects.CameraTurn>() != null && activeStatus != false) {
                var cameraTurn = GetComponent<FrameEffects.CameraTurn>();

                cameraTurn.rotation = Camera.main.transform.rotation.eulerAngles;
                FrameController.AddAnimationToQueue(cameraTurn.gameObject.name, true);
                cameraTurn.StartCoroutine(cameraTurn.TurnCamera(cameraTurn.degreesX, cameraTurn.degreesY, cameraTurn.speed));

            }
            if(GetComponent<FrameEffects.CameraMove>() != null && activeStatus != false) {
                var cameraMove = GetComponent<FrameEffects.CameraMove>();

                //cameraMove.moveToPosition = Camera.main.transform.position;
                FrameController.AddAnimationToQueue(cameraMove.gameObject.name, true);
                cameraMove.StartCoroutine(cameraMove.MoveCamera(cameraMove.moveToPosition, cameraMove.speed));
            }
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

        }
        #endregion
    }
}
