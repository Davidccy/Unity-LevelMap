using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierFollow : MonoBehaviour {
    // Reference to video from youtube
    // https://www.youtube.com/watch?v=11ofnLOE8pw

    #region Serialized Fields
    [SerializeField] private Transform[] _routes = null;

    [Range(0.1f, 2.0f)]
    [SerializeField] private float _speedModifier = 0; // Interpolation per second
    #endregion

    #region Internal Fields
    private int _routeToGo;
    private float interpolation;
    private Vector2 _targetPosition; // Player or arrow
    private bool _coroutineAllowed;
    #endregion

    #region Mono Behaviour Hooks
    private void Start() {
        Init();        
    }

    private void Update() {
        if (_coroutineAllowed) {
            StartCoroutine(GoByTheRoute(_routeToGo));
        }
    }
    #endregion

    #region Internal Methods
    private void Init() {
        _routeToGo = 0;
        interpolation = 0f;
        _coroutineAllowed = true;
    }

    private IEnumerator GoByTheRoute(int routeNumber) {
        _coroutineAllowed = false;

        List<Vector2> points = GetPoints(_routes[routeNumber]);

        while (interpolation < 1) {
            interpolation += Time.deltaTime * _speedModifier;

            _targetPosition = BezierUtility.GetInterpolationPosition(points, interpolation);

            transform.position = _targetPosition;

            yield return new WaitForEndOfFrame();
        }

        interpolation = 0f;

        _routeToGo += 1;

        if (_routeToGo > _routes.Length - 1) {
            _routeToGo = 0;
        }

        _coroutineAllowed = true;
    }

    private List<Vector2> GetPoints(Transform routeRoot) {
        List<Vector2> points = new List<Vector2>();
        if (routeRoot == null) {
            return points;
        }

        int childCount = routeRoot.childCount;
        for (int i = 0; i < childCount; i++) {
            points.Add(routeRoot.GetChild(i).position);
        }

        return points;
    }
    #endregion
}
