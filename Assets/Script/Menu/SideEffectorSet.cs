using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu {
	public class SideEffectorSet : Selectable {
	#if UNITY_EDITOR
		[MenuItem("GameObject/Menu/Side Effector Set", false, 1)]
		static public void OnCreate()
		{
			GameObject obj = new GameObject("Side Effector Set", typeof(RectTransform));

			if (Selection.activeGameObject)
			{
				obj.GetComponent<RectTransform>().parent = Selection.activeGameObject.transform;
			}
			else
			{
				obj.GetComponent<RectTransform>().parent = FindObjectOfType<Canvas>().transform;
			}
			obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			obj.AddComponent<SideEffectorSet>();

			Selection.activeGameObject = obj;
		}
	#endif

        public override SelectableType Type { get { return SelectableType.SideSet; } }

        [SerializeField]
		private Selectable up, down;

		[SerializeField]
		private UnityEvent leftEvent, righEvent;

		protected override void Awake() {
			base.Awake();
		}

		/// <summary>
		/// Make left effector look like been selected, execute left event
		/// </summary>
		/// <param name="menuSelected">Refrence selectable will never be changed it here</param>
		/// <returns>Always false</returns>
		public override bool Left(ref Selectable menuSelected) {
			// TODO: play menu side audio
			leftEvent.Invoke();
			return true;
		}

        /// <summary>
        /// Make right effector look like been selected, execute left event
        /// </summary>
        /// <param name="menuSelected">Refrence selectable will never be changed it here</param>
        /// <returns>Always false</returns>
        public override bool Right(ref Selectable menuSelected) {
			// TODO: play menu side audio
			righEvent.Invoke();
			return true;
		}

        /// <summary>
        /// Navigate to Up selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Up(ref Selectable menuSelected) {
			return ChangeNav(ref menuSelected, up);
		}

        /// <summary>
        /// Navigate to Down selectable
        /// </summary>
        /// <param name="menuSelected">Refrence varrible from manager</param>
        /// <returns>Wether refrence selectable changed<</returns>
        public override bool Down(ref Selectable menuSelected) {
			return ChangeNav(ref menuSelected, down);
		}
		public override void Submit() {}
	}
}