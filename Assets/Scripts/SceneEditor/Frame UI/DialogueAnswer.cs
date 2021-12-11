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
        public class DialogueAnswerValues : Values, IFrameUI_DialogueAnswerSerialization {
            public int nextKeyID { get; set; }
            public int previousKeyID { get; set; }
            public string text { get; set; }
            public DialogueAnswerValues(DialogueAnswer dialogue) {
                nextKeyID = dialogue.nextKeyID;
                previousKeyID = dialogue.previousKeyID;
                position = dialogue.position;
                activeStatus = dialogue.activeStatus;
                size = dialogue.size;
                text = dialogue.text;
            }
            public DialogueAnswerValues() { }
            [Serializable]
            public struct SerializedDialogueAnswerValues : IFrameUI_DialogueAnswerSerialization {
                [SerializeField]
                private int _nextKeyID;
                [SerializeField]
                private int _previousKeyID;
                [SerializeField]
                private Vector2 _position;
                [SerializeField]
                private bool _activeStatus;
                [SerializeField]
                private Vector2 _size;
                [SerializeField]
                private string _text;

                public int nextKeyID { get => _nextKeyID; set => _nextKeyID = value; }
                public int previousKeyID { get => _previousKeyID; set => _previousKeyID = value; }
                public Vector2 position { get => _position; set => _position = value; }
                public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
                public Vector2 size { get => _size; set => _size = value; }
                public string text { get => _text; set => _text = value; }
            }
            [SerializeField]
            public SerializedDialogueAnswerValues serializedDialogueAnswerValues;

            public void SetSerializedDialogueValues() {
                serializedDialogueAnswerValues.nextKeyID = nextKeyID;
                serializedDialogueAnswerValues.previousKeyID = previousKeyID;
                serializedDialogueAnswerValues.text = text;
                serializedDialogueAnswerValues.position = position;
                serializedDialogueAnswerValues.activeStatus = activeStatus;
                serializedDialogueAnswerValues.size = size;
            }
            public static void LoadSerialzedDialogueValues(List<SerializedDialogueAnswerValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new DialogueAnswerValues {
                        nextKeyID = svalue.nextKeyID,
                        previousKeyID = svalue.previousKeyID,
                        position = svalue.position,
                        activeStatus = svalue.activeStatus,
                        size = svalue.size,
                        text = svalue.text,
                    });
                }
            }
        }
        #endregion
        public interface IFrameUI_DialogueAnswerSerialization {
            Vector2 position { get; set; }
            bool activeStatus { get; set; }
            Vector2 size { get; set; }
            string text { get; set; }
        }
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

                nextKeyID = keyValues.nextKeyID;
                previousKeyID = keyValues.previousKeyID;
                activeStatus = keyValues.activeStatus;
                position = keyValues.position;
                size = keyValues.size;
                text = keyValues.text;
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
