using BattleshipBoardGame.UI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleshipBoardGame.UI
{
    public class MainMenu : BaseMenuItem
    {
        [SerializeField]
        Button startGameButton;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void OnMenuLoad()
        {
            gameObject.SetActive(true);
            startGameButton.onClick.AddListener(OnStartGameButton);
        }

        public override void OnMenuUnload()
        {
            gameObject.SetActive(false);
        }

        private void OnStartGameButton()
        {
            GameManager.instance.StartGameScene();
        }
    }
}
