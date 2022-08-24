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

        // Start is called before the first frame update
        void Start()
        {
            playersShips = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            playersShips.transform.parent = PlayerRoot.transform;
            playersShips.transform.localPosition = new Vector3(-7, 0, 0);

            playersAttackField = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            playersAttackField.transform.parent = PlayerRoot.transform;
            playersAttackField.transform.localPosition = new Vector3(7, 0, 0);

            oponentShips = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            oponentShips.transform.parent = OponentRoot.transform;
            oponentShips.transform.localPosition = new Vector3(-7, 0, 0);

            oponentAttackField = Instantiate(GameFieldPrefub).GetComponent<GameFieldManager>();
            oponentAttackField.transform.parent = OponentRoot.transform;
            oponentAttackField.transform.localPosition = new Vector3(7, 0, 0);

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
            Debug.Log("game");
        }

        // Update is called once per frame
        void Update()
        {
        
        }


    }
}
