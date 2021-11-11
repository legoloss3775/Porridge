using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Window", menuName = "Редактор Сцен/UI/Диалоговое окно", order = 0)]
public class FrameUIDialogueSO : FrameUIWindowSO
{
    public enum DialogueWindowType
    {
        CharacterLine,
        PlayerAnswer
    }
    public DialogueWindowType type;
    public string text { get { return GetText(); } set { SetText(value); } }

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize()
    {
        base.OnBeforeSerialize();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    private void SetText(string value)
    {
        try
        {
            prefab.GetComponent<TMPro.TMP_Text>().text = value;
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
        }
    }
    private string GetText()
    {
        try
        {
            return prefab.GetComponent<TMPro.TMP_Text>().text;
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("Отсутствует компонент текста в префабе " + prefab.name);
            return "Отсутсвует текст";
        }
    }
}
