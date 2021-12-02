using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_FrameKeу : FrameEditor
{
    public static void ShowFrameKeyData(FrameKey key) {
        var dialogueValues = key.frameKeyValues.Values.Where(ch => ch is FrameUI_DialogueValues);
        var answerValues = key.frameKeyValues.Values.Where(ch => ch is FrameUI_DialogueAnswerValues);

        GUILayout.Label(key.keySequence.previousKey?.id.ToString());

        if(dialogueValues != null)
            foreach(var element in dialogueValues) {
                FrameUI_DialogueValues dialogue = (FrameUI_DialogueValues)element;
                GUILayout.TextArea(dialogue?.text);
            }
        if (answerValues != null)
            foreach (var element in answerValues) {
                FrameUI_DialogueAnswerValues dialogue = (FrameUI_DialogueAnswerValues)element;
                GUILayout.TextArea(dialogue?.text);
            }
    }
}
#endif
