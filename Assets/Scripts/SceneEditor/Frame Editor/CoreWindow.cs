using FrameCore;
using FrameCore.ScriptableObjects;
using GameFramework;
using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс главного окна FrameEditor
    /// </summary>
    public class CoreWindow : EditorWindow {
        public static CoreWindow core { get { return (CoreWindow)EditorWindow.GetWindow(typeof(CoreWindow)); } }

        public FrameEditorSO frameEditorSO;
        public FrameManager frameManager { get; set; }
        public GameManager gameManager { get; set; }

        public SerializableDictionary<string, Editor> lightEditor = new SerializableDictionary<string, Editor>();

        public bool isEditing { get; set; }
        public Vector2 scroll;

        [MenuItem("Window/Frame Editor")]
        static void Init() {
            CoreWindow frameEditor = (CoreWindow)EditorWindow.GetWindow(typeof(CoreWindow), false, "Frame Editor");
            frameEditor.Show();
        }
        private void OnDisable() {
            NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.AssureCanvas();
            SaveFrameEditorNodeCanvas();

            foreach (var editor in lightEditor) {
                if (editor.Value != null)
                    DestroyImmediate(editor.Value);
            }
        }
        private void OnEnable() {
            foreach (var editor in lightEditor) {
                if (editor.Value != null)
                    DestroyImmediate(editor.Value);
            }
        }

        private void OnGUI() {
            if (EditorApplication.isCompiling) return;
            if (Application.isPlaying) {
                return;
            }
            else {
                if (focusedWindow == this) NodeEditorWindow.editor.ShowTab();
            }
            if (GameObject.Find("Frame Manager") == null && !EditorApplication.isCompiling) {
                //if (!EditorUtility.DisplayDialog("Редактор фрейма", "Начать редактирование фрейма на этой сцене?", "Да", "Отмена")) {
                //Close();
                //NodeEditorWindow.editor.Close();
                //return;
                //}
            }

            if (!isEditingAllowed()) {
                UpdateFrameEditorSO();
                UpdateFrameManager();
                UpdateGameManager();
                AssetManager.UpdateAssets();
                SetFrame();
            }
            if(FrameKey.frameCoreFlags.keys == null || FrameKey.frameCoreFlags.values == null) {
                SaveManager.LoadFlagsFile();
            }
            if (frameManager._assetDatabase == null || FrameManager.assetDatabase == null) {
                frameManager._assetDatabase = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
                FrameManager.assetDatabase = frameManager._assetDatabase;
            }
            if (frameManager.GetComponent<FrameController>() == null) {
                FrameController controller = frameManager.gameObject.AddComponent<FrameController>();
                controller.manager = frameManager;
            }
            if (isEditingAllowed()) {
                UpdateDirty();

                if (FrameManager.frame == null || FrameManager.frame.currentKey == null) return;

                UpdateFrameContainerChilds();

                //if(FrameManager.frame.nodeCanvas == null || FrameManager.frame.nodeCanvas.name != FrameManager.frame.id)
                //  FrameManager.frame.nodeCanvas = AssetManager.GetAtPath<NodeCanvas>("Frames/NodeCanvases/").Where(ch => ch.canvasName == "Canvas_" + FrameManager.frame.id).FirstOrDefault();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                scroll = GUILayout.BeginScrollView(scroll, false, true);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                Frame.FrameEditing();

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                switch (FrameManager.frame.currentKey.keyType) {
                    case FrameKey.KeyType.Default:

                        FrameCamera.FrameCameraEditing();

                        FrameEffect.FrameEffectEditing();

                        FrameLight.FrameLightEditing();

                        Background.FrameBackgroundEditing();

                        Dialogue.FrameDialogueEditing();

                        Character.FrameCharacterEditing();

                        break;
                    case FrameKey.KeyType.FlagChange:
                        break;
                    case FrameKey.KeyType.FlagCheck:
                        break;
                        /**case GameType.InnerFireFastSession:
if(FrameManager.GetGameManager<FastSessionManager>() != null )
  fastSessionManager = FrameManager.GetGameManager<FastSessionManager>();
GUILayout.FlexibleSpace();
GUILayout.BeginHorizontal();
GUILayout.FlexibleSpace();
GUILayout.Label("Игровой менеджер:", FrameGUIUtility.GetLabelStyle(Color.cyan, 25));
GUILayout.FlexibleSpace();
GUILayout.EndHorizontal();
GUILayout.BeginHorizontal();
GUILayout.FlexibleSpace();
fastSessionManager = (FastSessionManager)EditorGUILayout.ObjectField(fastSessionManager, typeof(FastSessionManager), true, GUILayout.MaxWidth(300));

if (fastSessionManager != null)
  FrameManager.frame.currentKey.gameManagerID = fastSessionManager.id;
if (!manager._gameManagers.Contains(fastSessionManager)) manager._gameManagers.Add(fastSessionManager);
if (!FrameManager.gameManagers.Contains(fastSessionManager)) FrameManager.gameManagers.Add(fastSessionManager);
GUILayout.FlexibleSpace();
GUILayout.EndHorizontal();
GUILayout.FlexibleSpace();
break;
case GameType.InnerFireLongSession:
break;
case GameType.InnerFireFreeRoam:
break;**/
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (CreationWindow.createdElementID != "")
                    CreationWindow.createdElementID = "";

                //DisableEffectsInEditor();
            }
        }
        public void DisableEffectsInEditor() {
            foreach (var effect in FrameManager.frameElements.Where(ch => ch is FrameCore.FrameEffect)) {
                foreach (var child in effect.GetComponentsInChildren<SpriteRenderer>()) {
                    child.enabled = false;
                }
            }
        }
        public void SaveFrameEditorNodeCanvas() {
            if (FrameManager.frame != null && FrameManager.frame.nodeCanvas != null) {
                if (NodeEditorWindow.editor != null && NodeEditorWindow.editor.canvasCache != null) {
                    if (!EditorApplication.isCompiling) {
                        if (Application.isPlaying) return;
                        NodeEditorWindow.editor.canvasCache.SaveNodeCanvas("Assets/Frames/NodeCanvases/Canvas_" + FrameManager.frame.id + ".asset");
                        NodeEditor.BeginEditingCanvas(FrameManager.frame.nodeCanvas);

                    }
                }
            }
        }
        public void UpdateDirty() {
            if (!EditorUtility.IsDirty(frameEditorSO)) {
                EditorUtility.SetDirty(frameEditorSO);
            }
            if (FrameManager.frame != null) {
                if (!EditorUtility.IsDirty(FrameManager.frame)) {
                    EditorUtility.SetDirty(FrameManager.frame);
                }
            }
        }
        public bool isEditingAllowed() {
            if (
                frameManager == null
                || FrameManager.UICanvas == null
                || frameEditorSO == null
                )
                return false;
            else return true;
        }

        #region FRAME_EDITING
        private void SetFrame() {
            List<FrameSO> frames = new List<FrameSO>();
            List<string> frameNames = new List<string>();

            if (AssetManager.GetFrameAssets().Length == 0) {
                CreationWindow.CreateFrame();
            }

            foreach (var frame in AssetManager.GetFrameAssets()) {
                frames.Add(frame);
                frameNames.Add(frame.name);
            }
            for (int i = 0; i < frames.Count; i++) {
                if (i == frameEditorSO.selectedFrameIndex) {
                    FrameManager.frame = frames[i];

                    FrameManager.ChangeFrame();
                    FrameManager.ChangeFrameKey();
                }
            }
        }
        #endregion
        #region VALUES_SETTINGS
        private void UpdateFrameEditorSO() {
            if (frameEditorSO == null) {
                frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
            }
        }
        private void UpdateFrameManager() {
            if (frameManager == null) {
                if (GameObject.Find("Frame Manager") != null) {
                    frameManager = GameObject.Find(
                        "Frame Manager"
                        )
                        .GetComponent<FrameManager>();
                }
                else
                    SetFrameManager();
            }
            if (FrameManager.UICanvas == null) {
                if (GameObject.Find("UI Canvas") != null)
                    FrameManager.UICanvas = GameObject.Find(
                        "UI Canvas"
                        )
                        .GetComponentInChildren<Canvas>();
                else
                    SetUICanvas();
            }
            if (FrameManager.UICanvasContainer == null) {
                FrameManager.UICanvasContainer = GameObject.Find("UI");
            }
            FrameManager.UICanvas.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            if (FrameManager.frameContainer == null) {
                if (GameObject.Find("Frame") == null)
                    FrameManager.frameContainer = new GameObject("Frame");
                else FrameManager.frameContainer = GameObject.Find("Frame");
            }
        }
        private void UpdateFrameContainerChilds() {
            if (FrameManager.UICanvasContainer != null && FrameManager.UICanvasContainer.transform.parent != FrameManager.frameContainer.transform) {
                FrameManager.UICanvasContainer.transform.SetParent(FrameManager.frameContainer.transform);
            }
            foreach (var element in FrameManager.frameElements) {
                if (element != null && element.gameObject != null && element.gameObject.transform.parent != FrameManager.frameContainer.transform) {
                    if (element is FrameCore.UI.Window) continue;
                    element.gameObject.transform.SetParent(FrameManager.frameContainer.transform);
                }
            }
        }
        private void UpdateGameManager() {
            if (gameManager == null) {
                if (GameObject.Find("Game Manager") != null) {
                    gameManager = GameObject.Find(
                        "Game Manager"
                        )
                        .GetComponent<GameManager>();
                }
                else
                    SetGameManager();
            }
        }
        private void SetGameManager() {
            gameManager = new GameObject(
                "Game Manager",
                typeof(GameManager)
                )
                .GetComponent<GameManager>();
            gameManager.frameManager = frameManager;
        }
        private void SetFrameManager() {
            frameManager = new GameObject(
                "Frame Manager",
                typeof(FrameManager)
                )
                .GetComponent<FrameManager>();
            FrameManager.assetDatabase = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        }
        private void SetUICanvas() {
            FrameManager.UICanvas = Instantiate(AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault().UI_CanvasPrefab).GetComponentInChildren<Canvas>();
            FrameManager.UICanvas.gameObject.transform.parent.name = "UI";
        }
        #endregion

    }
}
#endif
