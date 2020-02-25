using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BirdSight;

namespace Menu {
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        Selectable selected;
        Selectable defaultSelected;

        Canvas canvas;

        PlayerInputMap inputActions;


        void Awake() {
            canvas = GetComponent<Canvas>();

            selected.SelectSilently(true);
            defaultSelected = selected;

            SetupInput();

            canvas.enabled = enabled = false;
        }

        private void OnEnable() { inputActions.Enable(); }
        private void OnDisable() { inputActions.Disable(); }

        void SetupInput() {
            inputActions = new PlayerInputMap();

            inputActions.UI.Up.performed += delegate { selected.Up(ref selected); };
            inputActions.UI.Down.performed += delegate { selected.Down(ref selected); };
            inputActions.UI.Cancel.performed += delegate { GameMgr.mgr.ResumeGame(); };
            inputActions.UI.Submit.performed += delegate { selected.Submit(); };
        }

        public void Activate() {
            enabled = canvas.enabled = true;

            if (selected != defaultSelected) {
                selected.Select = false;
                selected = defaultSelected;
                selected.SelectSilently(true);
            }
        }

        public void Deactivate() {
            enabled = canvas.enabled = false;
        }

    }
}