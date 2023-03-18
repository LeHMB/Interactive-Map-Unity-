using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MarkerController))]
public class MarkerControllerEditor : Editor
{
    //SerializedProperty markersTest;
    SerializedProperty content;
    SerializedProperty image;
    SerializedProperty mapRenderer;

    void OnEnable()
    {
        //markersTest = serializedObject.FindProperty("markersTest");
        content = serializedObject.FindProperty("content");
        image = serializedObject.FindProperty("image");
        mapRenderer = serializedObject.FindProperty("mapRenderer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.PropertyField(markersTest);
        EditorGUILayout.PropertyField(content);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.PropertyField(mapRenderer);
        serializedObject.ApplyModifiedProperties();
    }
}
