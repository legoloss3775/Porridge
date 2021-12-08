﻿using System;
using UnityEditor;
using UnityEngine;
using static FrameSO;

[CreateAssetMenu(fileName = "Dialogue Window", menuName = "Редактор Сцен/UI/Диалоговое окно", order = 0)]
public class FrameUI_DialogueSO : FrameUI_WindowSO {
    public string text { get { return GetText(); } set { SetText(value); } }

    public override void OnAfterDeserialize() {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize() {
        base.OnBeforeSerialize();
    }
    public override void OnEnable() {
        base.OnEnable();
    }
    private void SetText(string value) {
        try {
            prefab.GetComponent<TMPro.TMP_Text>().text = value;
        }
        catch (System.NullReferenceException) {
            Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
        }
    }
    private string GetText() {
        try {
            return prefab.GetComponent<TMPro.TMP_Text>().text;
        }
        catch (System.NullReferenceException) {
            Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
            return "Отсутсвует текст";
        }
    }
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, FrameKey.Values values) {
        T elementClone = Instantiate(pair.elementObject.prefab, FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;

#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);

        var createdDialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(elementClone.id);
        createdDialogue.conversationCharacters = new SerializableDictionary<string, string>();
    }
    public override void CreateFrameElement<T>(FrameElementSO obj, Vector2 position, Vector2 size, out T elementClone) {
        elementClone = Instantiate(obj.prefab, position, new Quaternion(), FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.size = size;
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        foreach (var key in FrameManager.frame.frameKeys) {
            key.AddFrameKeyValues(elementClone.id, elementClone.GetFrameKeyValuesType());
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);

        var createdDialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(elementClone.id);
        createdDialogue.conversationCharacters = new SerializableDictionary<string, string>();
        foreach(var key in FrameManager.frame.frameKeys) {
            var values = (FrameUI_DialogueValues)key.frameKeyValues[createdDialogue.id];
            values.conversationCharacters = new SerializableDictionary<string, string>();
        }
    }

}
