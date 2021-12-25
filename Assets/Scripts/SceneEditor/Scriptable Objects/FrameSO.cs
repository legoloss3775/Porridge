using FrameCore.ScriptableObjects.UI;
using FrameCore.Serialization;
using NodeEditorFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameCore {
    namespace ScriptableObjects {
        public class FrameSO : ScriptableObject {
            public enum FrameType {
                Default,
            }
            public FrameType type;

            [SerializeField]
            public string id;
            [SerializeField]
            public int selectedKeyIndex;
            [SerializeField]
            public FrameKey currentKey;
            public List<FrameKey> frameKeys = new List<FrameKey>();
            public NodeCanvas nodeCanvas;
            [System.Serializable]
            public struct FrameElementIDPair {
                public FrameElementSO elementObject;
                public List<string> ids;
            }
            public List<FrameElementIDPair> usedElementsObjects = new List<FrameElementIDPair>();
            private void OnEnable() {
#if UNITY_EDITOR
                FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
                if (frameEditorSO != null && !frameEditorSO.frames.Contains(this))
                    frameEditorSO.frames.Add(this);

#endif

                if (currentKey == null) {
                    if (frameKeys.Count > 0)
                        currentKey = frameKeys[0];
                }
            }
            public void CreateNodeCanvas() {
                nodeCanvas = NodeEditorFramework.NodeCanvas.CreateCanvas<NodeEditorFramework.Standard.CalculationCanvasType>();

                NodeEditorFramework.NodeEditorSaveManager.SaveNodeCanvas("Assets/Frames/NodeCanvases/Canvas_" + id + ".asset", ref nodeCanvas, false);
                NodeEditor.curNodeCanvas = nodeCanvas;
            }
            public void AddKey(FrameKey key) {
                frameKeys.Add(key);
                key.id = frameKeys.IndexOf(key);

                /**#if UNITY_EDITOR
                                FrameKeyNode node = (FrameKeyNode)Node.Create(key.id.ToString(), Vector2.zero);

                                key.nodeIndex = NodeEditorFramework.NodeEditor.curNodeCanvas.nodes.IndexOf(node);
#endif**/
            }
            public List<string> GetAllIDsOfType<T>()
                where T : FrameElementSO {
                var IDs = new List<string>();
                foreach (var pair in usedElementsObjects) {
                    if (pair.elementObject is T)
                        foreach (var id in pair.ids)
                            IDs.Add(id);
                }
                return IDs;
            }
            public FrameElementIDPair GetPair<T>(T frameElementObject)
                where T : FrameElementSO {
                foreach (var pair in usedElementsObjects) {
                    if (pair.elementObject == frameElementObject)
                        return pair;
                }
                return new FrameElementIDPair();
            }
            public bool ContainsFrameElementID(string id) {
                foreach (var pair in usedElementsObjects) {
                    foreach (var pairedID in pair.ids)
                        if (id == pairedID)
                            return true;
                }
                return false;
            }
            public bool ContainsFrameElementObject<T>(T obj)
                where T : FrameElementSO {
                foreach (var pair in usedElementsObjects) {
                    if (pair.elementObject == obj)
                        return true;
                }
                return false;
            }
            public List<string> GetFrameElementIDsByObject<T>(T obj)
                where T : FrameElementSO {
                foreach (var pair in usedElementsObjects) {
                    if (pair.elementObject == obj)
                        return pair.ids;
                }
                return null;
            }
            public FrameElementSO GetFrameElementObjectByID(string id) {
                foreach (var pair in usedElementsObjects) {
                    if (pair.ids.Contains(id))
                        return pair.elementObject;
                }
                return null;
            }
            public void RemoveElementFromCurrentKey(string frameElementID) {
                foreach (var key in frameKeys) {
                    foreach (var value in key.frameKeyValues.ToList()) {
                        if (FrameManager.frame.GetFrameElementObjectByID(frameElementID) is DialogueSO && value.Key == frameElementID) {
                            DialogueValues dialogueValues = (DialogueValues)value.Value;
                            foreach (var character in dialogueValues.dialogueTextData.conversationCharacters.Values) {
                                RemoveElementFromCurrentKey(character);
                            }
                        }
                        if (value.Key == frameElementID)
                            key.frameKeyValues.Remove(value.Key);
                    }
                }
                foreach (var pair in usedElementsObjects.ToList()) {
                    if (pair.ids.Contains(frameElementID)) {
                        pair.ids.Remove(frameElementID);
                        DestroyElementOnScene(frameElementID);
                        if (pair.ids.Count <= 0)
                            usedElementsObjects.Remove(pair);
                    }
                }
            }
            public void DestroyElementOnScene(string frameElementID) {
                FrameElement element = FrameManager.GetFrameElementOnSceneByID<FrameElement>(frameElementID);
                if (element != null) {
                    FrameManager.RemoveElement(element.id);
                }
            }
            public static void LoadElementsOnScene<TKey, TValue>(List<FrameElementIDPair> pairs)
            where TValue : FrameElement
            where TKey : FrameCore.ScriptableObjects.FrameElementSO {
                foreach (var pair in pairs) {
                    if (pair.elementObject.GetType() == typeof(TKey)) {
                        foreach (var id in pair.ids) {
                            Values values = null;
                            if (FrameManager.frame.currentKey.frameKeyValues.ContainsKey(id))
                                values = FrameManager.frame.currentKey.frameKeyValues[id];

                            pair.elementObject.LoadElementOnScene<TValue>(pair, id, values);
                        }
                    }
                }
            }
        }
    }
}
