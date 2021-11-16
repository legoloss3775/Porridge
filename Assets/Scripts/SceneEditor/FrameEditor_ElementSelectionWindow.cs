using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FrameEditor_ElementSelectionWindow : EditorWindow 
{
    public FrameElementSO selectedFrameElementSO;
    public void ElementSelection<T>(T _selectedFrameElementSO)
        where T: FrameElementSO
    {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var obj in frameEditorSO.GetFrameElementsOfType<T>())
        {
            if (GUILayout.Button(obj.name))
            {
                selectedFrameElementSO = obj;
                this.Close();
                return;
            }
        }
    }
}
