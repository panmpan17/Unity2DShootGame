using UnityEngine;

namespace Menu {
	[CreateAssetMenu(menuName="Selectable Item Style")]
	public class SelectableStyle : ScriptableObject {
		public Color NormalColor;
		public Color ActiveColor;
		public Color SelectedColor;
		public Color DisabledColor;
	}
}