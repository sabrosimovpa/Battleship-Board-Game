using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.AI
{
    public interface IAI
    {
        public bool Ready { get; }
        public UnityEngine.Events.UnityEvent<bool> AIReadyToFight { get; }

        public bool IsAlive { get; }

        public void SetubAI(GameObject[] newShips, GameFieldManager shipsField, GameFieldManager targetingField);

        public void StartShooting();
    }
}
