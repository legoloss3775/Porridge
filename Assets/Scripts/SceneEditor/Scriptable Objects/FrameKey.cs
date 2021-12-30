
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace FrameCore {
    [System.Serializable]
    public class FrameKey {
        public int id;
        public int nodeIndex { get; set; }

        public SerializableDictionary<string, int> frameKeyTransitionKnobs = new SerializableDictionary<string, int>();
        public SerializableDictionary<string, int> frameKeyGlobalFlagKnobs = new SerializableDictionary<string, int>();

        public delegate void KeyListner();
        public event KeyListner onFrameKeyUpdate;

        public TransitionType transitionType;
        public KeyType keyType;
        public GameType gameType;
        public string gameManagerID;
        public FrameKeyDictionary frameKeyValues = new FrameKeyDictionary();

        public int[] flagNextKeyID = new int[2];

        public Serialization.FrameCoreFlags flagData;
        public static Serialization.FrameCoreFlags frameCoreFlags;
        //TODO: при игре в эдиторе запись в файле будет сохраняться даже после выхода из режима игры,
        //из-за чего нужно сделать сброс флагов в эдиторе

        public Serialization.KeySequenceData flagSequenceData;

        public GameObject cutscenePrefab;
        public GameObject cutscene;

        public enum TransitionType {
            DialogueLineContinue,
            DialogueAnswerSelection,
        }
        public enum KeyType {
            Default,
            FlagChange,
            FlagCheck,
        }
        public static bool CheckGlobalFlags(Serialization.FrameCoreFlags flags) {
            foreach (var id in flags.keys) {
                foreach (var global_ID in FrameKey.frameCoreFlags.keys) {
                    if (FrameKey.frameCoreFlags.GetValue(global_ID) == true) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            throw new System.Exception("Не найден флаг");
        }
        public static void UpdateGlobalFlags(Serialization.FrameCoreFlags flags) {
            foreach(var id in flags.keys) {
                foreach(var global_ID in FrameKey.frameCoreFlags.keys) {
                    if(id == global_ID) {
                        FrameKey.frameCoreFlags.SetValue(id, flags.GetValue(id));
                    }
                }
            }
        }
        public FrameKeyDictionary GetFrameKeyValuesOfType<T>()
            where T : Values {
            var exitDictionary = new FrameKeyDictionary();
            foreach (var pair in frameKeyValues) {
                if (pair.Value is T) {
                    exitDictionary.Add(pair.Key, pair.Value);
                }
            }
            return exitDictionary;
        }
        public Values GetFrameKeyValuesOfElement(string id) {
            if (ContainsID(id)) return frameKeyValues[id];
            else return null;
        }
        public void UpdateFrameKeyValues(string id, Values values) {
            if (ContainsID(id)) frameKeyValues[id] = values;
            else AddFrameKeyValues(id, values);

            onFrameKeyUpdate?.Invoke();
/**#if UNITY_EDITOR
            if (NodeEditorFramework.NodeEditor.curNodeCanvas != null && NodeEditorFramework.NodeEditor.curNodeCanvas.nodes.Count > this.nodeIndex) {
                FrameKeyNode node = (FrameKeyNode)NodeEditorFramework.NodeEditor.curNodeCanvas.nodes[this.nodeIndex];
                FrameKeyNode.UpdateKeyNodeValues(node, values, id);
                NodeEditorFramework.Standard.NodeEditorWindow.editor.Repaint();
            }
#endif**/
        }
        public void AddFrameKeyValues(string id, Values values) {
            if(!frameKeyValues.ContainsKey(id))
                if (id != null) frameKeyValues.Add(id, values);
        }
        public bool ContainsID(string id) {
            if (id != null)
                return frameKeyValues.ContainsKey(id);
            else return false;
        }
    }
    [System.Serializable]
    public abstract class Values {
        public Serialization.TransformData transformData;
        public static T GetObject<T>(params object[] args) {
            return (T)System.Activator.CreateInstance(typeof(T), args);
        }
    }
}

