using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName ="Frame Editor Settings", menuName ="Редактор фрейма/Настройки редактора")]
public class FrameEditorSO : ScriptableObject
{
    [SerializeField]
    public int selectedFrameIndex;
    public int selectedKeyIndex;

    public  List<FrameElementSO> frameElementsObjects = new List<FrameElementSO>();
}
