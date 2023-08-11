using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePlanet))]
public class GeneratePlanetMenu : Editor {
    /*
     * Create a planet creation button on the inspector
     */
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("GenerateMapRange value must be less than the PlanetDetail value!", MessageType.Warning);

        DrawDefaultInspector();
        
        if (GUILayout.Button("GeneratePlanet"))
        {
            GeneratePlanet generatePlanet = (GeneratePlanet)target;
           
            generatePlanet._GeneratePlanet();
        }
    }
}
