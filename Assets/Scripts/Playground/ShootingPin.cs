using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class ShootingPin : MonoBehaviour
    {
        [SerializeField]
        private GameObject HitPin;
        [SerializeField]
        private GameObject MissPin;
        [SerializeField]
        private GameObject PlacePin;
        [SerializeField]
        private GameObject WrongPosition;

        private ShootingPinState curState;

        public Vector2Int Position { get; set; }

        public ShootingPinState CurrentState => curState;

        public bool IsShipDestroyed { get; set; }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void ChangePinState(ShootingPinState newState)
        {
            curState = newState;

            HitPin.SetActive(false);
            MissPin.SetActive(false);
            PlacePin.SetActive(false);
            WrongPosition.SetActive(false);

            switch (curState)
            {                
                case ShootingPinState.Hit:
                    HitPin.SetActive(true);
                    break;
                case ShootingPinState.Miss:
                    MissPin.SetActive(true);
                    break;
                case ShootingPinState.Plasing:
                    PlacePin.SetActive(true);
                    break;
                case ShootingPinState.WrongPosition:
                    WrongPosition.SetActive(true);
                    break;

                default:
                    break;
            }
        }

        public enum ShootingPinState
        {
            None,
            Hit,
            Miss,
            Plasing,
            WrongPosition
        }
    }
}
