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
            loseImage.SetActive(!isPlayerWin);
            winImage.SetActive(isPlayerWin);
            gameObject.SetActive(true);
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
