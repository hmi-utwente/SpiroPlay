using System.Collections;
using System.Collections.Generic;
using Spirometry.SpiroController;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( SpiroManager ), true )]
public class CustomInspector_SpiroManager : Editor
{
    public override void OnInspectorGUI()
    {
        SpiroManager myTarget = (SpiroManager) target;
        
        //add functionality for overriding event calls
        EditorGUILayout.HelpBox("Use the button below to force a 'OnReachProficientFlow' event call", MessageType.Info);
        if(GUILayout.Button("Simulate peak flow"))
        {
            myTarget.ReachedPeakFlowOverride();
        }
        
        //draw the default inspector fields, with some space before and after
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //add functionality for simulating fake breath tests
        EditorGUILayout.HelpBox("Use the buttons below to simulate a perfect breath test", MessageType.Info);
        if(GUILayout.Button("Simulate real breath"))
        {
            myTarget.StartCoroutine(myTarget.SimulatePerfectBreath());
        }
    }
}
