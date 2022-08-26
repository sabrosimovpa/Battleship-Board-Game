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
        private Transform _choseSheepsPanel;

        [SerializeField]
        private WinLoseUI _winLoseUi;

        public UnityEvent<GameObject> OnSheepChosen = new UnityEvent<GameObject>();
        public UnityEvent<bool> PauseEvent = new UnityEvent<bool>();

        private Button lastStartedButton;

        // Start is called before the first frame update
        void Start()
        {
            _winLoseUi.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SwichPauseState();
            }
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
            _choseSheepsPanel.gameObject.SetActive(show);
        }

        public void EndGame(bool isPlayerWin)
        {
            _winLoseUi.ShowEndGameUI(isPlayerWin);
        }

        public void SwichPauseState()
        {
            _winLoseUi.PauseMenu(!_winLoseUi.IsOnPause);
            PauseEvent?.Invoke(_winLoseUi.IsOnPause);
        }
    }
}
