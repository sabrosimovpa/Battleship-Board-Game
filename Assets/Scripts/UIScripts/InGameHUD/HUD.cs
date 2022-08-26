using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BattleshipBoardGame.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private Transform choseSheepsPanel;

        [SerializeField]
        private WinLoseUI winLoseUi;

        public UnityEvent<GameObject> OnSheepChosen = new UnityEvent<GameObject>();

        private Button lastStartedButton;

        // Start is called before the first frame update
        void Start()
        {
            winLoseUi.gameObject.SetActive(false);
        }


        public void ShipPlaysed()
        {
            lastStartedButton = null;
        }

        public void ShipSelected(Button button, GameObject ship)
        {
            if(lastStartedButton != null)
            {
                lastStartedButton.interactable = true;
            }

            lastStartedButton = button;
            lastStartedButton.interactable = false;
            OnSheepChosen?.Invoke(ship);
        }

        public void ShowChoseSheepsPanel(bool show)
        {
            choseSheepsPanel.gameObject.SetActive(show);
        }

        public void EndGame(bool isPlayerWin)
        {
            winLoseUi.ShowEndGameUI(isPlayerWin);
        }
    }
}
