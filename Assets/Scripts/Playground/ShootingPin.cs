using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame
{
    public class ShootingPin : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hitPin;
        [SerializeField]
        private GameObject _MissPin;
        [SerializeField]
        private GameObject _placePin;
        [SerializeField]
        private GameObject _wrongPosition;

        private ShootingPinState curState;

        public Vector2Int Position { get; set; }

        public ShootingPinState CurrentState => curState;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void ChangePinState(ShootingPinState newState)
        {
            curState = newState;

            _hitPin.SetActive(false);
            _MissPin.SetActive(false);
            _placePin.SetActive(false);
            _wrongPosition.SetActive(false);

            switch (curState)
            {                
                case ShootingPinState.Hit:
                    _hitPin.SetActive(true);
                    break;
                case ShootingPinState.Miss:
                    _MissPin.SetActive(true);
                    break;
                case ShootingPinState.Plasing:
                    _placePin.SetActive(true);
                    break;
                case ShootingPinState.WrongPosition:
                    _wrongPosition.SetActive(true);
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
