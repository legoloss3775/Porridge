using FrameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.FrameEffects {
    public class EffectPrefab : MonoBehaviour {
        public float speed { get { return GetComponent<FrameEffect>() != null ? GetComponent<FrameEffect>().animationSpeed : 1f; } }
        public float animationDelay { get { return GetComponent<FrameEffect>() != null ? GetComponent<FrameEffect>().animationDelay : 0f; } }

        public virtual void OnFrameKeyChanged() { }
    }
}
