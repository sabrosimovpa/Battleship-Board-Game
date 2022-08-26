using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;
using System.Linq;

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
            sucsessHits.Clear();
            lastShootPosition = new Vector2Int(-1, -1);
            PlaceNextShip();
        }

        private Vector2Int lastShootPosition = new Vector2Int(-1,-1);
        private List<Vector2Int> sucsessHits = new List<Vector2Int>();

        public void StartShooting()
        {
            var shootedPositions = targitingField.ShootedPlayses;
            var position = AnalyzePosition(shootedPositions);
            if (position.x < 0)
            {
                position = new Vector2Int(URandom.Range(0, 10), URandom.Range(0, 10));
                while (!IsFreeToShoot(shootedPositions, position))
                {
                    position = ShiftRandomPosition(position);
                }
            }

            lastShootPosition = position;
            targitingField.PlaceShootOn(position);
        }

        private Vector2Int AnalyzePosition(ShootingPin[][] pinsMap)
        {
            var result = new Vector2Int(-1, -1);

            if(lastShootPosition.x >= 0)
            {
                var lastMark = pinsMap[lastShootPosition.x][lastShootPosition.y];
                if (lastMark.CurrentState == ShootingPin.ShootingPinState.Hit)
                {
                    sucsessHits.Add(lastShootPosition);
                }

                for (int i = 0; i < sucsessHits.Count; i++)
                {
                    var mark = sucsessHits[i];
                    var pin = pinsMap[mark.x][mark.y];
                    if(pin.IsShipDestroyed)
                    {
                        sucsessHits.RemoveAt(i);
                        i--;
                    }
                }

                if(sucsessHits.Count > 0 && sucsessHits.Contains(lastShootPosition))
                {
                    if(sucsessHits.Count > 1)
                    {
                        var last = sucsessHits[sucsessHits.Count - 1];
                        var prev = sucsessHits[sucsessHits.Count - 2];
                        var newH = last + last - prev;
                        if(newH.x >= 0 && newH.x < 10 && newH.y >= 0 && newH.y < 10)
                        {
                            if(IsFreeToShoot(pinsMap, newH))
                            {
                                return newH;
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                if(sucsessHits.Count > 0 && sucsessHits.Count < 2)
                {
                    var shoot = sucsessHits.Last();
                    do
                    {
                        shoot = ShiftRandomPosition(shoot);

                    } while (IsFreeToShoot(pinsMap, shoot));

                    result = shoot;
                }
            }

            return result;
        }

        private bool IsFreeToShoot(ShootingPin[][] pinsMap, Vector2Int position)
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
            
            var inGameField = true;
            do
            {
                var r = URandom.Range(0.0f, 4.0f);

                if (r <= 1.0f)
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
                    inGameField = false;
                    position = oldPosition;
                }

                if (position.y >= 10 || position.y < 0)
                {
                    inGameField = false;
                    position = oldPosition;
                }
            } while (!inGameField);

            return position;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }
    }
}
