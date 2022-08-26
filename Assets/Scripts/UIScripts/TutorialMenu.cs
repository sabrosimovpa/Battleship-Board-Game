using BattleshipBoardGame.UI.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.UI
{
    public class TutorialMenu : BaseMenuItem
    {
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void OnMenuLoad()
        {
            gameObject.SetActive(true);
        }

        public override void OnMenuUnload()
        {
            gameObject.SetActive(false);
        }
    }
}
