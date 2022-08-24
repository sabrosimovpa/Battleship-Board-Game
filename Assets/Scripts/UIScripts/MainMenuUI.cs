using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleshipBoardGame.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform startMenu;
        [SerializeField]
        private RectTransform[] menuObjects;

        private Stack<RectTransform> _navigationStack = new Stack<RectTransform>();

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < menuObjects.Length; i++)
            {
                menuObjects[i].gameObject.SetActive(false);
            }

            NavigateTo(startMenu);
        }

        public void NavigateTo(RectTransform newMenuObject)
        {
            if(_navigationStack.Count > 0)
            {
                _navigationStack.Peek().gameObject.SetActive(false);
            }

            newMenuObject.gameObject.SetActive(true);
            _navigationStack.Push(newMenuObject);
        }

        public void GoBack()
        {
            if(_navigationStack.Count > 1)
            {
                var current = _navigationStack.Pop();
                current.gameObject.SetActive(false);
                _navigationStack.Pop().gameObject.SetActive(true);
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
                _navigationStack.Pop().gameObject.SetActive(false);
            }

            _navigationStack.Clear();
#if UNITY_EDITOR
            NavigateTo(startMenu);
#endif
            Application.Quit();
        }
    }
}
