using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu {
    public enum SelectableType { Button, SideSet }

	public abstract class Selectable : MonoBehaviour {
		[SerializeField]
		private Graphic[] targetGraphics;
        private TextMeshProUGUI[] textMeshUIs;
        private TextMeshProUGUI[] TextMeshUIs { get {
            if (textMeshUIs == null) {
                List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
                for (int i = 0; i < targetGraphics.Length; i++)
                {
                    TextMeshProUGUI text = targetGraphics[i].GetComponent<TextMeshProUGUI>();
                    if (text != null) texts.Add(text);
                }
                textMeshUIs = texts.ToArray();
            }

            return textMeshUIs;
        } }
		[SerializeField]
		private SelectableStyle style;
		[System.NonSerialized]
		protected bool selected, actived, disabled;

        public bool Disable {
            get { return disabled; }
            set {
                disabled = value;
                ApplyStyle();
            }
        }

    #if UNITY_EDITOR
        protected void Reset() {
            targetGraphics = GetComponentsInChildren<Graphic>();
        }
    #endif

        public abstract SelectableType Type { get; }

		public abstract bool Left(ref Selectable menuSelected);
        public abstract bool Right(ref Selectable menuSelected);
        public abstract bool Up(ref Selectable menuSelected);
		public abstract bool Down(ref Selectable menuSelected);
        public abstract void Submit();

        protected virtual void Awake() {
            ApplyStyle();
        }

        /// <summary>
        /// Change state of the selectable
        /// </summary>
        /// <value>Selected or not</value>
        public bool Select {
            get { return selected; }
            set {
                selected = value;
                ApplyStyle();

                // TODO: play menu selcet audio
            }
        }

        public void SelectSilently(bool value) {
            selected = value;
            ApplyStyle();
        }

        /// <summary>
        /// Apply the style according to the state
        /// </summary>
		public void ApplyStyle() {
            if (style == null) return;

			Color color = style.NormalColor;

            if (disabled)
            {
                color = style.DisabledColor;
            }
            else if (actived)
            {
                color = style.ActiveColor;
            }
            else if (selected)
            {
                color = style.SelectedColor;
            }

            for (int i = 0; i < targetGraphics.Length; i++) targetGraphics[i].color = color;
		}

        /// <summary>
        /// Navigation function
        /// </summary>
        /// <param name="menuSelected">Refrence value from manager</param>
        /// <param name="selectable">The potencial nav selectable</param>
        /// <returns>Wether refrence selectable changed</returns>
        protected bool ChangeNav(ref Selectable menuSelected, Selectable selectable)
        {
            if (selectable == null) return false;
            selected = false;
            menuSelected = selectable;
            menuSelected.Select = true;
            ApplyStyle();
            return true;
        }
	}

	public class SelectableButton : Selectable {
    #if UNITY_EDITOR
        [MenuItem("GameObject/Menu/Selectable Button", false)]
        static public void OnCreate() {
            GameObject obj = new GameObject("Selectable Button", typeof(RectTransform));

            if (Selection.activeGameObject) {
                obj.GetComponent<RectTransform>().parent = Selection.activeGameObject.transform;
            } else {
                obj.GetComponent<RectTransform>().parent = FindObjectOfType<Canvas>().transform;
            }
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            obj.AddComponent<SelectableButton>();

            Selection.activeGameObject = obj;
        }
    #endif

        public override SelectableType Type { get { return SelectableType.Button; } }

		[SerializeField]
		private Selectable left, right, up, down;

		[SerializeField]
		private UnityEvent submitEvent;

        /// <summary>
        /// Navigate to Left selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Left(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, left); }

        /// <summary>
        /// Navigate to Right selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Right(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, right); }

        /// <summary>
        /// Navigate to Up selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Up(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, up); }

        /// <summary>
        /// Navigate to Down selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Down(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, down); }

        /// <summary>
        /// Execute submit event
        /// </summary>
        public override void Submit() {
            if (disabled || actived) return;
            // TODO: play menu click audio
            submitEvent.Invoke();
        }
	}
}