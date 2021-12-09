using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore {
    public interface IKeyTransition {
        int nextKeyID { get; set; }
        int previousKeyID { get; set; }

        void KeyTransitionInput();
    }
}
