using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_Frame : FrameEditor
{
    static string frameName;
    public static void FrameEditing() {
        ShowFrameData();
    }
    public static void ShowFrameData() {
        var currentFrame = FrameManager.frame;
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        FrameGUIUtility.GuiLine();
        GUILayout.EndVertical();

        if (AssetManager.GetFrameAssets().Length > 0)
            GUILayout.Label(AssetManager.GetAtPath<FrameSO>("Frames/")[frameEditorSO.selectedFrameIndex].name);

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        FrameGUIUtility.GuiLine();
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        ElementCreation(FrameEditor_CreationWindow.CreationType.Frame);

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        FrameGUIUtility.GuiLine();
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        FrameKeySelection();

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        FrameGUIUtility.GuiLine();

        GUILayout.EndVertical();
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

        FrameManager.frame.selectedKeyIndex = GUILayout.SelectionGrid(FrameManager.frame.selectedKeyIndex, keyStrings.ToArray(), 12, GUILayout.MaxWidth(450));

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
#endif
