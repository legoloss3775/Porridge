using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FrameManager))]
public class FrameController : MonoBehaviour
{
    public enum InputType {
        ButtonClick,
        KeyInput,
    }
    public InputType type;
    public FrameManager manager;
    private void Awake() {
        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            IInteractable element = (IInteractable)FrameManager.frameElements.Where(ch => ch is IInteractable).FirstOrDefault();
            manager.SetKey(element.nextKeyID);
        }
    }
}
