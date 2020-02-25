using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BirdSight;

namespace Menu {
    public class MainMenu : MonoBehaviour
    {
        // static private Color transparent = new Color(1, 1, 1, 0);
        [SerializeField]
        Selectable selected;
        Selectable defaultSelected;

        Canvas canvas;
        public Canvas Canvas { get { return canvas; } }

        PlayerInputMap inputActions;

        void Awake()
        {
            canvas = GetComponent<Canvas>();

            selected.SelectSilently(true);
            defaultSelected = selected;

            SetupInput();

            enabled = false;
            // Activate();
        }

        private void OnEnable() { inputActions.Enable(); }
        private void OnDisable() { inputActions.Disable(); }

        void SetupInput()
        {
            inputActions = new PlayerInputMap();

            inputActions.UI.Up.performed += delegate { selected.Up(ref selected); };
            inputActions.UI.Down.performed += delegate { selected.Down(ref selected); };
            inputActions.UI.Cancel.performed += delegate { GameMgr.mgr.ResumeGame(); };
            inputActions.UI.Submit.performed += delegate { selected.Submit(); };
        }

        public void Activate()
        {
            enabled = canvas.enabled = true;
        }

        public void FadeIn() {
            GetComponent<Animator>().SetTrigger("FadeIn");

            if (selected != defaultSelected)
            {
                selected.Select = false;
                selected = defaultSelected;
                selected.SelectSilently(true);
            }
        }

        public void StartGamePressed() {
            GetComponent<Animator>().SetTrigger("FadeOut");
        }

        public void FadeOutFinished() {
            GameMgr.mgr.GameRestart();
            canvas.enabled = enabled = false;
        }

        public void Quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }

}