using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class AssetManager {
    public static void UpdateAssets() {
        UpdateFrameAssetsOfType<FrameCharacterSO>("Characters");
        UpdateFrameAssetsOfType<FrameBackgroundSO>("Backgrounds");
        UpdateFrameAssetsOfType<FrameUI_DialogueSO>("UI/Dialogue Windows");
        UpdateFrames();
    }
    public static void UpdateFrames() {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        List<FrameSO> list = new List<FrameSO>();
        foreach (var obj in frameEditorSO.frames.FindAll(ch => ch is FrameSO)) {
            list.Add(obj);
        }
        list = list.Except(
            AssetManager.GetAtPath<FrameSO>("Frames/")
            )
            .ToList();
        if (list.Count > 0) {
            foreach (var obj in list)
                frameEditorSO.frames.Remove(obj);
        }
        for (int i = 0; i < frameEditorSO.frames.ToList().Count; i++) {
            if (frameEditorSO.frames[i] == null) {
                frameEditorSO.frames.RemoveAt(i);
            }
        }
        for (int i = 1; i < frameEditorSO.frames.ToList().Count; i++) {
            if (frameEditorSO.frames[i].name == frameEditorSO.frames[i - 1].name)
                frameEditorSO.frames.RemoveAt(i);
        }
    }
    public static void UpdateFrameAssetsOfType<T>(string folderName)
        where T : FrameElementSO{
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        List<T> list = new List<T>();
        foreach (var obj in frameEditorSO.frameElementsObjects.FindAll(ch => ch is T)) {
            list.Add(obj as T);
        }
        list = list.Except(
            AssetManager.GetAtPath<T>("Frames/" + folderName + "/")
            )
            .ToList();
        if (list.Count > 0) {
            foreach (var obj in list)
                frameEditorSO.frameElementsObjects.Remove(obj);
        }
        for (int i = 0; i < frameEditorSO.frameElementsObjects.ToList().Count; i++) {
            if (frameEditorSO.frameElementsObjects[i] == null)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
        }
        for (int i = 1; i < frameEditorSO.frameElementsObjects.ToList().Count; i++) {
            if (frameEditorSO.frameElementsObjects[i].name == frameEditorSO.frameElementsObjects[i - 1].name)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
        }
    }
    public static FrameSO[] GetFrameAssets() {
        return AssetManager.GetAtPath<FrameSO>("Frames/");
    }
    public static T[] GetAtPath<T>(string path) {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string fileName in fileEntries) {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += fileName.Substring(index);

            UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];


        return result;
    }
}
#endif
[Serializable]
public class FrameKeyDictionary : Dictionary<string, FrameKey.Values>, ISerializationCallbackReceiver {
    [SerializeField]
    private List<string> keys = new List<string>();

    [SerializeField]
    private List<FrameKey.Values> values = new List<FrameKey.Values>();

    [SerializeField]
    private List<FrameElementValues.SerializedElementValues> serializedElementValues =
        new List<FrameElementValues.SerializedElementValues>();
    [SerializeField]
    private List<FrameUI_DialogueValues.SerializedDialogueValues> serializedDialogueValues =
        new List<FrameUI_DialogueValues.SerializedDialogueValues>();
    [SerializeField]
    private List<FrameCharacterValues.SerializedFrameCharacterValues> serializedFrameCharacterValues =
        new List<FrameCharacterValues.SerializedFrameCharacterValues>();
    public virtual void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();
        serializedElementValues.Clear();
        serializedDialogueValues.Clear();
        serializedFrameCharacterValues.Clear();

        foreach (var pair in this.Where(ch => ch.Value is FrameElementValues)) {
            FrameElementValues _sev = (FrameElementValues)pair.Value;
            keys.Add(pair.Key);
            serializedElementValues.Add(_sev.SetSerializedFrameKeyElementValues());
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameUI_DialogueValues)) {
            FrameUI_DialogueValues _sdv = (FrameUI_DialogueValues)pair.Value;
            keys.Add(pair.Key);
            serializedDialogueValues.Add(_sdv.SetSerializedDialogueValues());
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameCharacterValues)) {
            FrameCharacterValues _schv = (FrameCharacterValues)pair.Value;
            keys.Add(pair.Key);
            serializedFrameCharacterValues.Add(_schv.SetSerializedFrameCharacterValues());
        }
    }

    public virtual void OnAfterDeserialize() {
        this.Clear();
        FrameElementValues
            .LoadSerialzedFrameKeyElementValues(serializedElementValues, values);
        FrameUI_DialogueValues
            .LoadSerialzedDialogueValues(serializedDialogueValues, values);
        FrameCharacterValues
            .LoadSerialzedFrameKeyCharacterElementValues(serializedFrameCharacterValues, values);


        for (int i = 0; i < keys.Count; i++) {
            this.Add(keys[i], values[i]);
        }
    }
}
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this) {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize() {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("oh shit, here we go again"));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}

