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
        private GameObject gameFieldPrefub;
        [SerializeField]
        private GameObject playerRoot;
        [SerializeField]
        private GameObject oponentRoot;
        [SerializeField]
        private HUD userUi;

        [SerializeField]
        private GameObject[] shipsForAI;

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
            playersShips = Instantiate(gameFieldPrefub).GetComponent<GameFieldManager>();
            playersShips.transform.parent = playerRoot.transform;
            playersShips.transform.localPosition = new Vector3(-7, 0, 0);
            playersShips.name = "playersShips_" + playersShips.name;

            playersAttackField = Instantiate(gameFieldPrefub).GetComponent<GameFieldManager>();
            playersAttackField.transform.parent = playerRoot.transform;
            playersAttackField.transform.localPosition = new Vector3(7, 0, 0);
            playersAttackField.name = "playersAttackField" + playersAttackField.name;

            oponentShips = Instantiate(gameFieldPrefub).GetComponent<GameFieldManager>();
            oponentShips.transform.parent = oponentRoot.transform;
            oponentShips.transform.localPosition = new Vector3(-7, 0, 0);
            oponentShips.name = "oponentShips" + oponentShips.name;

            oponentAttackField = Instantiate(gameFieldPrefub).GetComponent<GameFieldManager>();
            oponentAttackField.transform.parent = oponentRoot.transform;
            oponentAttackField.transform.localPosition = new Vector3(7, 0, 0);
            oponentAttackField.name = "oponentAttackField" + oponentAttackField.name;

            userUi.ShowChoseSheepsPanel(true);
            userUi.OnSheepChosen.AddListener(OnUserChoseSheep);
            _aI.SetubAI(shipsForAI, oponentShips, oponentAttackField);
            StartCoroutine(WaitForAllReady());
        }

        private void OnUserChoseSheep(GameObject arg0)
        {
            playersShips.StartShipPlasing(arg0, OnPlasingFinish);
        }

        private void OnPlasingFinish()
        {
            playsedShips++;
            userUi.ShipPlaysed();
        }

        private IEnumerator WaitForAllReady()
        {
            yield return null;

            while(playsedShips < shipsForAI.Length || !_aI.Ready)
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
                Debug.Log("gameStop");
            }    
        }

        private bool IsneedProside()
        {
            var result = true;

            if(!playersShips.IsEnioneStanding)
            {
                Debug.Log("PlayerLose");
                result = false;
            }

            if(!oponentShips.IsEnioneStanding)
            {
                Debug.Log("AI lose");
                result = false;
            }

            return result;
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

        // Update is called once per frame
        void Update()
        {
        
        }



    }
}
