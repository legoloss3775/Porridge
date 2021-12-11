using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameCore {
    public enum GameType {
        FrameInteraction,
        InnerFireFastSession,
        InnerFireLongSession,
        InnerFireFreeRoam,
        Custom,
    }
    [System.Serializable]
    public class FrameManager : MonoBehaviour, ISerializationCallbackReceiver {

        public static GameType GAME_TYPE { get { return frame.currentKey.gameType; } }

        public static FrameSO frame;
        public static Canvas UICanvas;
        public Canvas _UICanvas;

        [SerializeField]
        public static List<FrameElement> frameElements = new List<FrameElement>();
        public List<FrameElement> serializedFrameElementsList = new List<FrameElement>();

        public static FrameEditorSO assetDatabase;
        public FrameEditorSO _assetDatabase;

        int selectedFrame = 0;
        int selectedFrameKey = 0;

        private void Awake() {

            /**foreach (var effect in frameElements.Where(ch => ch is FrameCore.FrameEffect)) {
                foreach (var child in effect.GetComponentsInChildren<SpriteRenderer>()) {
                    child.enabled = true;
                }
            }**/
        }
        private void Start() {
            assetDatabase = _assetDatabase;
            SetDefaultFrame();
        }

        public static void SetKey(int keyIndex) {
            if (frame.frameKeys.Count >= keyIndex &&
                keyIndex >= 0) {
                frame.currentKey = frame.frameKeys[keyIndex];
                ChangeFrameKey();
            }
        }
        public static void SetFrame(int frameIndex) {
            if (assetDatabase.frames.Count > frameIndex &&
                frameIndex >= 0) {
                UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
                frame = assetDatabase.frames[frameIndex];

                frame.currentKey = frame.frameKeys[0];

                ChangeFrame();
                ChangeFrameKey();
            }
        }
        public void SetDefaultFrame() {
            if (_assetDatabase.frames.Count > 0) {
                UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
                frame = _assetDatabase.frames[_assetDatabase.selectedFrameIndex];
                frame.currentKey = frame.frameKeys[0];
                ChangeFrame();
                ChangeFrameKey();
            }
        }
        public static T GetFrameElementOnSceneByID<T>(string id)
        where T : FrameElement {
            foreach (var element in FrameManager.frameElements)
                if (element.id == id) {
                    return (T)element;
                }
            var e = frameElements.OfType<T>().Where(ch => ch.id == id).LastOrDefault();
            return e;
        }
        public static bool ContainsElementInManagerByID(string id) {
            foreach (var element in FrameManager.frameElements)
                if (element.id == id)
                    return true;
            return false;
        }
        public static void AddElement(FrameElement element) {
            frameElements.Add(element);
        }
        public static void RemoveElement(string id) {
            for (int i = 0; i < frameElements.Count; i++) {
                if (frameElements[i].id == id) {
                    DestroyImmediate(frameElements[i].gameObject);
                    frameElements.RemoveAt(i);
                }
            }
        }
        public static void ChangeFrameKey() {
            foreach (var element in frameElements.ToList()) {
                if (element == null) continue;

                if (frame.currentKey
                    .frameKeyValues
                    .ContainsKey(element.id)) {

                    if (element is Dialogue dialogue) {
                        dialogue.UpdateValuesFromKey(frame.currentKey.frameKeyValues[element.id]);
                        dialogue.UpdateConversationCharacterFromKey(frame.currentKey.frameKeyValues[element.id]);
                    }
                    else element.UpdateValuesFromKey(frame.currentKey.frameKeyValues[element.id]);
                }
                else frame.currentKey.AddFrameKeyValues(element.id, element.GetFrameKeyValuesType());
            }
        }
        //TODO: switch gametype?
        public static void ChangeFrame() {
            ClearElements();
            FrameSO.LoadElementsOnScene<FrameEffectSO, FrameEffect>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<BackgroundSO, Background>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<DialogueSO, Dialogue>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<DialogueAnswerSO, DialogueAnswer>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<CharacterSO, Character>(frame.usedElementsObjects);

#if UNITY_EDITOR
            NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.AssureCanvas();
            NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.LoadNodeCanvas("Assets/Frames/NodeCanvases/Canvas_" + frame.id + ".asset");
            EditorUtility.SetDirty(frame.nodeCanvas);
#endif
        }
        public static void ClearElements() {
            foreach (var element in frameElements.ToList()) {
                if (element != null) {
                    var go = element.gameObject;
                    DestroyImmediate(element);
                    DestroyImmediate(go);
                }
            }
            frameElements.Clear();
        }
#if UNITY_EDITOR
        public void UpdateDirty() {
            foreach (var element in frameElements)
                if (EditorUtility.IsDirty(element))
                    EditorUtility.ClearDirty(element);
        }
#endif
        public void OnBeforeSerialize() {
            serializedFrameElementsList.Clear();
            foreach (var element in frameElements) {
                if (element != null)
                    serializedFrameElementsList.Add(element);
            }
        }
        public void OnAfterDeserialize() {
            frameElements.Clear();
            foreach (var element in serializedFrameElementsList) {

                if (element != null)
                    frameElements.Add(element);

            }
            UICanvas = _UICanvas;
            assetDatabase = _assetDatabase;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameManager))]
    public class FrameManagerCustomInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var manager = (FrameManager)target;
            FrameManager.assetDatabase = manager._assetDatabase;
            Debug.Log(target.GetInstanceID());
        }
    }
#endif
}
