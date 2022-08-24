using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleshipBoardGame
{
    [RequireComponent(typeof(Button))]
    public class ShipSellectedButtonClick : MonoBehaviour
    {
        [SerializeField]
        private GameObject ship;
        [SerializeField]
        private UnityEngine.Events.UnityEvent<Button, GameObject> ShipSelectedEvent;

        private Button button;
        // Start is called before the first frame update
        void Start()
        {
            button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnButtoClick);
        }

        private void OnButtoClick()
        {
            ShipSelectedEvent?.Invoke(button, ship);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
