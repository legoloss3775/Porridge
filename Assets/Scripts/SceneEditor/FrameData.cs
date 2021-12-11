using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Serialization {
    public static class FrameData {
        
        public struct KeySequenceData {
            public int nextKeyID;
            public int previousKeyID;
        }
        public struct TransformData {
            public Vector2 position;
            public Vector2 size;
            public bool activeStatus;
        }
        public struct TextData {
            public string text;
        }
    }
}
