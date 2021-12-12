using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Serialization {
    [System.Serializable]
    public static class FrameData {
        

    }
    [System.Serializable]
    public struct KeySequenceData {
        public int nextKeyID;
        public int previousKeyID;
    }
    [System.Serializable]
    public struct TransformData {
        public Vector2 position;
        public Vector2 size;
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
    }
    [System.Serializable]
    public struct DialogueAnswerTextData {
        public string text;
    }
    [System.Serializable]
    public struct FrameEffectData {
        public float animationSpeed;
    }
}
