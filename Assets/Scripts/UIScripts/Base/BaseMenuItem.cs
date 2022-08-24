using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.UI.Base
{
    public abstract class BaseMenuItem : MonoBehaviour
    {
        public abstract void OnMenuLoad();

        public abstract void OnMenuUnload();
    }
}
