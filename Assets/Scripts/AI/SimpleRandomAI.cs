using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace BattleshipBoardGame.AI
{
    public class SimpleRandomAI : MonoBehaviour, IAI
    {
        private GameFieldManager shipsField;
        private GameFieldManager targitingField;

        private GameObject[] ships;
        private int playsedShips = 0;

        private bool ready;

        public bool Ready => ready;

        private UnityEngine.Events.UnityEvent<bool> aIReadyToFight = new UnityEngine.Events.UnityEvent<bool>();

        public UnityEngine.Events.UnityEvent<bool> AIReadyToFight => aIReadyToFight;

        public bool IsAlive
        {
            get
            {
                return shipsField != null && shipsField.isActiveAndEnabled;
            }
        }

        public void SetubAI(GameObject[] newShips, GameFieldManager shipsField, GameFieldManager targetingField)
        {
            ships = newShips;
            this.shipsField = shipsField;
            this.targitingField = targetingField;
            PlaceNextShip();
        }

        public void StartShooting()
        {
            var shootedPositions = targitingField.ShootedPlayses;
            var position = new Vector2Int(URandom.Range(0, 10), URandom.Range(0, 10));
            while (!IsFreePosotion(shootedPositions, position))
            {
                position = ShiftRandomPosition(position);
            }

            targitingField.PlaceShootOn(position);
        }

        private bool IsFreePosotion(ShootingPin[][] pinsMap, Vector2Int position)
        {
            return pinsMap[position.x][position.y] == null || pinsMap[position.x][position.y].CurrentState == ShootingPin.ShootingPinState.None;
        }

        private void PlaceNextShip()
        {
            StartCoroutine(PlaceNextShipC());
        }

        private IEnumerator PlaceNextShipC()
        {
            yield return null;

            playsedShips++;
            if (playsedShips <= ships.Length)
            {
                shipsField.StartShipPlasing(ships[playsedShips - 1], PlaceNextShip);
            }
            else
            {
                ready = true;
                aIReadyToFight?.Invoke(ready);
                yield break;
            }

            yield return null;
            var position = new Vector2Int(URandom.Range(0, 10), URandom.Range(0, 10));
            while (!shipsField.TryToFitShipOnPosition(position))
            {
                if (URandom.Range(0.0f, 2.0f) > 1.0f)
                {
                    shipsField.RotatePlaysingShip();
                }

                position = ShiftRandomPosition(position);
            }

            shipsField.PlaceShipOnLastPlace();
        }

        private Vector2Int ShiftRandomPosition(Vector2Int oldPosition)
        {
            var position = oldPosition;
            
            var var = URandom.Range(0.0f, 4.0f);
            switch (var)
            {
                case <= 1.0f:
                    position = position + Vector2Int.up;
                    break;
                case <= 2.0f:
                    position = position + Vector2Int.left;
                    break;
                case <= 3.0f:
                    position = position + Vector2Int.down;
                    break;
                case <= 4.0f:
                    position = position + Vector2Int.right;
                    break;
                default:
                    position = position + Vector2Int.one;
                    break;
            }

            if (position.x >= 10 || position.x < 0)
            {
                position = new Vector2Int(URandom.Range(1, 9), position.y);
            }

            if (position.y >= 10 || position.y < 0)
            {
                position = new Vector2Int(position.x, URandom.Range(1, 9));
            }

            return position;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }
    }
}
