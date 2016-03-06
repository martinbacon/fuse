using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Audio;

[CustomEditor(typeof(GameManager))]
public class GameManagerEdtior : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;
        if (GUILayout.Button("Add one kill"))
        {
            gm.Kill();
        }
    }
}
