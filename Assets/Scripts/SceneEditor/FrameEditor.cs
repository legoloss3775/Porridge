using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FrameEditor : EditorWindow {

    public FrameEditorSO frameEditorSO;
    public FrameManager manager { get; set; }

    public static readonly Vector2 DEFAULT_ELEMENT_POSITION = Vector2.zero;
    public static readonly bool DEFAULT_ELEMENT_ACTIVESTATE = true;

    [MenuItem("Window/Редактор фрейма")]
    static void Init() {
        FrameEditor frameEditor = (FrameEditor)EditorWindow.GetWindow(typeof(FrameEditor), false, "Редактор фрейма");
        frameEditor.Show();
    }
    private void OnGUI() {

        if (!isEditingAllowed())
            if (EditorUtility.DisplayDialog("Редактор фрейма", "Начать создание фрейма на данной сцене?", "Да", "Отмена")) {
                UpdateFrameEditorSO();
                UpdateFrameManager();
                SetFrame();
            }
        if (isEditingAllowed()) {
            AssetManager.UpdateAssets();
            UpdateDirty();

            FrameSelection();
            FrameKeySelection();

            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();

            GUILayout.Label("Создать персонажа на сцене", GUILayout.Width(200));
            FrameElementCreationSelection<FrameCharacterSO, FrameCharacter>();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();

            GUILayout.Label("Создать диалог на сцене", GUILayout.Width(200));
            FrameElementCreationSelection<FrameUI_DialogueSO, FrameUI_Dialogue>();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            FrameEditor_Dialogue.FrameDialogueEditing();
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
    private void FrameElementCreationSelection<TKey, TValue>()
        where TKey : global::FrameElementSO
        where TValue : global::FrameElement {
        foreach (var elementObject in frameEditorSO.frameElementsObjects.FindAll(el => el is TKey)) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(elementObject.name, GUILayout.Width(200))) {
                elementObject.CreateElementOnScene<TValue>(elementObject, Vector2.zero, out string id);
                FrameManager.ChangeFrameKey();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
    }
    private void FrameKeySelection() {
        if (GUILayout.Button("Новый кадр")) {
            FrameManager.frame.AddKey(new FrameKey());
            foreach (var element in FrameManager.frameElements) {
                FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 1].AddFrameKeyValues(element.id, FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 2].frameKeyValues[element.id]);
            } 
        }
        List<string> keyStrings = new List<string>();
        foreach (var key in FrameManager.frame.frameKeys)
            keyStrings.Add(FrameManager.frame.frameKeys.IndexOf(key).ToString());

        FrameManager.frame.selectedKeyIndex = GUILayout.SelectionGrid(FrameManager.frame.selectedKeyIndex, keyStrings.ToArray(), 8);

        foreach (var key in FrameManager.frame.frameKeys) {
            if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                if (key != null) {
                    FrameManager.frame.currentKey = key;
                    FrameManager.ChangeFrameKey();
                }
            }
        }
    }
    private void SetFrame() {
        List<FrameSO> frames = new List<FrameSO>();
        List<string> frameNames = new List<string>();

        if (AssetManager.GetFrameAssets().Length == 0) {
            CreateFrame();
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
    private void FrameSelection() {
        List<FrameSO> frames = new List<FrameSO>();
        List<string> frameNames = new List<string>();

        if (AssetManager.GetFrameAssets().Length == 0) {
            CreateFrame();
        }
        if (GUILayout.Button("Создать новый фрейм"))
            CreateFrame();

        foreach (var frame in AssetManager.GetFrameAssets()) {
            frames.Add(frame);
            frameNames.Add(frame.name);
        }
        frameEditorSO.selectedFrameIndex = GUILayout.SelectionGrid(frameEditorSO.selectedFrameIndex, frameNames.ToArray(), 8);
        for (int i = 0; i < frames.Count; i++) {
            if (i == frameEditorSO.selectedFrameIndex && FrameManager.frame != frames[i]) {
                FrameManager.frame = frames[i];

                FrameManager.ChangeFrame();
                FrameManager.ChangeFrameKey();
            }
        }
    }
    private void CreateFrame() {
        int count = AssetManager.GetAtPath<FrameSO>("Frames/").Length;
        string path = "Assets/Frames/Frame " + count + ".asset";
        FrameSO frame = ScriptableObject.CreateInstance<FrameSO>();
        frame.selectedKeyIndex = 0;
        AssetDatabase.CreateAsset(frame, path);
        frame.id = "Frame_" + count;
        FrameManager.frame = frame;
        frameEditorSO.selectedFrameIndex = AssetManager.GetFrameAssets().Length - 1;


        FrameManager.ChangeFrame();
        FrameManager.ChangeFrameKey();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

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
        FrameManager.UICanvas.sortingOrder = 1;

        var go = new GameObject("Event System", typeof(EventSystem));
        var e = go.GetComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
    #endregion

}
