using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

public class FrameEditor_Window : EditorWindow {

    public FrameEditorSO frameEditorSO;
    public FrameManager manager { get; set; }

    public bool isEditing { get; set; }
    public Vector2 scroll;
    public static readonly Vector2 DEFAULT_ELEMENT_POSITION = Vector2.zero;
    public static readonly bool DEFAULT_ELEMENT_ACTIVESTATE = true;

    [MenuItem("Window/Frame Editor")]
    static void Init() {
        FrameEditor_Window frameEditor = (FrameEditor_Window)EditorWindow.GetWindow(typeof(FrameEditor_Window), false, "Frame Editor");
        frameEditor.Show();
    }
    private void OnDisable() {
        NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.AssureCanvas();
        SaveFrameEditorNodeCanvas();
    }
    private void OnGUI() {
        if (!isEditingAllowed()) {
            UpdateFrameEditorSO();
            UpdateFrameManager();
            SetFrame();
        }
        if(manager._assetDatabase == null || FrameManager.assetDatabase == null) {
            manager._assetDatabase = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
            FrameManager.assetDatabase = manager._assetDatabase;
        }
        if(manager.GetComponent<FrameController>() == null) {
            FrameController controller = manager.gameObject.AddComponent<FrameController>();
            controller.manager = manager;
        }
         if (isEditingAllowed()) {
            AssetManager.UpdateAssets();
            UpdateDirty();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            scroll = GUILayout.BeginScrollView(scroll,false, true);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            FrameEditor_Frame.FrameEditing();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            FrameEditor_Background.FrameBackgroundEditing();
            FrameEditor_Dialogue.FrameDialogueEditing();
            FrameEditor_Character.FrameCharacterEditing();

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (FrameEditor_CreationWindow.createdElementID != "")
                FrameEditor_CreationWindow.createdElementID = "";
        }
    }
    public void SaveFrameEditorNodeCanvas() {
        if (FrameManager.frame != null && FrameManager.frame.nodeCanvas != null) {
            if (NodeEditorWindow.editor != null && NodeEditorWindow.editor.canvasCache != null) {
                if (!EditorApplication.isCompiling) {
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
            manager == null
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
            FrameEditor_CreationWindow.CreateFrame();
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
        if (manager == null) {
            if (GameObject.Find("Frame Manager") != null) {
                manager = GameObject.Find(
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
                    .GetComponent<Canvas>();
            else
                SetUICanvas();
        }

    }
    private void SetFrameManager() {
        manager = new GameObject(
            "Frame Manager",
            typeof(FrameManager)
            )
            .GetComponent<FrameManager>();
        FrameManager.assetDatabase = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
    }
    private void SetUICanvas() {
        FrameManager.UICanvas = new GameObject(
            "UI Canvas",
            typeof(Canvas)
            )
            .GetComponent<Canvas>();

        var scaler = FrameManager.UICanvas.gameObject.AddComponent<CanvasScaler>();
        FrameManager.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        FrameManager.UICanvas.worldCamera = Camera.main;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1;
        FrameManager.UICanvas.sortingOrder = 100;

        var go = new GameObject("Event System", typeof(EventSystem));
        var e = go.GetComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
    #endregion

}
#endif
