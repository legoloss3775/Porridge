using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(fileName ="Frame Editor Settings", menuName ="Редактор фрейма/Настройки редактора")]
namespace FrameCore {
    namespace ScriptableObjects {
        public class FrameEditorSO : ScriptableObject {
            [SerializeField]
            public int selectedFrameIndex;
            public int selectedKeyIndex;

            public GameObject UI_CanvasPrefab;

            public List<FrameElementSO> frameElementsObjects = new List<FrameElementSO>();
            public List<FrameSO> frames = new List<FrameSO>();

            public List<string> GetFrameElementsObjectsNames<T>()
                where T : FrameElementSO {
                List<string> names = new List<string>();
                foreach (T obj in frameElementsObjects.Where(ch => ch is T)) {
                    names.Add(obj.name);
                }
                return names;
            }
            public List<T> GetFrameElementsOfType<T>()
            where T : FrameElementSO {
                List<T> frameElementObjects = new List<T>();
                foreach (T obj in frameElementsObjects.Where(ch => ch is T)) {
                    frameElementObjects.Add(obj);
                }
                return frameElementObjects;
            }
        }
    }
}
