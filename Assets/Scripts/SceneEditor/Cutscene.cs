using FrameCore.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore {
    public class Cutscene : MonoBehaviour, IKeyTransition {
        public KeySequenceData keySequenceData { get { return _keySequenceData; } set { _keySequenceData = value; } }
        [SerializeField]
        private KeySequenceData _keySequenceData;

        private void OnDisable() {
            FrameManager.SetKey(keySequenceData.nextKeyID);
        }
    }
}
