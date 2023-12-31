using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBezierCurveRenderer : Image {
    // Reference:
    // Normal line calculating
    // https://stackoverflow.com/questions/43547886/is-it-really-so-difficult-to-draw-smooth-lines-in-unity/51958673#51958673    

    #region Serialized Fields
    [Header("Test")]
    [SerializeField] private float _thickness = 0;
    [SerializeField] private Gradient _gradient = new Gradient();

    [Range(0, 1)]
    [SerializeField] private float _durationStart = 0;
    [Range(0, 1)]
    [SerializeField] private float _durationEnd = 0;
    #endregion

    #region internal Fields
    private List<Vector2> _pointList = new List<Vector2>();
    #endregion

    #region APIs
    public void SetPointList(List<Vector2> pointList) {
        _pointList = pointList;

        this.SetAllDirty();
    }
    #endregion

    // First version of implementation
    // Nothing but only lines composed by points
    //protected override void OnPopulateMesh(VertexHelper vh) {
    //    Debug.LogErrorFormat("on pupulate mesh");

    //    if (_pointList == null || _pointList.Count < 0) {
    //        return;
    //    }

    //    vh.Clear();
    //    for (int i = 0; i < _pointList.Count; i++) {
    //        Vector3 v1 = new Vector3(_pointList[i].x, _pointList[i].y + 100);
    //        Vector3 v2 = new Vector3(_pointList[i].x, _pointList[i].y - 100);

    //        vh.AddVert(v1, color, Vector2.zero);
    //        vh.AddVert(v2, color, Vector2.zero);
    //    }

    //    int totalVertex = _pointList.Count * 2;
    //    for (int i = 0; i < totalVertex - 3; i += 2) {
    //        int index0 = i;
    //        int index1 = i + 1;
    //        int index2 = i + 2;
    //        int index3 = i + 3;

    //        vh.AddTriangle(index0, index1, index2);
    //        vh.AddTriangle(index1, index2, index3);
    //    }
    //}

    // Second version of implementation
    // Add calculation of Normal Line, and determine color by Gradient
    protected override void OnPopulateMesh(VertexHelper vh) {
        if (_pointList == null || _pointList.Count < 1) {
            return;
        }

        vh.Clear();

        // Calculate normal vectors
        List<Vector2> normalVectorList = new List<Vector2>();
        for (int i = 0; i < _pointList.Count - 1; i++) {
            Vector2 directionalVector = _pointList[i + 1] - _pointList[i];
            Vector2 nVector = new Vector2(directionalVector.y, -directionalVector.x);
            nVector.Normalize();
            normalVectorList.Add(nVector);
        }

        // Add
        Vector2 finalNormalVector = normalVectorList[normalVectorList.Count - 1];
        int totalPointCount = _pointList.Count;
        Vector2 uvTopLeft = new Vector2(0, 1);
        Vector2 uvBottomLeft = new Vector2(0, 0);
        for (int i = 0; i < _pointList.Count; i++) {
            Vector2 nVector = i < normalVectorList.Count ? normalVectorList[i] : finalNormalVector;

            Vector2 pointUp = _pointList[i] + nVector * _thickness;
            Vector2 pointDown = _pointList[i] - nVector * _thickness;

            // Color
            float time = (float) i / totalPointCount;
            Color c = IsInDuration(time) ? Color.black : Color.white;

            // test
            c = IsInDuration(time) ? _gradient.Evaluate(time) : Color.white;
            // test

            vh.AddVert(pointUp, c, uvTopLeft);
            vh.AddVert(pointDown, c, uvBottomLeft);
        }

        int totalVertex = vh.currentVertCount;
        for (int i = 0; i < totalVertex - 3; i += 2) {
            int index0 = i;
            int index1 = i + 1;
            int index2 = i + 2;
            int index3 = i + 3;

            vh.AddTriangle(index0, index1, index2);
            vh.AddTriangle(index1, index2, index3);
        }
    }

    private bool IsInDuration(float time) {
        if (_durationStart == _durationEnd) {
            return false;
        }

        if (time >= _durationStart && time <= _durationEnd) {
            return true;
        }

        if (time <= _durationStart && time >= _durationEnd) {
            return true;
        }

        return false;
    }
}
