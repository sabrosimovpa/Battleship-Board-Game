using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.UI

{
    public class WinLoseUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject loseImage;
        [SerializeField]
        private GameObject winImage;
        [SerializeField]
        private GameObject pauseImage;

        [SerializeField]
        private GameObject continueButton;

        public bool IsOnPause => pauseImage.gameObject.activeSelf;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ShowEndGameUI(bool isPlayerWin)
        {
            continueButton.SetActive(false);
            pauseImage.SetActive(false);
            loseImage.SetActive(!isPlayerWin);
            winImage.SetActive(isPlayerWin);
            gameObject.SetActive(true);
            
        }

        public void PauseMenu(bool show)
        {
            continueButton.SetActive(true);
            pauseImage.SetActive(show);
            loseImage.SetActive(false);
            winImage.SetActive(false);
            gameObject.SetActive(show);
        }

        public void OnRestartButton()
        {
            GameManager.instance.StartGameScene();
        }

        public void OnMainMenu()
        {
            GameManager.instance.GoToMainMenu();
        }

        public void OnQuitButton()
        {
            GameManager.instance.ExitGame();
        }
    }
}
