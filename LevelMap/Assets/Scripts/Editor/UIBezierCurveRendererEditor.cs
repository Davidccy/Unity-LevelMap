using UnityEditor;

[CustomEditor(typeof(UIBezierCurveRenderer))]
public class UIBezierCurveRendererEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
}
