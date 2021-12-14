using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InnerFire {
    public class Checkpoint : MonoBehaviour {
        public FastSessionManager fastSessionManager;
        private void Start() {
            fastSessionManager = (FastSessionManager)FrameCore.FrameManager.GetGameManager<FastSessionManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            FrameCore.FrameManager.SetKey(fastSessionManager.nextKeyID);
        }
    }
}
