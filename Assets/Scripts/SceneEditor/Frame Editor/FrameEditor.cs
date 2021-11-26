using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

#if UNITY_EDITOR 
public static class FrameGUIUtility {
    internal static int hotControl;

    public static void GuiLine(int i_height = 1) {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

    }
}
public abstract class FrameEditor : Editor
{
    #region VALUES
    public enum EditorType {
        DialogueEditor,
        CharacterEditor,
        BackgroundEditor,
    }
    public static SerializableDictionary<EditorType, Vector2> scrollPositions = new SerializableDictionary<EditorType, Vector2> {
        { EditorType.DialogueEditor, new Vector2()},
        { EditorType.CharacterEditor, new Vector2()},
        { EditorType.BackgroundEditor, new Vector2()},
    };
    public static SerializableDictionary<EditorType, string> searchTexts = new SerializableDictionary<EditorType, string> {
        { EditorType.DialogueEditor, ""},
        { EditorType.CharacterEditor, ""},
        { EditorType.BackgroundEditor, ""},
    };
    public static SerializableDictionary<EditorType, bool> foldouts = new SerializableDictionary<EditorType, bool> {
        { EditorType.DialogueEditor, true},
        { EditorType.CharacterEditor, true},
        { EditorType.BackgroundEditor, true},
    };
    public enum PositioningType {
        Horizontal,
        Vertical,
    }
    #endregion
    public static void ElementEditing<TElementSO, TElement>(PositioningType posType, EditorType editorType, bool scrollEnabled = false, bool searchEnabled = false, params Action<TElement>[] action)
    where TElementSO : FrameElementSO
    where TElement : FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        switch (posType) {
            case PositioningType.Horizontal: {

                if(scrollEnabled)
                    scrollPositions[editorType] = GUILayout.BeginScrollView(scrollPositions[editorType]);

                if (searchEnabled) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Поиск: ", GUILayout.MaxWidth(275));
                    GUILayout.FlexibleSpace();
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(200));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO)) {
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj).ToList()) {

                            if(searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                Debug.Log(elementID);
                                continue;
                            }

                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for(int i = 0; i < action.Length; i++) {
                                    action[i](element);
                                }
                            }
                            else {
                                //TODO: else?
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                if(scrollEnabled)
                    GUILayout.EndScrollView();

                break;
            }
            case PositioningType.Vertical: {

                if (scrollEnabled)
                    scrollPositions[editorType] = GUILayout.BeginScrollView(scrollPositions[editorType]);

                if (searchEnabled) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Поиск: ", GUILayout.MaxWidth(75));
                    GUILayout.FlexibleSpace();
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(150));
                    GUILayout.EndHorizontal();
                    FrameGUIUtility.GuiLine();
                }

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO))
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj).ToList()) {

                            if (searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                continue;
                            }
                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for (int i = 0; i < action.Length; i++) {
                                    action[i](element);
                                }
                                if(FrameManager.frame.usedElementsObjects.Where(ch => ch.elementObject is TElementSO).Count() > 1 ) {
                                    FrameGUIUtility.GuiLine();
                                }
                            }
                            else {
                                //TODO: else?
                            }
                        }
                    }
                //GUILayout.FlexibleSpace();
                if(scrollEnabled)
                    GUILayout.EndScrollView();

                break;
            }
        }
    }
    public static void ElementCreation(FrameEditor_CreationWindow.CreationType creationType) {
        if (creationType == FrameEditor_CreationWindow.CreationType.Frame) {
            if (GUILayout.Button("Выбрать фрейм", GUILayout.MaxWidth(282.5f))) {
                var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
                editor.type = creationType;
                editor.ShowPopup();
            }
        }
        else {
            if (GUILayout.Button("+", GUILayout.MaxWidth(25))) {
                var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
                editor.type = creationType;
                editor.ShowPopup();
            }
        }
    }
    public static void ElementSelection<TElement>(TElement element)
    where TElement : FrameElement {
        if(GUILayout.Button("•", GUILayout.MaxWidth(25))) {
            Selection.activeObject = element.gameObject;
            GameObject activeGO = Selection.activeGameObject;
            SceneView.lastActiveSceneView.rotation = activeGO.transform.rotation;

            Vector3 position = activeGO.transform.position + (activeGO.transform.forward * 10);
            SceneView.lastActiveSceneView.pivot = position;

            SceneView.lastActiveSceneView.Repaint();
        }
    }
    public static void ElementDeletion<TElement>(TElement element)
        where TElement: FrameElement {
        if (GUILayout.Button("X", GUILayout.MaxWidth(25))) {
            FrameManager.frame.RemoveElementFromCurrentKey(element.id);
        }
    }
}
public class FrameEditor_CreationWindow : EditorWindow {
    public enum CreationType {
        Frame,
        FrameCharacter,
        FrameDialogue,
        FrameBackground,
    }
    public CreationType type;
    public static string createdElementID;
    public string searchText;
    public Vector2 scroll;
    private void OnEnable() {
        createdElementID = "";
    }
    private void OnGUI() {
        switch (type) {
            case CreationType.FrameCharacter:
                FrameElementCreationSelection<FrameCharacterSO, FrameCharacter>();
                break;
            case CreationType.FrameDialogue:
                FrameElementCreationSelection<FrameUI_DialogueSO, FrameUI_Dialogue>();
                break;
            case CreationType.FrameBackground:
                FrameElementCreationSelection<FrameBackgroundSO, FrameBackground>();
                break;
            case CreationType.Frame:
                FrameSelection();
                break;
        }
    }
    private void FrameElementCreationSelection<TKey, TValue>()
    where TKey : global::FrameElementSO
    where TValue : global::FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Поиск:", GUILayout.Width(50));
        searchText = GUILayout.TextArea(searchText, GUILayout.MaxWidth(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        scroll = GUILayout.BeginScrollView(scroll);
        foreach (var elementObject in frameEditorSO.frameElementsObjects.FindAll(el => el is TKey)) {

            if (searchText != null && searchText != "") {
                if (!elementObject.name.Split('_')[0].Contains(searchText))
                    continue;
            }

            GUILayout.BeginHorizontal();
            var icon = UnityEditor.AssetPreview.GetAssetPreview(elementObject.prefab);
            GUILayout.BeginVertical();
            if (GUILayout.Button(icon, GUILayout.MaxWidth(200))) {
                elementObject.CreateElementOnScene<TValue>(elementObject, Vector2.zero, out string id);
                createdElementID = id;
                FrameManager.ChangeFrameKey();
                Debug.Log(id);
                Close();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(75);
            GUILayout.Label(elementObject.name);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
    }
    private void FrameSelection() {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        List<FrameSO> frames = new List<FrameSO>();
        List<string> frameNames = new List<string>();

        if (AssetManager.GetFrameAssets().Length == 0) {
            CreateFrame();
        }
        if (GUILayout.Button("Создать новый фрейм", GUILayout.MaxWidth(250)))
            CreateFrame();

        foreach (var frame in AssetManager.GetFrameAssets()) {
            frames.Add(frame);
            frameNames.Add(frame.name);
        }
        frameEditorSO.selectedFrameIndex = GUILayout.SelectionGrid(frameEditorSO.selectedFrameIndex, frameNames.ToArray(), 3);
        for (int i = 0; i < frames.Count; i++) {
            if (i == frameEditorSO.selectedFrameIndex && FrameManager.frame != frames[i]) {
                FrameManager.frame = frames[i];

                FrameManager.ChangeFrame();
                FrameManager.ChangeFrameKey();
                Close();
            }
        }
    }
    private void CreateFrame() {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

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
}
#endif