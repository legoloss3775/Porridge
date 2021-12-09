using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameCore {
    [RequireComponent(typeof(FrameManager))]
    public class FrameController : MonoBehaviour {
        public enum InputType {
            ButtonClick,
            KeyInput,
        }
        public FrameManager manager;
    }
}
