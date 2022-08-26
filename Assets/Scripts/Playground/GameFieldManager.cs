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
        private GameObject _playgroundTilesRoot;
        [SerializeField]
        private GameObject _playgroundAncor;
        [SerializeField]
        private GameObject _shootingPinPrefub;

        [SerializeField]
        private GameObject _destrooyedEnemyShipPartIndicator;

        private GameObject playsingShip;

        private GameFieldState gameFieldState = GameFieldState.None;

        private UnityEvent<Vector2Int> TileHoverChangedEvent = new UnityEvent<Vector2Int>();
        private Vector2Int lastHoweredTilePosition;

        private ShipScript[][] placesOccupiedByShips = new ShipScript[10][];
        private ShootingPin[][] shootedPlayses = new ShootingPin[10][];
        private List<ShipScript> ShipsOnField = new List<ShipScript>();
        private int liveShipOnField;

        public int LiveShipOnField => liveShipOnField;

        public bool IsEnioneStanding => liveShipOnField > 0;

        public ShootingPin[][] ShootedPlayses => shootedPlayses;

        // Start is called before the first frame update
        void Start()
        {
            ResetPlaces();
            var tiles = _playgroundTilesRoot.GetComponentsInChildren<PlaygroundTile>();

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].TileMouseEvent.AddListener(OnTileMouseEventHandler);
            }
        }

        private void OnTileMouseEventHandler(PlaygroundTile tile, PlaygroundTile.TileEventType eventType)
        {
            if(eventType == PlaygroundTile.TileEventType.MouseEnter)
            {
                TileHoverChangedEvent?.Invoke(tile.Position);
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonUp(0))
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

            if (Input.GetMouseButtonUp(1))
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

        private GameFieldState prePauseState = GameFieldState.None;

        public void Pause(bool isPaused)
        {
            if(isPaused && gameFieldState != GameFieldState.None)
            {
                prePauseState = this.gameFieldState;
                this.gameFieldState = GameFieldState.None;
            }

            if(!isPaused && prePauseState != GameFieldState.None && prePauseState != gameFieldState)
            {
                gameFieldState = prePauseState;
                prePauseState = GameFieldState.None;
            }
        }

        #region plaseShipOnField
        private Action onPlaysingFinished;

        public void StartShipPlasing(GameObject shipPrefub, Action onPlaysingFinished)
        {
            lastHoweredTilePosition = Vector2Int.zero;
            this.onPlaysingFinished = onPlaysingFinished;
            gameFieldState = GameFieldState.PlasingShips;
            if (playsingShip != null)
            {
                Destroy(playsingShip);
                playsingShip = null;
            }
            playsingShip = Instantiate(shipPrefub);
            playsingShip.transform.parent = _playgroundAncor.transform;
            playsingShip.transform.localPosition = new Vector3(10, 0.1f, -4);
            TileHoverChangedEvent.RemoveAllListeners();
            TileHoverChangedEvent.AddListener(SignalFitOnPositionChanged);
        }

        private void SignalFitOnPositionChanged(Vector2Int tilePosition)
        {
            if (gameFieldState == GameFieldState.None) return;
            lastHoweredTilePosition = tilePosition;
            playsingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTilePosition);
            playsingShip.transform.localPosition = new Vector3(lastHoweredTilePosition.x, 0.1f, -lastHoweredTilePosition.y);
        }

        private void RotateShip()
        {
            if (playsingShip != null)
            {
                playsingShip.GetComponent<ShipScript>().Rotate();
                playsingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTilePosition);
            }
        }

        private void PlaceShip()
        {
            if(playsingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTilePosition))
            {
                //TodoStoresShip Coordinates and occupation places
                playsingShip.transform.localPosition = new Vector3(lastHoweredTilePosition.x, 0, -lastHoweredTilePosition.y);
                playsingShip.GetComponent<ShipScript>().OcupadePlace(lastHoweredTilePosition, placesOccupiedByShips);
                ShipsOnField.Add(playsingShip.GetComponent<ShipScript>());
                liveShipOnField++;
                gameFieldState = GameFieldState.None;
                TileHoverChangedEvent.RemoveAllListeners();
                playsingShip = null;
                onPlaysingFinished?.Invoke();
            }
        }

        public void RotatePlaysingShip()
        {
            RotateShip();
        }

        public bool TryToFitShipOnPosition(Vector2Int newPosition)
        {
            lastHoweredTilePosition = newPosition;
            return playsingShip.GetComponent<ShipScript>().IsShipFitt(placesOccupiedByShips, lastHoweredTilePosition);
        }

        public void PlaceShipOnLastPlace()
        {
            PlaceShip();
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

            currentShoot = Instantiate(_shootingPinPrefub).GetComponent<ShootingPin>();
            currentShoot.gameObject.transform.parent = _playgroundAncor.transform;
            currentShoot.transform.localPosition = new Vector3(10, 0, -4);
            TileHoverChangedEvent.RemoveAllListeners();
            TileHoverChangedEvent.AddListener(ShootTraker);
            currentShoot.ChangePinState(ShootingPin.ShootingPinState.Plasing);
        }

        private void ShootTraker(Vector2Int hoveredTile)
        {
            if (gameFieldState == GameFieldState.None) return;
            lastHoweredTilePosition = hoveredTile;
            PlaceShoot(true);
        }

        private void PlaceShoot(bool checkOnly = false)
        {            
            var pinInPlace = shootedPlayses[lastHoweredTilePosition.x][lastHoweredTilePosition.y];
            if (pinInPlace == null)
            {
                currentShoot.ChangePinState(ShootingPin.ShootingPinState.Plasing);
            }
            else
            {
                currentShoot.ChangePinState(ShootingPin.ShootingPinState.WrongPosition);
            }

            currentShoot.gameObject.transform.localPosition = new Vector3(lastHoweredTilePosition.x, 0, -lastHoweredTilePosition.y);

            if (checkOnly) return;

            if(currentShoot.CurrentState == ShootingPin.ShootingPinState.Plasing)
            {
                shootedPlayses[lastHoweredTilePosition.x][lastHoweredTilePosition.y] = currentShoot;
                TileHoverChangedEvent.RemoveAllListeners();
                gameFieldState = GameFieldState.None;
                currentShoot = null;
                playerShootsCallback?.Invoke(new Vector2Int[] { lastHoweredTilePosition });
            }
        }

        public void PlaceShootOn(Vector2Int position)
        {
            lastHoweredTilePosition = position;
            PlaceShoot(false);
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
                            var destroyedPart = Instantiate(_destrooyedEnemyShipPartIndicator);
                            destroyedPart.transform.parent = _playgroundAncor.transform;
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

                currentShoot = Instantiate(_shootingPinPrefub).GetComponent<ShootingPin>();
                currentShoot.gameObject.transform.parent = _playgroundAncor.transform;
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
