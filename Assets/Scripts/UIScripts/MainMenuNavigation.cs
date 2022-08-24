using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleshipBoardGame.UI.Base;

namespace BattleshipBoardGame.UI
{
    public class MainMenuNavigation : MonoBehaviour
    {
        [SerializeField]
        private BaseMenuItem startMenu;
        [SerializeField]
        private BaseMenuItem[] menuObjects;

        private Stack<BaseMenuItem> _navigationStack = new Stack<BaseMenuItem>();

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < menuObjects.Length; i++)
            {
                menuObjects[i].gameObject.SetActive(false);
            }

            NavigateTo(startMenu);
        }

        public void NavigateTo(BaseMenuItem newMenuObject)
        {
            if(_navigationStack.Count > 0)
            {
                _navigationStack.Peek().OnMenuUnload();
            }

            newMenuObject.OnMenuLoad();
            _navigationStack.Push(newMenuObject);
        }

        public void GoBack()
        {
            if(_navigationStack.Count > 1)
            {
                var current = _navigationStack.Pop();
                current.OnMenuUnload();
                _navigationStack.Pop().OnMenuLoad();
            }
            else
            {
                Application.Quit();
            }    
        }

        public void ApplicationQuit()
        { 
            if(_navigationStack.Count > 0)
            {
                _navigationStack.Pop().OnMenuUnload();
            }

            _navigationStack.Clear();
#if UNITY_EDITOR
            NavigateTo(startMenu);
#endif
            GameManager.instance.ExitGame();
        }
    }
}
