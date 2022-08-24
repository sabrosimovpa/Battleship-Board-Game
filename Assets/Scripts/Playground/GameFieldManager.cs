using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleshipBoardGame
{
    public class GameFieldManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject playgroundTilesRoot;
        [SerializeField]
        private GameObject PlaygroundAncor;


        [SerializeField]
        private GameObject testShip;

        private GameObject PlaysingShip;

        private GameFieldState gameFieldState = GameFieldState.None;

        private UnityEvent<PlaygroundTile> TileHoverChangedEvent = new UnityEvent<PlaygroundTile>();
        private PlaygroundTile lastHoweredTile;

        private ShipScript[][] placesOccupiedByShips = new ShipScript[10][];
        private List<ShipScript> ShipsOnField = new List<ShipScript>();

        // Start is called before the first frame update
        void Start()
        {
            ResetPlaceOccupiedByShip();
            var tiles = playgroundTilesRoot.GetComponentsInChildren<PlaygroundTile>();

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].TileMouseEvent.AddListener(OnTileMouseEventHandler);
            }

            if (testShip != null)
            {
                StartShipPlasing(testShip, StartTestPl);
            }
        }

        private void StartTestPl()
        {
            if (testShip != null)
            {
                StartShipPlasing(testShip, StartTestPl);
            }
        }

        private void OnTileMouseEventHandler(PlaygroundTile tile, PlaygroundTile.TileEventType eventType)
        {
            if(eventType == PlaygroundTile.TileEventType.MouseEnter)
            {
                TileHoverChangedEvent?.Invoke(tile);
            }
        }

        private bool canHandleClick = false;


        // Update is called once per frame
        void Update()
        {


            if (Input.GetMouseButtonDown(0))
            {
                if (!CanDoInputAction()) return;
                
                switch (gameFieldState)
                {
                    case GameFieldState.PlasingShips:
                        PlaceShip();
                        break;
                    case GameFieldState.WaytForShoot:
                        break;
                    case GameFieldState.WaytForReply:
                        break;
                    case GameFieldState.EndGame:
                        break;
                    default:
                        break;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!CanDoInputAction()) return;

                switch (gameFieldState)
                {
                    case GameFieldState.PlasingShips:
                        RotateShip();
                        break;
                    case GameFieldState.WaytForShoot:
                        break;
                    case GameFieldState.WaytForReply:
                        break;
                    case GameFieldState.EndGame:
                        break;
                    default:
                        break;
                }
            }
        }

        private bool CanDoInputAction()
        {
            var result = false;

            var mousePosition = Input.mousePosition;
            var screenPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            var curPosition = gameObject.transform.position;

            result = Mathf.Abs(screenPosition.x - curPosition.x) <= 5 &&
                     Mathf.Abs(screenPosition.z - curPosition.z) <=5 ;

            return result;
        }

        private Action onPlaysingFinished;

        public void StartShipPlasing(GameObject shipPrefub, Action onPlaysingFinished)
        {
            lastHoweredTile = null;
            this.onPlaysingFinished = onPlaysingFinished;
            gameFieldState = GameFieldState.PlasingShips;
            if (PlaysingShip != null)
            {
                Destroy(PlaysingShip);
                PlaysingShip = null;
            }
            PlaysingShip = Instantiate(shipPrefub);
            PlaysingShip.transform.parent = PlaygroundAncor.transform;
            PlaysingShip.transform.localPosition = new Vector3(10, 0.1f, -4);
            TileHoverChangedEvent.RemoveAllListeners();
            TileHoverChangedEvent.AddListener(TryFitHere);
        }

        private void TryFitHere(PlaygroundTile tile)
        {
            lastHoweredTile = tile;
            PlaysingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, tile.Position);
            PlaysingShip.transform.localPosition = new Vector3(tile.Position.x, 0.1f, -tile.Position.y);
        }

        private void RotateShip()
        {
            if (PlaysingShip != null)
            {
                PlaysingShip.GetComponent<ShipScript>().Rotate();
                PlaysingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTile.Position);
            }
        }

        private void PlaceShip()
        {
            if(PlaysingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTile.Position))
            {
                //TodoStoresShip Coordinates and occupation places
                PlaysingShip.transform.localPosition = new Vector3(lastHoweredTile.Position.x, 0, -lastHoweredTile.Position.y);
                PlaysingShip.GetComponent<ShipScript>().OcupadePlace(lastHoweredTile.Position, placesOccupiedByShips);
                PlaysingShip = null;
                gameFieldState = GameFieldState.None;
                TileHoverChangedEvent.RemoveAllListeners();
                onPlaysingFinished?.Invoke();
            }
        }

        private void ResetPlaceOccupiedByShip()
        {
            ShipsOnField.Clear();
            for (int row = 0; row < placesOccupiedByShips.Length; row++)
            {
                if (placesOccupiedByShips[row] == null)
                {
                    placesOccupiedByShips[row] = new ShipScript[10];
                }
                for (int col = 0; col < placesOccupiedByShips.Length; col++)
                {
                    placesOccupiedByShips[row][col] = null;
                }
            }
        }

        public enum GameFieldState
        {
            None,
            PlasingShips,
            WaytForShoot,
            WaytForReply,
            EndGame
        }
    }
}
