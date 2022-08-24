using BattleshipBoardGame.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class GameManager : SingletonMono<GameManager>
    {
        public void StartGameScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void GoToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("StartScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
