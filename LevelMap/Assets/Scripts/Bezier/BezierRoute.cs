using System.Collections.Generic;
using UnityEngine;

public class BezierRoute : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Transform[] _controlPoints = null;
    //[SerializeField] private int _totalPoint = 1;
    #endregion

    #region Internal Fields
    private Vector2 _gizmosPosition = Vector2.zero;
    #endregion

    #region Gizmos Handling
    private void OnDrawGizmos() {
        if (_controlPoints == null || _controlPoints.Length <= 1) {
            return;
        }

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < _controlPoints.Length; i++) {
            points.Add(_controlPoints[i].position);
        }

        for (float t = 0; t <= 1; t += 0.05f) {
            _gizmosPosition = BezierUtility.GetInterpolationPosition(points, t);
            Gizmos.DrawSphere(_gizmosPosition, 5f);
        }

        Gizmos.color = Color.white;
        for (int i = 1; i < _controlPoints.Length; i++) {
            Gizmos.DrawLine(
            new Vector2(_controlPoints[i - 1].position.x, _controlPoints[i - 1].position.y),
            new Vector2(_controlPoints[i].position.x, _controlPoints[i].position.y)
            );
        }
    }
    #endregion
}
