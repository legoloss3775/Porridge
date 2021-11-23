using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FrameEditor_CreationWindow : EditorWindow{
    public enum CreationType {
        FrameCharacter,
        FrameDialogue,
        FrameBackground,
    }
    public CreationType type;
    public static string createdElementID;
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
        }
    }
    private void FrameElementCreationSelection<TKey, TValue>()
    where TKey : global::FrameElementSO
    where TValue : global::FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var elementObject in frameEditorSO.frameElementsObjects.FindAll(el => el is TKey)) {
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
        GUILayout.FlexibleSpace();
    }
}
public class FrameEditor_FrameData : FrameEditor_Element
{
    public static void FrameEditing() {
        ElementCreation(FrameEditor_CreationWindow.CreationType.FrameCharacter);
    }
    public static void ElementCreation(FrameEditor_CreationWindow.CreationType creationType){
        if(GUILayout.Button("+", GUILayout.MaxWidth(25))) {
            var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
            editor.type = creationType;
            editor.ShowPopup();
        }
    }
    public static void ShowFrameData(FrameSO frame) {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();


        FrameKeySelection();
    }
    public static void FrameKeySelection() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.MaxWidth(25))) {
            FrameManager.frame.AddKey(new FrameKey());
            foreach (var element in FrameManager.frameElements) {
                try {
                    FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 1].AddFrameKeyValues(element.id, FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 2].frameKeyValues[element.id]);
                }
                catch (System.Exception) {
                    FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 1].AddFrameKeyValues(element.id, element.GetFrameKeyValuesType());
                }

            }
        }
        List<string> keyStrings = new List<string>();
        foreach (var key in FrameManager.frame.frameKeys)
            keyStrings.Add(FrameManager.frame.frameKeys.IndexOf(key).ToString());

        FrameManager.frame.selectedKeyIndex = GUILayout.SelectionGrid(FrameManager.frame.selectedKeyIndex, keyStrings.ToArray(), 8, GUILayout.MaxWidth(25));

        foreach (var key in FrameManager.frame.frameKeys) {
            if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                if (key != null) {
                    FrameManager.frame.currentKey = key;
                    FrameManager.ChangeFrameKey();
                }
            }
        }
        GUILayout.EndHorizontal();
    }
}
