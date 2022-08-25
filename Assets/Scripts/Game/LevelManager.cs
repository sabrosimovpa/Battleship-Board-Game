using BattleshipBoardGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject GameFieldPrefub;
        public GameObject PlayerRoot;
        public GameObject OponentRoot;
        public HUD UserUi;

        private GameFieldManager playersShips;
        private GameFieldManager playersAttackField;

        private GameFieldManager oponentShips;
        private GameFieldManager oponentAttackField;

        private int playsedShips = 0;

        private GameFieldManager attacingField;
        private GameFieldManager attacedField;

        private bool isPlayerFire = false;

        // Start is called before the first frame update
        void Start()
        {
            playersShips = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            playersShips.transform.parent = PlayerRoot.transform;
            playersShips.transform.localPosition = new Vector3(-7, 0, 0);
            playersShips.name = "playersShips_" + playersShips.name;

            playersAttackField = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            playersAttackField.transform.parent = PlayerRoot.transform;
            playersAttackField.transform.localPosition = new Vector3(7, 0, 0);
            playersAttackField.name = "playersAttackField" + playersAttackField.name;

            oponentShips = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            oponentShips.transform.parent = OponentRoot.transform;
            oponentShips.transform.localPosition = new Vector3(-7, 0, 0);
            oponentShips.name = "oponentShips" + oponentShips.name;

            oponentAttackField = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            oponentAttackField.transform.parent = OponentRoot.transform;
            oponentAttackField.transform.localPosition = new Vector3(7, 0, 0);
            oponentAttackField.name = "oponentAttackField" + oponentAttackField.name;

            UserUi.ShowChoseSheepsPanel(true);
            UserUi.OnSheepChosen.AddListener(OnUserChoseSheep);
        }

        private void OnUserChoseSheep(GameObject arg0)
        {
            playersShips.StartShipPlasing(arg0, OnPlasingFinish);
        }

        private void OnPlasingFinish()
        {
            playsedShips++;
            UserUi.ShipPlaysed();
            if(playsedShips >= 5)
            {
                StartGameSession();
            }
            
        }

        private void StartGameSession()
        {
            PrepareAndStartAttack();
        }

        private void PrepareAndStartAttack(bool isUserShoot = true)
        {
            SetubAttacingPositions(isUserShoot);
            StartAttack();
        }

        private void SetubAttacingPositions(bool isPlayerFire)
        {
            this.isPlayerFire = isPlayerFire;
            if(isPlayerFire)
            {
                attacingField = playersAttackField;
                attacedField = playersShips;
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
        }

        private void EndShooting(Vector2Int[] shoots)
        {
            var results = attacedField.CheckShoots(shoots);
            attacingField.ApplyHitResult(results);
            PrepareAndStartAttack(); //TEST purpose
        }

        // Update is called once per frame
        void Update()
        {
        
        }



    }
}
