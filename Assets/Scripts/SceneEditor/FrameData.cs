using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Serialization {
    [System.Serializable]
    public static class FrameData {
        

    }
    [System.Serializable]
    public struct FrameCoreFlags {
        public List<string> keys;
        public List<bool> values;

        #region FLAG_METHODS
        public bool ContainsKey(string key) {
            for(int i = 0; i < keys.Count; i++) {
                if (keys[i] == key) {
                    return true;
                }
            }
            return false;
        }
        public bool GetValue(string key) {
            for (int i = 0; i < keys.Count; i++) {
                if (keys[i] == key) {
                    return values[i];
                }
            }
            throw new System.Exception("Значение не найдено");
        }
        public void SetValue(string key, bool value) {
            for (int i = 0; i < keys.Count; i++) {
                if (keys[i] == key) {
                    values[i] = value;
                }
            }
        }
        public void Add(string key, bool value) {
            for (int i = 0; i < keys.Count; i++) {
                if (keys[i] == key) {
                    throw new System.Exception("Ключ уже был добавлен в словарь");
                }
            }
            keys.Add(key);
            values.Add(value);
        }
        public void Remove(string key) {
            for (int i = 0; i < keys.Count; i++) {
                if (keys[i] == key) {
                    keys.Remove(keys[i]);
                    values.Add(values[i]);
                }
            }
        }
        #endregion
    }
    [System.Serializable]
    public struct KeySequenceData {
        public int nextKeyID;
        public int previousKeyID;
    }
    [System.Serializable]
    public struct TransformData {
        public Vector3 position;
        public Vector2 size;
        public Vector3 rotation;
        public bool activeStatus;
    }
    [System.Serializable]
    public struct CharacterData {
        public string dialogueID;
        public Character.CharacterType type;
        public ScriptableObjects.CharacterSO.CharacterEmotionState emotionState;
        public int selectedPartIndex;
    }
    [System.Serializable]
    public struct DialogueTextData {
        public string text;
        public int speakingCharacterIndex;
        public string conversationCharacterID;
        public SerializableDictionary<string, string> conversationCharacters;
        public FrameCore.UI.Dialogue.FrameDialogueElementType type;
        public float textAnimationTime;
        public bool autoContinue;
        public float transitionDelay;
        public float textAnimationDelay;
    }
    [System.Serializable]
    public struct DialogueAnswerTextData {
        public string text;
    }
    [System.Serializable]
    public struct FrameEffectData {
        public float animationSpeed;
        public float animationDelay;
    }
    [System.Serializable]
    public struct CameraTurnAnimationData {
        public float degreesX;
        public float degreesY;
        public Vector3 moveTo;
    }
    [System.Serializable]
    public struct FrameLightData {
        public float intensity;
        public float outerRange;
        public float innerRange;
        public Color color;
        public float outerAngle;
        public float innerAngle;
    }
    [System.Serializable]
    public struct FrameAudioData {
        public float volume;
    }
}
