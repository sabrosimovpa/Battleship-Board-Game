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
        private GameObject ShootingPinPrefub;

        [SerializeField]
        private GameObject DestrooyedEnemyShipPartIndicator;

        private GameObject PlaysingShip;

        private GameFieldState gameFieldState = GameFieldState.None;

        private UnityEvent<PlaygroundTile> TileHoverChangedEvent = new UnityEvent<PlaygroundTile>();
        private PlaygroundTile lastHoweredTile;

        private ShipScript[][] placesOccupiedByShips = new ShipScript[10][];
        private ShootingPin[][] shootedPlayses = new ShootingPin[10][];
        private List<ShipScript> ShipsOnField = new List<ShipScript>();
        private int liveShipOnField;

        public int LiveShipOnField => liveShipOnField;

        // Start is called before the first frame update
        void Start()
        {
            ResetPlaces();
            var tiles = playgroundTilesRoot.GetComponentsInChildren<PlaygroundTile>();

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].TileMouseEvent.AddListener(OnTileMouseEventHandler);
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
                        PlaceShoot();
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
        #region plaseShipOnField
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
                ShipsOnField.Add(PlaysingShip.GetComponent<ShipScript>());
                liveShipOnField++;
                gameFieldState = GameFieldState.None;
                TileHoverChangedEvent.RemoveAllListeners();
                PlaysingShip = null;
                onPlaysingFinished?.Invoke();
            }
        }
        #endregion

        #region shootingState

        private Action<Vector2Int[]> playerShootsCallback;
        private ShootingPin currentShoot;

        public void StartShooting(Action<Vector2Int[]> callback)
        {
            playerShootsCallback = callback;
            gameFieldState = GameFieldState.WaytForShoot;

            if(currentShoot != null)
            {
                Destroy(currentShoot.gameObject);
                currentShoot = null;
            }

            currentShoot = Instantiate(ShootingPinPrefub).GetComponent<ShootingPin>();
            currentShoot.gameObject.transform.parent = PlaygroundAncor.transform;
            currentShoot.transform.localPosition = new Vector3(10, 0, -4);
            TileHoverChangedEvent.RemoveAllListeners();
            TileHoverChangedEvent.AddListener(ShootTraker);
            currentShoot.ChangePinState(ShootingPin.ShootingPinState.Plasing);
        }

        private void ShootTraker(PlaygroundTile hoveredTile)
        {
            lastHoweredTile = hoveredTile;
            PlaceShoot(true);
        }

        private void PlaceShoot(bool checkOnly = false)
        {
            var normalizedVector = new Vector2Int(Mathf.RoundToInt(lastHoweredTile.Position.x), Mathf.RoundToInt(lastHoweredTile.Position.y));
            var pinInPlace = shootedPlayses[normalizedVector.x][normalizedVector.y];
            if (pinInPlace == null)
            {
                currentShoot.ChangePinState(ShootingPin.ShootingPinState.Plasing);
            }
            else
            {
                currentShoot.ChangePinState(ShootingPin.ShootingPinState.WrongPosition);
            }

            currentShoot.gameObject.transform.localPosition = new Vector3(normalizedVector.x, 0, -normalizedVector.y);

            if (checkOnly) return;

            if(currentShoot.CurrentState == ShootingPin.ShootingPinState.Plasing)
            {
                shootedPlayses[normalizedVector.x][normalizedVector.y] = currentShoot;
                TileHoverChangedEvent.RemoveAllListeners();
                gameFieldState = GameFieldState.None;
                currentShoot = null;
                playerShootsCallback?.Invoke(new Vector2Int[] { normalizedVector });
            }
        }        

        public void ApplyHitResult(IList<ShootResult> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                if(shootedPlayses[result.Position.x][result.Position.y] != null)
                {
                    shootedPlayses[result.Position.x][result.Position.y].ChangePinState(result.IsOnTarget == true ? ShootingPin.ShootingPinState.Hit : ShootingPin.ShootingPinState.Miss);
                    if(result.isDestroyed)
                    {
                        for (int pi= 0; pi < result.DestroyedPositionsOfShip.Length; pi++)
                        {
                            var destroyedPart = Instantiate(DestrooyedEnemyShipPartIndicator);
                            destroyedPart.transform.parent = PlaygroundAncor.transform;
                            destroyedPart.transform.localPosition = new Vector3(result.DestroyedPositionsOfShip[pi].x, 0, -result.DestroyedPositionsOfShip[pi].y);
                        }
                    }
                }
            }
        }

        public IList<ShootResult> CheckShoots(IList<Vector2Int> shootingPositions)
        {
            var result = new List<ShootResult>();

            for (int i = 0; i < shootingPositions.Count; i++)
            {
                var position = shootingPositions[i];
                var shootResult = new ShootResult() { Position = position, IsOnTarget = false };
                var shipOnPosition = this.placesOccupiedByShips[position.x][position.y];
                if (shipOnPosition != null && shipOnPosition.TryTakeDamage(position))
                {
                    shootResult.IsOnTarget = true;
                    if (!shipOnPosition.IsAlive)
                    {
                        liveShipOnField--;
                        shootResult.isDestroyed = true;
                        shootResult.DestroyedPositionsOfShip = shipOnPosition.GetShipPartsPositions();
                    }
                }

                currentShoot = Instantiate(ShootingPinPrefub).GetComponent<ShootingPin>();
                currentShoot.gameObject.transform.parent = PlaygroundAncor.transform;
                currentShoot.Position = position;
                currentShoot.transform.localPosition = new Vector3(currentShoot.Position.x, 0, -currentShoot.Position.y);
                currentShoot.ChangePinState(ShootingPin.ShootingPinState.Miss);
                shootedPlayses[position.x][position.y] = currentShoot;
                if (shootResult.IsOnTarget)
                {
                    currentShoot.ChangePinState(ShootingPin.ShootingPinState.Hit);
                }
                currentShoot = null;
                result.Add(shootResult);
            }

            return result;
        }

        #endregion


        private void ResetPlaces()
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

            for (int row = 0; row < shootedPlayses.Length; row++)
            {
                if (shootedPlayses[row] == null)
                {
                    shootedPlayses[row] = new ShootingPin[10];
                }
                for (int col = 0; col < shootedPlayses.Length; col++)
                {
                    shootedPlayses[row][col] = null;
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

        public class ShootResult
        {
            public Vector2Int Position { get; set; }
            public bool IsOnTarget { get; set; }

            public bool isDestroyed { get; set; }

            public Vector2Int[] DestroyedPositionsOfShip { get; set; }
        }
    }
}
