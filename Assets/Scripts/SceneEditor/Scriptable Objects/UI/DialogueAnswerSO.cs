using UnityEngine;

namespace FrameCore {
    namespace ScriptableObjects {
        namespace UI {
            [CreateAssetMenu(fileName = "Dialogue Answer", menuName = "Редактор Сцен/UI/Диалоговый ответ", order = 0)]
            public class DialogueAnswerSO : WindowSO {
                public string text { get { return GetText(); } set { SetText(value); } }

                public override void OnAfterDeserialize() {
                    base.OnAfterDeserialize();
                }

                public override void OnBeforeSerialize() {
                    base.OnBeforeSerialize();
                }
                public override void OnEnable() {
                    base.OnEnable();
                }
                public override Vector2 GetPrefabPosition() {
                    return prefab.GetComponent<RectTransform>().anchoredPosition;
                }
                public override Vector2 GetPrefabSize() {
                    return prefab.GetComponent<RectTransform>().sizeDelta;
                }
                private void SetText(string value) {
                    try {
                        prefab.GetComponent<TMPro.TMP_Text>().text = value;
                    }
                    catch (System.NullReferenceException) {
                        Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
                    }
                }
                private string GetText() {
                    try {
                        return prefab.GetComponent<TMPro.TMP_Text>().text;
                    }
                    catch (System.NullReferenceException) {
                        Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
                        return "Отсутсвует текст";
                    }
                }
            }
        }
    }
}

