using BattleshipBoardGame.AI;
using BattleshipBoardGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gameFieldPrefub;
        [SerializeField]
        private GameObject _playerRoot;
        [SerializeField]
        private GameObject _oponentRoot;
        [SerializeField]
        private HUD _userUi;

        [SerializeField]
        private GameObject[] _shipsForAI;

        private GameFieldManager playersShips;
        private GameFieldManager playersAttackField;

        private GameFieldManager oponentShips;
        private GameFieldManager oponentAttackField;

        private int playsedShips = 0;

        private GameFieldManager attacingField;
        private GameFieldManager attacedField;

        private bool isPlayerFire = false;

        private IAI _aI;

        // Start is called before the first frame update
        void Start()
        {
            _aI = gameObject.AddComponent<SimpleRandomAI>();
            playersShips = Instantiate(_gameFieldPrefub).GetComponent<GameFieldManager>();
            playersShips.transform.parent = _playerRoot.transform;
            playersShips.transform.localPosition = new Vector3(-7, 0, 0);
            playersShips.name = "playersShips_" + playersShips.name;

            playersAttackField = Instantiate(_gameFieldPrefub).GetComponent<GameFieldManager>();
            playersAttackField.transform.parent = _playerRoot.transform;
            playersAttackField.transform.localPosition = new Vector3(7, 0, 0);
            playersAttackField.name = "playersAttackField" + playersAttackField.name;

            oponentShips = Instantiate(_gameFieldPrefub).GetComponent<GameFieldManager>();
            oponentShips.transform.parent = _oponentRoot.transform;
            oponentShips.transform.localPosition = new Vector3(-7, 0, 0);
            oponentShips.name = "oponentShips" + oponentShips.name;

            oponentAttackField = Instantiate(_gameFieldPrefub).GetComponent<GameFieldManager>();
            oponentAttackField.transform.parent = _oponentRoot.transform;
            oponentAttackField.transform.localPosition = new Vector3(7, 0, 0);
            oponentAttackField.name = "oponentAttackField" + oponentAttackField.name;

            _userUi.ShowChoseSheepsPanel(true);
            _userUi.OnSheepChosen.AddListener(OnUserChoseSheep);
            _userUi.PauseEvent.RemoveAllListeners();
            _userUi.PauseEvent.AddListener(OnPause);
            _aI.SetubAI(_shipsForAI, oponentShips, oponentAttackField);
            StartCoroutine(WaitForAllReady());
        }

        private void OnPause(bool isOnPause)
        {
            playersShips.Pause(isOnPause);
            playersAttackField.Pause(isOnPause);

            oponentShips.Pause(isOnPause);
            oponentAttackField.Pause(isOnPause);
        }

        private void OnUserChoseSheep(GameObject arg0)
        {
            playersShips.StartShipPlasing(arg0, OnPlasingFinish);
        }

        private void OnPlasingFinish()
        {
            playsedShips++;
            _userUi.ShipPlaysed();
        }

        private IEnumerator WaitForAllReady()
        {
            yield return null;

            while(playsedShips < _shipsForAI.Length || !_aI.Ready)
            {
                yield return new WaitForSeconds(1.0f);
            }

            StartGameSession();
        }

        private void StartGameSession()
        {
            PrepareAndStartAttack();
        }

        private void PrepareAndStartAttack(bool isUserShoot = true)
        {
            bool needProside = IsneedProside();

            if (needProside)
            {
                SetubAttacingPositions(isUserShoot);
                StartAttack();
            }
            else
            {
                EndGame();
            }    
        }

        private bool IsneedProside()
        {
            return playersShips.IsEnioneStanding && oponentShips.IsEnioneStanding;
        }

        private void SetubAttacingPositions(bool isPlayerFire)
        {
            this.isPlayerFire = isPlayerFire;
            if(isPlayerFire)
            {
                attacingField = playersAttackField;
                attacedField = oponentShips;
            }
            else
            {
                attacingField = oponentAttackField;
                attacedField = playersShips;
            }
        }

        private void StartAttack()
        {
            attacingField.StartShooting(EndShooting);
            if (!isPlayerFire)
            {
                _aI.StartShooting();
            }            
        }

        private void EndShooting(Vector2Int[] shoots)
        {
            var results = attacedField.CheckShoots(shoots);
            attacingField.ApplyHitResult(results);
            PrepareAndStartAttack(!isPlayerFire);
        }

        private void EndGame()
        {
            _userUi.EndGame(playersShips.IsEnioneStanding);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

    }
}
