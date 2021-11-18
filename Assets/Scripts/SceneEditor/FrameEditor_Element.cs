using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
#if UNITY_EDITOR 
public abstract class FrameEditor_Element : Editor
{
    public static void ElementEditing<TElementSO, TElement>(Action<TElement> action)
    where TElementSO : FrameElementSO
    where TElement : FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach(var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO))
            if(FrameManager.frame.ContainsFrameElementObject((TElementSO)obj))
                foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj)) {
                    var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                    if (element == null) {
                        continue;
                    }

                    EditorUtility.SetDirty(element);

                    if (!Application.isPlaying) {
                        action(element);
                    }
                    else {

                    }
                }
    }
}
#endif