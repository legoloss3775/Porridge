using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameCore {
    [RequireComponent(typeof(FrameManager))]
    public class FrameController : MonoBehaviour {
        public static bool INPUT_BLOCK;
        public enum InputType {
            ButtonClick,
            KeyInput,
        }
        public FrameManager manager;
        public static SerializableDictionary<string, bool> animations = new SerializableDictionary<string, bool>();

        private void Update() {
            if (animations.Count == 0) INPUT_BLOCK = false;
        }
        public static void AddAnimationToQueue(string key, bool value) {
            if (key == null) return;
            if (!animations.ContainsKey(key)) {
                animations.Add(key, value);
            }
        }
        public static void RemoveAnimationFromQueue(string key) {
            if (key == null) return;
            if (animations.ContainsKey(key)) {
                animations.Remove(key);
            }
        }
    }
}
