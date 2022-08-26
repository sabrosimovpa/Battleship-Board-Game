using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleshipBoardGame
{
    public class PlaygroundTile : MonoBehaviour
    {
        public Vector2Int Position;

        public UnityEvent<PlaygroundTile, TileEventType> TileMouseEvent = new UnityEvent<PlaygroundTile, TileEventType>();

        // Start is called before the first frame update
        void Start()
        {
            Position = new Vector2Int(Mathf.RoundToInt(gameObject.transform.localPosition.x), Mathf.RoundToInt(-gameObject.transform.localPosition.z));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnMouseOver()
        {
            
        }

        private void OnMouseEnter()
        {            
            TileMouseEvent?.Invoke(this, TileEventType.MouseEnter);
        }

        private void OnMouseExit()
        {
            TileMouseEvent?.Invoke(this, TileEventType.MouseExit);            
        }

        public enum TileEventType
        {
            MouseEnter,
            MouseExit
        }
    }
}
