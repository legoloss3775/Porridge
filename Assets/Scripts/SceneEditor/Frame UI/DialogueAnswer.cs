using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.Serialization;
using FrameCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FrameCore {
    /// <summary>
    /// Для сериализации параметров
    /// <see cref="FrameElement">
    /// </summary>
    namespace Serialization {
        #region SERIALIZATION

        [Serializable]
        public class DialogueAnswerValues : Values {
            public KeySequenceData keySequenceData;
            public DialogueAnswerTextData dialogueAnswerTextData;
            public DialogueAnswerValues(DialogueAnswer dialogue) {
                keySequenceData = new KeySequenceData {
                    nextKeyID = dialogue.nextKeyID,
                    previousKeyID = dialogue.previousKeyID,
                };
                transformData = new TransformData {
                    position = dialogue.position,
                    activeStatus = dialogue.activeStatus,
                    size = dialogue.size,
                };
                dialogueAnswerTextData = new DialogueAnswerTextData {
                    text = dialogue.text,
                };
            }
            public DialogueAnswerValues() { }
            [Serializable]
            public struct SerializedDialogueAnswerValues {
                public TransformData transformData;
                public KeySequenceData keySequenceData;
                public DialogueAnswerTextData dialogueAnswerTextData;
            }
            [SerializeField]
            public SerializedDialogueAnswerValues serializedDialogueAnswerValues {
                get {
                    return new SerializedDialogueAnswerValues {
                        dialogueAnswerTextData = dialogueAnswerTextData,
                        keySequenceData = keySequenceData,
                        transformData = transformData,
                    };
                }
            }
            public static void LoadSerialzedDialogueValues(List<SerializedDialogueAnswerValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new DialogueAnswerValues {
                        dialogueAnswerTextData = svalue.dialogueAnswerTextData,
                        keySequenceData = svalue.keySequenceData,
                        transformData = svalue.transformData,
                    });
                }
            }
        }
        #endregion
    }
    namespace UI {
        public class DialogueAnswer : Window, IKeyTransition {
            public override Vector2 position {
                get {
                    try {
                        return this.GetComponent<RectTransform>().anchoredPosition;
                    }
                    catch (System.Exception) {
                        if (this != null)
                            return this.gameObject.transform.position;
                        else
                            return frameElementObject.prefab.transform.position;
                    }
                }
                set {
                    GetComponent<RectTransform>().anchoredPosition = value;
                }
            }
            public override Vector2 size {
                get {
                    try {
                        return RectTransformExtensions.GetSize(this.GetComponent<RectTransform>());
                    }
                    catch (System.Exception) {
                        if (this != null)
                            return RectTransformExtensions.GetSize(this.GetComponent<RectTransform>());
                        else
                            return RectTransformExtensions.GetSize(frameElementObject.prefab.GetComponent<RectTransform>());
                    }
                }
                set {
                    RectTransformExtensions.SetWidth(this.GetComponent<RectTransform>(), value.x);
                    RectTransformExtensions.SetHeight(this.GetComponent<RectTransform>(), value.y);
                }
            }
            public int nextKeyID { get; set; }
            public int previousKeyID { get; set; }
            public string text {
                get {
                    return GetTextComponent().text;
                }
                set {
                    GetTextComponent().text = value;
                }
            }

            private Button button;

            private void Start() {
                button = GetComponent<Button>();
                KeyTransitionInput();
            }
            private void Update() {
                if (FrameController.INPUT_BLOCK) button.enabled = false;
                else button.enabled = true;
            }
            public void KeyTransitionInput() => button.onClick.AddListener(() => FrameManager.SetKey(nextKeyID));

            #region VALUES_SETTINGS
            public TextMeshProUGUI GetTextComponent() {
                if (this != null)
                    return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            }
            public override Values GetFrameKeyValuesType() {
                return new DialogueAnswerValues(this);
            }
            public override void UpdateValuesFromKey(Values frameKeyValues) {
                var keyValues = (DialogueAnswerValues)frameKeyValues;

                nextKeyID = keyValues.keySequenceData.nextKeyID;
                previousKeyID = keyValues.keySequenceData.previousKeyID;
                activeStatus = keyValues.transformData.activeStatus;
                position = keyValues.transformData.position;
                size = keyValues.transformData.size;
                text = keyValues.dialogueAnswerTextData.text;
            }
            #endregion

            #region EDITOR
#if UNITY_EDITOR
            [CustomEditor(typeof(DialogueAnswer))]
            [CanEditMultipleObjects]
            public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector {
                public override void OnInspectorGUI() {
                    DialogueAnswer answer = (DialogueAnswer)target;
                    EditorUtility.SetDirty(answer);

                    answer.position = answer.GetComponent<RectTransform>().anchoredPosition;
                    answer.size = answer.GetComponent<RectTransform>().sizeDelta;

                    answer.SetKeyValuesWhileNotInPlayMode();    
                    if (targets.Length > 1) {
                        foreach (var target in targets) {
                            FrameElement mTarget = (FrameElement)target;
                            answer.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                            answer.size = answer.GetComponent<RectTransform>().sizeDelta;
                            mTarget.SetKeyValuesWhileNotInPlayMode();
                        }
                    }

                    this.SetElementInInspector<DialogueAnswerSO>();
                }
            }
#endif
            #endregion
        }
    }
}
