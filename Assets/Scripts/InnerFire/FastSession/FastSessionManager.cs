using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework {
    namespace InnerFire {
        public class FastSessionManager : GameManager {
            public static GameObject FastSessionContainer;
            private void Update() {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    FrameCore.FrameManager.SetKey(nextKeyID);
                }   
            }
        }
    }
}
