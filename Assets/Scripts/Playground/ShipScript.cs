using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class ShipScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject shipRoot;

        [SerializeField]
        private int totalSize;

        [SerializeField]
        private int sizeFromCenterToNose;

        private Vector2Int centerPlace;

        [SerializeField]
        private int sizeFromCenterToBack;

        [SerializeField]
        public GameObject[] WrongPlaysmentIndicators; //0 -- Center, 1 to n From center to nose, n+1 to j from center to back
        public bool[] demagedShipParts;
        public int damageTaked = 0;

        private bool isRotated = false;

        public bool IsRotated
        {
            get => isRotated;

            set
            {
                isRotated = value;
                if(isRotated)
                {
                    shipRoot.transform.eulerAngles = new Vector3(0, 90, 0);
                }
                else
                {
                    shipRoot.transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        public int TotalSize => totalSize;

        public bool IsAlive
        {
            get
            {
                return damageTaked < totalSize;
            }
        }

        public void Rotate()
        {
            IsRotated = !IsRotated;
        }

        public bool IsShipFitt(ShipScript[][] occupiedPlaces, Vector2 centerPlace)
        {
            var result = true;
            var fitInField = true;
            ResetIndicators();

            Vector2Int centerCoordinates = new Vector2Int(Mathf.RoundToInt(centerPlace.x), Mathf.RoundToInt(centerPlace.y));
            
            //Fit in game field
            for (int i = 0; i < totalSize; i++)
            {
                fitInField = true;
                var coordinateShift = ShiftCoordinateToShipPart(i, centerCoordinates);

                if (coordinateShift.x < 0 || coordinateShift.x >= occupiedPlaces.Length)
                {
                    result = false;
                    fitInField = false;
                    WrongPlaysmentIndicators[i].gameObject.SetActive(true);
                    //Debug.Log($"coordinateShift {coordinateShift} __ centerPlace {centerPlace} __ i {i}");                    
                }

                if (coordinateShift.y < 0 || coordinateShift.y >= occupiedPlaces.Length)
                {
                    result = false;
                    fitInField = false;
                    WrongPlaysmentIndicators[i].gameObject.SetActive(true);
                    //Debug.Log($"coordinateShift {coordinateShift} __ centerPlace {centerPlace} __ i {i}");
                }

                if (fitInField == true)
                {
                    if (occupiedPlaces[coordinateShift.x][coordinateShift.y] != null)
                    {
                        result = false;
                        WrongPlaysmentIndicators[i].gameObject.SetActive(true);
                    }
                }
            }

            return result;
        }

        public void OcupadePlace(Vector2 centerPlace, ShipScript[][] occupiedPlaces)
        {
            Vector2Int centerCoordinates = new Vector2Int(Mathf.RoundToInt(centerPlace.x), Mathf.RoundToInt(centerPlace.y));
            this.centerPlace = centerCoordinates;
            for (int i = 0; i < totalSize; i++)
            {
                var coordinateShift = ShiftCoordinateToShipPart(i, centerCoordinates);
                occupiedPlaces[coordinateShift.x][coordinateShift.y] = this;
            }
        }

        public bool TryTakeDamage(Vector2 firePlace)
        {
            var result = false;
            Vector2Int fireCoord = new Vector2Int(Mathf.RoundToInt(firePlace.x), Mathf.RoundToInt(firePlace.y));
            for (int i = 0; i < totalSize; i++)
            {
                var coordinateShift = ShiftCoordinateToShipPart(i, centerPlace);
                if(fireCoord == coordinateShift && demagedShipParts[i] == false)
                {
                    damageTaked++;
                    result = demagedShipParts[i] = true;
                    return result;
                }
            }

            return result;
        }

        public Vector2Int[] GetShipPartsPositions()
        {
            var ressult = new Vector2Int[totalSize];

            for (int i = 0; i < totalSize; i++)
            {
                ressult[i] = ShiftCoordinateToShipPart(i, centerPlace);
            }

            return ressult;
        }

        public void ResetIndicators()
        {
            for (int i = 0; i < WrongPlaysmentIndicators.Length; i++)
            {
                WrongPlaysmentIndicators[i].SetActive(false);
            }
        }

        private Vector2Int ShiftCoordinateToShipPart(int index, Vector2Int center)
        {
            int shiftedX = center.x;
            int shiftedY = center.y;

            var coordinateShift = GetShiftCoordinateForPartIndex(index);

            if(isRotated)
            {
                shiftedX = center.x + coordinateShift;
            }
            else
            {
                shiftedY = center.y - coordinateShift;
            }

            return new Vector2Int(shiftedX, shiftedY);
        }

        private int GetShiftCoordinateForPartIndex(int index)
        {
            var shift = 0;

            if(index > 0 && index <= sizeFromCenterToNose)
            {
                shift = index;
            }

            if(index > sizeFromCenterToNose && index < totalSize)
            {
                shift = sizeFromCenterToNose - index;
            }

            return shift;
        }

        // Start is called before the first frame update
        void Start()
        {
            demagedShipParts = new bool[totalSize];
        }
    }
}
