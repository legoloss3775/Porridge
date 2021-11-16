using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameSO;

[CreateAssetMenu(fileName = "Character", menuName = "Редактор Сцен/Персонаж")]
public class FrameCharacterSO : FrameElementSO {
    public enum CharacterEmotionState {
        Default,
        Angry,
        Sad,
        Thinking,
        Ashamed
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
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, FrameKey.Values values) {
        var cValues = (FrameCharacterValues)values;
        switch (cValues.type) {
            case FrameCharacter.CharacterType.Standalone: {
                T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
                elementClone.frameElementObject = pair.elementObject;
                elementClone.id = id;
                EditorUtility.SetDirty(elementClone);
                FrameManager.AddElement(elementClone);
                return;
            }
            case FrameCharacter.CharacterType.Conversation: {
                var dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(cValues.dialogueID);
                var dialogueValues = (FrameUI_DialogueValues)FrameManager.frame.currentKey.frameKeyValues[cValues.dialogueID];
                foreach (var character in dialogueValues.conversationCharacters)
                    if (character.Key == pair.elementObject.id) {
                        T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
                        elementClone.frameElementObject = pair.elementObject;
                        elementClone.id = id;
                        EditorUtility.SetDirty(elementClone);
                        FrameManager.AddElement(elementClone);

                        SetCharacterInDialogue(dialogue);
                    }
                break;
            }
        }

        void SetCharacterInDialogue(FrameUI_Dialogue dialogue) {
            var dialogueValues = (FrameUI_DialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];
            switch (dialogue.type) {
                case FrameUI_Dialogue.FrameDialogueElementType.OneCharacter: {
                    if (dialogue != null && dialogue.currentConversationCharacter != null) dialogue.RemovePreviousCharacterOnScene();

                    dialogue.currentConversationCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                    dialogue.conversationCharacterID = id;
                    break;
                }
                case FrameUI_Dialogue.FrameDialogueElementType.MultibleCharacters: {
                    if (dialogueValues.conversationCharacterID == id) {
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
