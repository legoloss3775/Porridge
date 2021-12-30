using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace GameFramework {
    public enum GameState {
        MainMenu,
        FrameSequence,
    }
    public class GameManager : MonoBehaviour {

        public GameState gameState;
        public FrameManager frameManager;
        public PlayerData playerData;

        public void OnApplicationQuit() {
#if !UNITY_EDITOR
            switch (gameState) {
                case GameState.MainMenu:
                    break;
                case GameState.FrameSequence:
                    SaveManager.SavePlayer();
                    break;
            }
#endif
        }
        public void Start() {
            if (SceneManager.GetActiveScene().buildIndex == 0) {
                gameState = GameState.MainMenu;
            }
            else if(SceneManager.GetActiveScene().buildIndex == 1) {
                gameState = GameState.FrameSequence;
            }

            switch (gameState) {
                case GameState.MainMenu:
                    break;
                case GameState.FrameSequence:
                    frameManager = GameObject.Find("Frame Manager").GetComponent<FrameManager>();

#if UNITY_EDITOR
                    FrameKey.frameCoreFlags = SaveManager.LoadFlagsFile().flags;
#endif
#if !UNITY_EDITOR
                if (SaveManager.LoadPlayer() != null){
                    FrameKey.frameCoreFlags = SaveManager.LoadPlayer().flags;
                    playerData = SaveManager.LoadPlayer();

                    FrameManager.assetDatabase.selectedFrameIndex = playerData.currentFrame;
                    FrameManager.assetDatabase.selectedKeyIndex = playerData.currentKey;
                    frameManager._assetDatabase.selectedFrameIndex = playerData.currentFrame;
                    frameManager._assetDatabase.selectedKeyIndex = playerData.currentKey;
                }
                else{
                    playerData = new PlayerData();
                    FrameKey.frameCoreFlags = SaveManager.LoadFlagsFile().flags;
                }
#endif
                    FrameManager.SetFrame(FrameManager.assetDatabase.selectedFrameIndex, FrameManager.assetDatabase.selectedKeyIndex);
                    break;
            }
        }
        public void LoadFrame() {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        public void GameQuit() {
            switch (gameState) {
                case GameState.MainMenu:
#if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#endif
                    Application.Quit();
                    break;
                case GameState.FrameSequence:
                    break;
            }
        }
    }
}
