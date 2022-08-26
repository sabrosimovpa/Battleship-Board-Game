using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace BattleshipBoardGame.AI
{
    public class SimpleRandomAI : MonoBehaviour, IAI
    {
        private GameFieldManager _shipsField;
        private GameFieldManager _targitingField;

        private GameObject[] _ships;
        private int _playsedShips = 0;

        private bool _ready;

        public bool Ready => _ready;

        private UnityEngine.Events.UnityEvent<bool> aIReadyToFight = new UnityEngine.Events.UnityEvent<bool>();

        public UnityEngine.Events.UnityEvent<bool> AIReadyToFight => aIReadyToFight;

        public bool IsAlive
        {
            get
            {
                return _shipsField != null && _shipsField.isActiveAndEnabled;
            }
        }

        public void SetubAI(GameObject[] newShips, GameFieldManager shipsField, GameFieldManager targetingField)
        {
            _ships = newShips;
            this._shipsField = shipsField;
            this._targitingField = targetingField;
            PlaceNextShip();
        }

        private Vector2Int lastShootPosition;

        public void StartShooting()
        {
            var shootedPositions = _targitingField.ShootedPlayses;
            var position = new Vector2Int(URandom.Range(0, 10), URandom.Range(0, 10));
            while (!IsFreePosotion(shootedPositions, position))
            {
                position = ShiftRandomPosition(position);
            }

            _targitingField.PlaceShootOn(position);
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

            _playsedShips++;
            if (_playsedShips <= _ships.Length)
            {
                _shipsField.StartShipPlasing(_ships[_playsedShips - 1], PlaceNextShip);
            }
            else
            {
                _ready = true;
                aIReadyToFight?.Invoke(_ready);
                yield break;
            }

            yield return null;
            var position = new Vector2Int(URandom.Range(0, 10), URandom.Range(0, 10));
            while (!_shipsField.TryToFitShipOnPosition(position))
            {
                if (URandom.Range(0.0f, 2.0f) > 1.0f)
                {
                    _shipsField.RotatePlaysingShip();
                }

                position = ShiftRandomPosition(position);
            }

            _shipsField.PlaceShipOnLastPlace();
        }

        private Vector2Int ShiftRandomPosition(Vector2Int oldPosition)
        {
            var position = oldPosition;
            
            var r = URandom.Range(0.0f, 4.0f);
            if(r <= 1.0f)
            {
                position = position + Vector2Int.up;
            }



            if (r <= 2.0f && r > 1.0f)
            {
                position = position + Vector2Int.right;
            }
            if (r <= 3.0f && r > 2.0f)
            {
                position = position + Vector2Int.down;
            }
            if (r <= 4.0f && r > 3.0f)
            {
                position = position + Vector2Int.left;
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
