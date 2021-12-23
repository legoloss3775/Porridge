using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;

namespace GameFramework {
    public class GameManager : MonoBehaviour, IKeyTransition {
        /**public string id { get {
                if (this != null)
                    return name + "_" + FrameManager.gameManagers.Count;
                else return null;
            } 
        }**/
        [SerializeField]
        private int _nextKeyID;
        [SerializeField]
        private int _previousKeyID;

        public int nextKeyID { get => _nextKeyID; set => _nextKeyID = value; }
        public int previousKeyID { get => _previousKeyID; set => _previousKeyID = value; }

        public void KeyTransitionInput() {

        }
    }
}
