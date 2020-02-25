using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct Timer {
    public float TargetTime, RunTime;

    public Timer(float time) {
        TargetTime = time;
        RunTime = 0;
    }

    public void Reset() {
        RunTime = 0;
    }

    public float Progress { get {
        return Mathf.Clamp(RunTime / TargetTime, 0, 1);
    } }
    public bool Ended { get {
        return RunTime >= TargetTime;
    } }
    public bool UpdateEnd { get {
        RunTime += Time.deltaTime;
        return RunTime >= TargetTime;
    } }
    public bool FixedUpdateEnd { get {
        RunTime += Time.fixedDeltaTime;
        return RunTime >= TargetTime;
    } }
    public bool UnscaleUpdateTimeEnd { get {
        RunTime += Time.unscaledDeltaTime;
        return RunTime >= TargetTime;
    } }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Timer))]
    public class _PropertyDrawer: PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.width = position.width / 2 - 5;
            EditorGUI.LabelField(position, label);
            position.x += position.width + 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("TargetTime"), GUIContent.none);
        }
    }
#endif
}