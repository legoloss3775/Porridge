using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;

namespace GameFramework {
    public class GameManager : MonoBehaviour {

        public FrameManager frameManager;
        public PlayerData playerData;

        public void OnApplicationQuit() {
#if !UNITY_EDITOR
            SaveManager.SavePlayer();
#endif
        }
        public void Start() {
            frameManager = GameObject.Find("Frame Manager").GetComponent<FrameManager>();

#if UNITY_EDITOR
            FrameKey.frameCoreFlags = SaveManager.LoadFlagsFile().flags;

#endif

#if !UNITY_EDITOR
            if (SaveManager.LoadPlayer() != null){
                FrameKey.frameCoreFlags = SaveManager.LoadPlayer().flags;
                playerData = SaveManager.LoadPlayer();
            }
            else{
                playerData = new PlayerData();
                FrameKey.frameCoreFlags = SaveManager.LoadFlagsFile().flags;
            }
             FrameManager.assetDatabase.selectedFrameIndex = playerData.currentFrame;
             FrameManager.assetDatabase.selectedKeyIndex = playerData.currentKey;
             frameManager._assetDatabase.selectedFrameIndex = playerData.currentFrame;
             frameManager._assetDatabase.selectedKeyIndex = playerData.currentKey;
#endif
            // SetFrame(0, 0);
            FrameManager.SetFrame(FrameManager.assetDatabase.selectedFrameIndex, FrameManager.assetDatabase.selectedKeyIndex);
        }
        
    }
}
