using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
public class FrameEditor_Character : FrameEditor_Element
{
    public static void FrameCharacterEditing() {
        Action<FrameCharacter> action = FrameCharacterStateEditing;
        ElementEditing<FrameCharacterSO, FrameCharacter>(action);
    }
    private static void FrameCharacterStateEditing(FrameCharacter character) {
        var keyValues = character.GetFrameKeyValues<FrameCharacterValues>();
        var characterSO = (FrameCharacterSO)character.frameElementObject;

        keyValues.emotionState = (FrameCharacterSO.CharacterEmotionState)EditorGUILayout.EnumPopup("Настроение:", keyValues.emotionState);

        var characterParts = new List<CharacterPart>();
        foreach (var part in characterSO.characterParts.Where(ch => ch.state == keyValues.emotionState))
            characterParts.Add(part);

        if(characterParts.Count > 0)
            CharacterPartPrefabSelection(characterParts);

        void CharacterPartPrefabSelection(List<CharacterPart> parts) {
            var names = new List<string>();
            foreach(var part in parts) {
                names.Add(part.statePrefab.name);
            }

            keyValues.selectedPartIndex = EditorGUILayout.Popup(keyValues.selectedPartIndex, names.ToArray());
            var selectedPart = parts[keyValues.selectedPartIndex];
            if (character.emotionState != keyValues.emotionState || character.selectedPartIndex != keyValues.selectedPartIndex) {
                character.selectedPartIndex = keyValues.selectedPartIndex;
                character.CharacterPartChange(selectedPart, keyValues.emotionState);
            }
        }
    }
}
#endif