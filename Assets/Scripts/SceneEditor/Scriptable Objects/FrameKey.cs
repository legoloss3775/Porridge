﻿using System;
using System.Diagnostics.Contracts;
using UnityEngine;

[Serializable]
public class FrameKey {
    public int id;
    public int nodeIndex;

    //public KeyNode node;
    public SerializableDictionary<string, int> dialogueOutputKnobs = new SerializableDictionary<string, int>();

    public TransitionType transitionType;
    //public KeySequence keySequence;
    public FrameKeyDictionary frameKeyValues = new FrameKeyDictionary();
    
    [Serializable]
    public struct KeySequence {
        public FrameKey nextKey;
        public FrameKey previousKey;
    }
    public enum TransitionType {
        DialogueLineContinue,
        DialogueAnswerSelection,
    }
    [Serializable]
    public abstract class Values {
        public virtual bool activeStatus { get; set; }
        public virtual Vector2 position { get; set; }
        public static T GetObject<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
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
    }
    public void AddFrameKeyValues(string id, Values values) {
        if (id != null) frameKeyValues.Add(id, values);
    }
    public bool ContainsID(string id) {
        if (id != null)
            return frameKeyValues.ContainsKey(id);
        else return false;
    }
}
