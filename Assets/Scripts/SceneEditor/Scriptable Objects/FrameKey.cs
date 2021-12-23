
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace FrameCore {
    [Serializable]
    public class FrameKey {
        public int id;
        public int nodeIndex { get; set; }

        public SerializableDictionary<string, int> frameKeyTransitionKnobs = new SerializableDictionary<string, int>();

        public delegate void KeyListner();
        public event KeyListner onFrameKeyUpdate;

        public TransitionType transitionType;
        public GameType gameType;
        public string gameManagerID;
        public FrameKeyDictionary frameKeyValues = new FrameKeyDictionary();

        public enum TransitionType {
            DialogueLineContinue,
            DialogueAnswerSelection,
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
    [Serializable]
    public abstract class Values {
        public Serialization.TransformData transformData;
        public static T GetObject<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }
}

