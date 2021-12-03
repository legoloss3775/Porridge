using NodeEditorFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_Frame : FrameEditor
{
    public static void FrameEditing() {
        ShowFrameData();
    }
    public static void ShowFrameData() {
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

        FrameKeyTransitionSelection();

        GUILayout.EndVertical();
    }
    public static void FrameKeyTransitionSelection() {
        var keyT = FrameManager.frame.currentKey;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Режим перехода");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        keyT.transitionType = (FrameKey.TransitionType)EditorGUILayout.EnumPopup(keyT.transitionType);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        switch (keyT.transitionType) {
            case FrameKey.TransitionType.DialogueAnswerSelection:
                foreach (var dialogue in FrameManager.frameElements.Where(ch => ch is FrameUI_Dialogue)) {
                    ChangeActiveState(keyT, dialogue, false);
                }
                foreach (var dialogueAnswer in FrameManager.frameElements.Where(ch => ch is FrameUI_DialogueAnswer)) {
                    //ChangeActiveState(keyT, dialogueAnswer, true);
                }
                break;
            case FrameKey.TransitionType.DialogueLineContinue:
                foreach (var dialogueAnswer in FrameManager.frameElements.Where(ch => ch is FrameUI_DialogueAnswer)) {
                    ChangeActiveState(keyT, dialogueAnswer, false);
                }
                foreach (var dialogue in FrameManager.frameElements.Where(ch => ch is FrameUI_Dialogue)) {
                    //ChangeActiveState(keyT, dialogue, true);
                }
                break;
        }

    }
    public static void FrameKeySelection() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.MaxWidth(25))) {
            var key = new FrameKey();
            FrameManager.frame.AddKey(key);
            foreach (var element in FrameManager.frameElements) {
                try {
                    FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 1].AddFrameKeyValues(element.id, FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 2].frameKeyValues[element.id]);
                }
                catch (System.Exception) {
                    FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.Count - 1].AddFrameKeyValues(element.id, element.GetFrameKeyValuesType());
                }
                if (element is IInteractable) {
                    KeyNode node = (KeyNode)NodeEditorFramework.NodeEditor.curNodeCanvas.nodes[key.nodeIndex];
                    var knob = node.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
                    knob.SetPosition(node.oldPos + node.oldPosOffset);
                    knob.SetValue<FrameKey>(node.frameKey);
                    node.oldPos = knob.sidePosition;

                    node.frameKey.dialogueOutputKnobs.Add(element.id, node.outputKnobs.IndexOf(knob));
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
