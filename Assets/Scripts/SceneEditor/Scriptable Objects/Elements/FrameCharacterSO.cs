using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameSO;

[CreateAssetMenu(fileName = "Character", menuName = "Редактор Сцен/Персонаж")]
public class FrameCharacterSO : FrameElementSO {
    [Serializable]
    public enum CharacterEmotionState {
        Обычный = 0,
        Злой = 1,
        Грустный = 2,
        Задумался = 3,
        Стыднобыть = 4
    }

    [HideInInspector]
    public CharacterEmotionState state;

    [OneLine.OneLine]
    public List<CharacterPart> characterParts;

    public override void OnAfterDeserialize() {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize() {
        base.OnBeforeSerialize();
    }
    public override void OnEnable() {
        base.OnEnable();
    }
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, FrameKey.Values keyValues) {
        var characterKeyValues = (FrameCharacterValues)keyValues;
        switch (characterKeyValues.type) {
            case FrameCharacter.CharacterType.Standalone: {
                T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
                elementClone.frameElementObject = pair.elementObject;
                elementClone.id = id;
#if UNITY_EDITOR
                EditorUtility.SetDirty(elementClone);
#endif
                FrameManager.AddElement(elementClone);
                return;
            }
            case FrameCharacter.CharacterType.Conversation: {
                var dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(characterKeyValues.dialogueID);
                var dialogueValues = (FrameUI_DialogueValues)FrameManager.frame.currentKey.frameKeyValues[characterKeyValues.dialogueID];
                foreach (var character in dialogueValues.conversationCharacters)
                    if (character.Key == pair.elementObject.id) {
                        T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
                        elementClone.frameElementObject = pair.elementObject;
                        elementClone.id = id;
#if UNITY_EDITOR
                        EditorUtility.SetDirty(elementClone);
#endif
                        FrameManager.AddElement(elementClone);

                        SetCharacterInDialogue(dialogue);
                    }
                break;
            }
        }

        void SetCharacterInDialogue(FrameUI_Dialogue dialogue) {
            var dialogueKeyValues = (FrameUI_DialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];
            switch (dialogue.type) {
                case FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
                    if (dialogue != null && dialogue.currentConversationCharacter != null) dialogue.RemovePreviousCharacterOnScene();

                    dialogue.currentConversationCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                    dialogue.conversationCharacterID = id;
                    break;
                }
                case FrameUI_Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
                    if (dialogueKeyValues.conversationCharacterID == id) {
                        dialogue.currentConversationCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                        dialogue.conversationCharacterID = id;
                    }
                    break;
                }
            }
        }
    }
}
[System.Serializable]
public class CharacterPart {
    public GameObject statePrefab;
    public FrameCharacterSO.CharacterEmotionState state;
}
