using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmartBoxCollider))]
public class SmartBoxColliderEditor : Editor {
    SmartBoxCollider collider;

    private void OnEnable() {
        collider = (SmartBoxCollider) target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("groundLayer"));

        EditorGUILayout.IntSlider(serializedObject.FindProperty("horizontalDetectPointCount"), 2, 10, "Hor detect point count");
        EditorGUILayout.IntSlider(serializedObject.FindProperty("verticalDetectPointCount"), 2, 10, "Ver detect point count");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftRayDis"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightRayDis"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upRayDis"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("downRayDis"));

        serializedObject.ApplyModifiedProperties();
    }
}