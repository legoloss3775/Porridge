using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_FrameKeу : FrameEditor
{
    public static void ShowFrameKeyData(FrameKey key) {
        var dialogueValues = (FrameUI_DialogueValues)key.frameKeyValues.Where(ch => ch.Value is FrameUI_DialogueValues).FirstOrDefault().Value;

        var chValues = new List<FrameCharacterValues>();
        var chKeys = new List<string>();
        foreach (var value in key.frameKeyValues.Where(ch => ch.Value is FrameCharacterValues)) {
            chValues.Add((FrameCharacterValues)value.Value);
            chKeys.Add(value.Key);
        }
        //
        switch (dialogueValues?.state) {
            case FrameUI_Dialogue.FrameDialogueState.CharacterLine:
                GUILayout.TextArea(dialogueValues.text);
                break;
            case FrameUI_Dialogue.FrameDialogueState.PlayerAnswer:
                break;
        }

        for(int i = 0; i < chKeys.Count; i++) {
            FrameGUIUtility.GuiLine();
            GUILayout.BeginHorizontal();

            if (dialogueValues != null && dialogueValues.type == FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж
                && chKeys[i] != dialogueValues.conversationCharacterID) {
                GUILayout.EndHorizontal();
                continue;
            }

            GUILayout.Label(chKeys[i]);
            GUILayout.Label(chValues[i].emotionState.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            FrameGUIUtility.GuiLine();
        }
    }
}
#endif
