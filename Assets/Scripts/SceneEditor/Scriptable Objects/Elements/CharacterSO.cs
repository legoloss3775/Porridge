using FrameCore.Serialization;
using FrameCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameCore.ScriptableObjects.FrameSO;

namespace FrameCore {
    namespace ScriptableObjects {
        [CreateAssetMenu(fileName = "Character", menuName = "Редактор Сцен/Персонаж")]
        public class CharacterSO : FrameElementSO {
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
            public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, Values keyValues) {
                var characterKeyValues = (CharacterValues)keyValues;
                if (characterKeyValues == null) return;
                switch (characterKeyValues.type) {
                    case Character.CharacterType.Standalone: {
                        T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
                        elementClone.frameElementObject = pair.elementObject;
                        elementClone.id = id;
                        foreach (var key in FrameManager.frame.frameKeys)
                            key.UpdateFrameKeyValues(elementClone.id, elementClone.GetFrameKeyValuesType());
#if UNITY_EDITOR
                        EditorUtility.SetDirty(elementClone);
#endif
                        FrameManager.AddElement(elementClone);
                        return;
                    }
                    case Character.CharacterType.Conversation: {
                        var dialogue = FrameManager.GetFrameElementOnSceneByID<Dialogue>(characterKeyValues.dialogueID);
                        var dialogueValues = (DialogueValues)FrameManager.frame.currentKey.frameKeyValues[characterKeyValues.dialogueID];
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

                void SetCharacterInDialogue(Dialogue dialogue) {
                    var dialogueKeyValues = (DialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];
                    switch (dialogue.type) {
                        case Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
                            if (dialogue != null && dialogue.currentConversationCharacter != null) dialogue.RemovePreviousCharacterOnScene();

                            dialogue.currentConversationCharacter = FrameManager.GetFrameElementOnSceneByID<Character>(id);
                            dialogue.conversationCharacterID = id;
                            break;
                        }
                        case Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
                            if (dialogueKeyValues.conversationCharacterID == id) {
                                dialogue.currentConversationCharacter = FrameManager.GetFrameElementOnSceneByID<Character>(id);
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
            public CharacterSO.CharacterEmotionState state;
        }
    }
}
