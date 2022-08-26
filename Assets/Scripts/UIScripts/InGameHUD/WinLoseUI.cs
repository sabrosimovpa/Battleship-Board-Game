using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.UI

{
    public class WinLoseUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _loseImage;
        [SerializeField]
        private GameObject _winImage;
        [SerializeField]
        private GameObject _pauseImage;
        [SerializeField]
        private GameObject _continueButton;

        public bool IsOnPause => _pauseImage.gameObject.activeSelf;

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
            _continueButton.SetActive(false);
            _pauseImage.SetActive(false);
            _loseImage.SetActive(!isPlayerWin);
            _winImage.SetActive(isPlayerWin);
            gameObject.SetActive(true);
            
        }

        public void PauseMenu(bool show)
        {
            _continueButton.SetActive(true);
            _pauseImage.SetActive(show);
            _loseImage.SetActive(false);
            _winImage.SetActive(false);
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
