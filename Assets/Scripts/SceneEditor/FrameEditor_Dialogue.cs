using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public static class FrameEditor_Dialogue 
{
    public static void FrameDialogueEditing()
    {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is FrameUIDialogueSO))
            if (FrameManager.frame.ContainsFrameElementObject(obj))
                foreach (var frameDialogueID in FrameManager.frame.GetFrameElementIDsByObject(obj).ToList())
                {
                    foreach (var pair in FrameManager.frame.currentKey.frameKeyValues.Where(ch => ch.Value is UIDialogueValues).ToList())
                    {
                        if (pair.Key == frameDialogueID)
                        {
                            FrameUIDialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUIDialogue>(frameDialogueID);
                            UIDialogueValues values = (UIDialogueValues)pair.Value;

                            if (!Application.isPlaying)
                            {
                                values.text = GUILayout.TextArea(values.text, GUILayout.MaxHeight(100));
                                if (dialogue != null)
                                {
                                    dialogue.text = values.text;
                                }
                                else
                                    FrameManager.ChangeFrame();
                                FrameUIDialogueCharacterSelection(dialogue);
                                UpdateFrameUIDialogueCharacter(dialogue);
                            }
                            else
                            {
                                dialogue.text = GUILayout.TextArea(dialogue.text, GUILayout.MaxHeight(100));
                            }
                        }
                    }
                }
    }
    public static void FrameUIDialogueCharacterSelection(FrameUIDialogue dialogue)
    {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        List<FrameCharacterSO> frameCharactersSO = new List<FrameCharacterSO>();
        List<string> names = new List<string>();
        foreach (FrameCharacterSO character in frameEditorSO.frameElementsObjects.Where(ch => ch is FrameCharacterSO))
        {
            frameCharactersSO.Add(character);
            names.Add(character.name);
        }

        UIDialogueValues values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];

        values.selectedCharacterIndex = EditorGUILayout.Popup("Собеседник:", values.selectedCharacterIndex, names.ToArray());
        dialogue.selectedCharacterIndex = values.selectedCharacterIndex;
        
        for (int i = 0; i < frameCharactersSO.Count; i++)
        {
            if (i == values.selectedCharacterIndex)
            {
                dialogue.conversationCharacterSO = frameCharactersSO[i];
            }
        }
    }
    public static void UpdateFrameUIDialogueCharacter(FrameUIDialogue dialogue)
    {
        UIDialogueValues values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];

        if (dialogue.conversationCharacterSO != null && values.conversationCharacterID == null)
        {
            FrameCharacter character = new FrameCharacter();
            FrameSO.CreateElementOnScene<FrameCharacter>(dialogue.conversationCharacterSO, character, dialogue.conversationCharacterSO.prefab.transform.position, out string ID);
            character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID);
            character.dialogueID = dialogue.id;

            dialogue.conversationCharacterID = ID;
            dialogue.conversationCharacter = character;
            values.conversationCharacterID = ID;

            character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
            dialogue.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUIDialogue>();

            Debug.Log(dialogue.conversationCharacter.dialogueID);
        }
        if (dialogue.conversationCharacterSO != null && dialogue.conversationCharacterSO != dialogue.conversationCharacter.frameElementObject)
        {
            FrameCharacter newCharacter = new FrameCharacter();
            FrameKey key = FrameManager.frame.currentKey;
            FrameCharacterValues chValues = null;
            if (FrameManager.frame.GetFrameElementIDsByObject(dialogue.conversationCharacterSO) != null)
            {
                foreach (var characterID in FrameManager.frame.GetFrameElementIDsByObject(dialogue.conversationCharacterSO))
                {
                    if (key.ContainsID(characterID))
                        chValues = (FrameCharacterValues)key.frameKeyValues[characterID];
                    if (FrameManager.frame.frameKeys.Count > 1 && chValues == null)
                    {
                        chValues = (FrameCharacterValues)FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.IndexOf(FrameManager.frame.currentKey) - 1].frameKeyValues[characterID];
                        FrameManager.frame.frameKeys[FrameManager.frame.selectedKeyIndex].AddFrameKeyValues(characterID, chValues);
                    }
                    if (chValues != null)
                    {
                        if (chValues.dialogueID == dialogue.id && FrameManager.frame.GetFrameElementObjectByID(characterID) == dialogue.conversationCharacterSO)
                        {
                            FrameManager.RemoveElement(dialogue.conversationCharacter.id);
                            FrameSO.LoadElementOnScene(FrameManager.frame.GetPair(FrameManager.frame.GetFrameElementObjectByID(characterID)), characterID, newCharacter,chValues);
                            newCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(characterID);
                            newCharacter.dialogueID = dialogue.id;
                            dialogue.conversationCharacter = newCharacter;
                            dialogue.conversationCharacterID = characterID;
                            values.conversationCharacterID = characterID;

                            key.UpdateFrameElementKeyValues(newCharacter);

                            newCharacter.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
                            dialogue.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUIDialogue>();

                            Debug.Log(characterID + " " + FrameManager.frame.frameKeys.IndexOf(key));
                            return;
                        }
                    }
                }
            }
            FrameManager.RemoveElement(dialogue.conversationCharacterID);
            FrameCharacter character = new FrameCharacter();
            FrameSO.CreateElementOnScene<FrameCharacter>(dialogue.conversationCharacterSO, character, dialogue.conversationCharacterSO.prefab.transform.position, out string ID);
            character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID);
            character.dialogueID = dialogue.id;
            dialogue.conversationCharacterID = ID;
            dialogue.conversationCharacter = character;
            values.conversationCharacterID = ID;

            character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
            dialogue.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUIDialogue>();
        }
    }
}
