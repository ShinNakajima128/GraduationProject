using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScenarioData))]
public class ScenarioDataCustomInspector : Editor
{
    ScenarioData _scenarioData;

    private void OnEnable()
    {
        _scenarioData = target as ScenarioData;
    }

    public override void OnInspectorGUI()
    {
        if (!_scenarioData) return;

        if (GUILayout.Button("DialogUpdate"))
        {
            _scenarioData.LoadDialogDataFromSpreadsheet(_scenarioData.URL, _scenarioData.ScenarioSheetName);
        }

        base.OnInspectorGUI();
        EditorUtility.SetDirty(_scenarioData);
    }
}
