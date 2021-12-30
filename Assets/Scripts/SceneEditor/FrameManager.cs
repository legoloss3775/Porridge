using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameCore {
    public enum GameType {
        FrameInteraction,
        Cutscene,
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

        public static GameObject frameContainer;
        public static GameObject UICanvasContainer;

        public delegate void FrameListner();
        public static event FrameListner onFrameKeyChanged;
        public static event FrameListner onFrameChanged;

        //
        /**public static List<GameFramework.GameManager> gameManagers = new List<GameFramework.GameManager>();
        public List<GameFramework.GameManager> _gameManagers = new List<GameFramework.GameManager>();**/
            //
        private void Awake() {
            assetDatabase = _assetDatabase;
            UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
            frameContainer = GameObject.Find("Frame");
            UICanvasContainer = GameObject.Find("UI");

            UICanvas.worldCamera = Camera.main;

            // SetFrame(0, 0);
            //SetFrame(assetDatabase.selectedFrameIndex, assetDatabase.selectedKeyIndex);
            /**foreach (var effect in frameElements.Where(ch => ch is FrameCore.FrameEffect)) {
                foreach (var child in effect.GetComponentsInChildren<SpriteRenderer>()) {
                    child.enabled = true;
                }
            }**/
        }
        private void Start() {
        }

        private void Update() {
            if (UICanvas.worldCamera != Camera.main)
                UICanvas.worldCamera = Camera.main;

            foreach(var flag in FrameKey.frameCoreFlags.keys) {
                Debug.Log(flag + " " + FrameKey.frameCoreFlags.GetValue(flag).ToString());
            }
        }
        public static void SetKey(int keyIndex) {
            if (frame.frameKeys.Count > keyIndex &&
                keyIndex >= 0) {
                frame.currentKey = frame.frameKeys[keyIndex];

                switch (frame.currentKey.gameType) {
                    case GameType.FrameInteraction:
                        switch (frame.currentKey.keyType) {
                            case FrameKey.KeyType.Default:
                                if (frame.currentKey.cutscene != null) Destroy(frame.currentKey.cutscene);
                                frameContainer.SetActive(true);

                                ChangeFrameKey();

                                break;
                            case FrameKey.KeyType.FlagChange:
                                if (frame.currentKey.cutscene != null) Destroy(frame.currentKey.cutscene);
                                frameContainer.SetActive(true);

                                FrameKey.UpdateGlobalFlags(frame.currentKey.flagData);

                                frame.currentKey = frame.frameKeys[frame.currentKey.flagSequenceData.nextKeyID];

                                ChangeFrameKey();

                                break;
                            case FrameKey.KeyType.FlagCheck:
                                if (frame.currentKey.cutscene != null) Destroy(frame.currentKey.cutscene);
                                frameContainer.SetActive(true);

                                if (FrameKey.CheckGlobalFlags(frame.currentKey.flagData)) {
                                    frame.currentKey = frame.frameKeys[frame.currentKey.flagNextKeyID[0]];

                                    ChangeFrameKey();
                                }
                                else {
                                    frame.currentKey = frame.frameKeys[frame.currentKey.flagNextKeyID[1]];

                                    ChangeFrameKey();
                                }

                                break;
                        }
                        break;
                    case GameType.Cutscene:
                        frameContainer.SetActive(false);
                        frame.currentKey.cutscene = Instantiate(frame.currentKey.cutscenePrefab);
                        break;
                    case GameType.InnerFireFastSession:
                        break;
                    case GameType.InnerFireLongSession:
                        break;
                    case GameType.InnerFireFreeRoam:
                        break;
                    case GameType.Custom:
                        break;
                }

                /**switch (frame.currentKey.gameType) {
                    case GameType.FrameInteraction:
                        foreach(var gameManager in gameManagers) {
                            gameManager.gameObject.SetActive(false);
                        }
                        break;
                    case GameType.InnerFireFastSession:
                        GetGameManager<GameFramework.InnerFire.FastSessionManager>().gameObject.SetActive(true);
                        frame.currentKey.gameManagerID = GetGameManager<GameFramework.InnerFire.FastSessionManager>().id;
                        break;
                    case GameType.InnerFireLongSession:
                        break;
                    case GameType.InnerFireFreeRoam:
                        break;
                    case GameType.Custom:
                        break;
                }**/
            }
        }
        public static void SetFrame(int frameIndex, int keyIndex) {

            UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
            UICanvas.worldCamera = Camera.main;

            if (assetDatabase.frames.Count > frameIndex &&
                frameIndex >= 0) {
                var sortedFrames = from frame in assetDatabase.frames
                                   orderby frame.name
                                   select frame;

                frame = sortedFrames.ToArray()[frameIndex];

                if (keyIndex > frame.frameKeys.Count)
                    frame.selectedKeyIndex = keyIndex;
                else frame.selectedKeyIndex = 0;

                ChangeFrame();
                if (keyIndex < frame.frameKeys.Count)
                    SetKey(keyIndex);
                else SetKey(0);
            }
        }
        public void SetDefaultFrame() => SetFrame(0, 0);
        /**public static GameFramework.GameManager GetGameManager<T>()
            where T: GameFramework.GameManager {

            return (T)gameManagers.Where(ch => ch is T).FirstOrDefault();
        }**/
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
                    if (!Application.isPlaying) {
                        DestroyImmediate(frameElements[i].gameObject);
                        frameElements.RemoveAt(i);
                    }
                    else {
                        Destroy(frameElements[i].gameObject);
                        frameElements.RemoveAt(i);
                    }
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
            onFrameKeyChanged = null;
            foreach (var element in frameElements) {
                onFrameKeyChanged += element.OnKeyChanged;
            }

            if(Application.isPlaying)
                onFrameKeyChanged?.Invoke();
        }
        //TODO: switch gametype?/
        public static void ChangeFrame() {
            ClearElements();
            FrameSO.LoadElementsOnScene<FrameCameraSO, FrameCamera>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<FrameEffectSO, FrameEffect>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<BackgroundSO, Background>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<DialogueSO, Dialogue>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<DialogueAnswerSO, DialogueAnswer>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<CharacterSO, Character>(frame.usedElementsObjects);
            FrameSO.LoadElementsOnScene<FrameLightSO, FrameLight>(frame.usedElementsObjects);

#if UNITY_EDITOR
            if (Application.isPlaying) return;
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
#endif//
        //
        public void OnBeforeSerialize() {
            serializedFrameElementsList.Clear();
            //_gameManagers.Clear();
            foreach (var element in frameElements) {
                if (element != null)
                    serializedFrameElementsList.Add(element);
            }
            /** foreach(var manager in gameManagers) {
                 if (manager != null)
                     _gameManagers.Add(manager);
             }**/
        }
        public void OnAfterDeserialize() {
            frameElements.Clear();
            //  gameManagers.Clear();
            foreach (var element in serializedFrameElementsList) {

                if (element != null)
                    frameElements.Add(element);

            }
            /**foreach(var manager in _gameManagers) {
                if (manager != null)
                    gameManagers.Add(manager);
            }**/
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
