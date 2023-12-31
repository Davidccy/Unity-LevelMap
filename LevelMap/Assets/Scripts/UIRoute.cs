using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIRoute : MonoBehaviour {
    public enum RouteType { 
        Linear,
        Bezier,
    }

    #region Serialized Fields
    [SerializeField] private Canvas _canvas = null;

    [SerializeField] private UILevelNode _levelNodeStart = null;
    [SerializeField] private UILevelNode _levelNodeEnd = null;

    [SerializeField] private RouteType _routeType = RouteType.Linear;
    [SerializeField] private List<RectTransform> _passageNodeList = new List<RectTransform>();
    [SerializeField] private RectTransform _rectResRoute = null;
    [SerializeField] private RectTransform _rectRouteRoot = null;

    [SerializeField] private float _routeThickness = 10;
    [SerializeField] private float _routeDrawSpeed = 10;

    [Header("Bezier")]
    [Range(5, 100)]
    [SerializeField] private int _pointDensity = 0;
    [SerializeField] private UIBezierCurveRenderer _rectResBezierRoute = null;
    #endregion

    #region Internal Fields
    private RectTransform _rectCanvas = null; // For gizmos
    private bool _isOpened = false;
    #endregion

    #region Properties
    public bool IsOpened {
        get {
            return _isOpened;
        }
    }

    public UILevelNode LevelNodeStart {
        get {
            return _levelNodeStart;
        }
    }

    public UILevelNode LevelNodeEnd {
        get {
            return _levelNodeEnd;
        }
    }

    public RouteType RType {
        get {
            return _routeType;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        SetRouteOpened(false);
    }
    #endregion

    #region APIs
    public void SetRouteOpened(bool opened, bool skipAnimation = true, bool fromStart = true) {
        _isOpened = opened;
        DrawRoutes();
    }

    public List<Vector2> GetRoutePoints(bool fromStart = true) {
        List<Vector2> pointList = new List<Vector2>();
        pointList.Add(Utility.ConvertRectTransformToCanvasPosition((RectTransform) _levelNodeStart.transform));
        for (int i = 0; i < _passageNodeList.Count; i++) {
            pointList.Add(Utility.ConvertRectTransformToCanvasPosition(_passageNodeList[i]));
        }
        pointList.Add(Utility.ConvertRectTransformToCanvasPosition((RectTransform) _levelNodeEnd.transform));

        if (!fromStart) {
            pointList.Reverse();
        }

        return pointList;
    }
    #endregion

    #region Internal Methods
    private async void DrawRoutes() {
        List<Vector2> pointList = GetRoutePoints();
        if (_routeType == RouteType.Linear) {
            for (int i = 0; i < pointList.Count - 1; i++) {
                await DrawRouteLinear(pointList[i], pointList[i + 1]);
            }
        }
        else {
            await DrawRouteBezier(pointList);
        }
    }

    private float GetVectorDegree(Vector2 normalizedVector) {
        float tanX = normalizedVector.x != 0 ? normalizedVector.y / normalizedVector.x : 0;
        float x = Mathf.Atan(tanX);
        if (normalizedVector.x == 0) {
            if (normalizedVector.y > 0) {
                x = Mathf.PI / 2;
            }
            else {
                x = -Mathf.PI / 2;
            }
        }
        else if (normalizedVector.x < 0) {
            if (normalizedVector.y > 0) {
                x += Mathf.PI;
            }
            else {
                x -= Mathf.PI;
            }
        }
        float xDegree = x * Mathf.Rad2Deg;

        return xDegree;
    }

    private async Task DrawRouteLinear(Vector2 startPos, Vector2 endPos) {
        RectTransform newRect = Instantiate(_rectResRoute, _rectRouteRoot);
        newRect.gameObject.SetActive(true);
        newRect.anchorMin = new Vector2(0, 0.5f);
        newRect.anchorMax = new Vector2(0, 0.5f);
        newRect.pivot = new Vector2(0.0f, 0.5f);

        float length = (endPos - startPos).magnitude;
        newRect.sizeDelta = new Vector2(length, 10);
        newRect.anchoredPosition = startPos + Vector2.right * _rectRouteRoot.rect.width * 0.5f;
        newRect.localEulerAngles = new Vector3(0, 0, GetVectorDegree((endPos - startPos).normalized));

        float animationLength = 0;
        while (animationLength < length) {
            newRect.sizeDelta = new Vector2(animationLength, _routeThickness);
            animationLength += _routeDrawSpeed;
            await Task.Delay(10);
        }
    }

    private async Task DrawRouteBezier(List<Vector2> pointList) {
        // NOTE:
        // Old method
        //UIBezierCurveRenderer uiBCRenderer = Instantiate(_rectResBezierRoute, _rectRouteRoot);
        //uiBCRenderer.gameObject.SetActive(true);
        //List<Vector2> bPointList = new List<Vector2>();
        //for (int i = 0; i < _pointDensity + 1; i++) {
        //    Vector2 p = BezierUtility.GetInterpolationPosition(pointList, (float) (i) / _pointDensity);
        //    bPointList.Add(p);
        //}
        //uiBCRenderer.SetPointList(bPointList);
        // Old method

        UIBezierCurveRenderer uiBCRenderer = Instantiate(_rectResBezierRoute, _rectRouteRoot);
        uiBCRenderer.gameObject.SetActive(true);

        List<Vector2> bPointList = new List<Vector2>();
        for (int i = 0; i < _pointDensity + 1; i++) {
            Vector2 p = BezierUtility.GetInterpolationPosition(pointList, (float) (i) / _pointDensity);
            bPointList.Add(p);
        }

        for (int i = 2; i < bPointList.Count; i++) {
            List<Vector2> ss = bPointList.GetRange(0, i);
            uiBCRenderer.SetPointList(ss);
            await Task.Delay(10);
        }
    }
    #endregion

    #region Gizmos Handling
    private void OnDrawGizmos() {
        if (_canvas == null) {
            return;
        }

        if (_rectCanvas == null) {
            _rectCanvas = (RectTransform) _canvas.transform;
        }

        if (_levelNodeStart == null || _levelNodeEnd == null) {
            return;
        }

        // Draw lines
        List<Vector2> pointList = new List<Vector2>();
        pointList.Add(_levelNodeStart.transform.position);
        for (int i = 0; i < _passageNodeList.Count; i++) {
            pointList.Add(_passageNodeList[i].position);
        }
        pointList.Add(_levelNodeEnd.transform.position);

        if (_routeType == RouteType.Linear) {
            for (int i = 0; i < pointList.Count - 1; i++) {
                Gizmos.DrawLine(pointList[i], pointList[i + 1]);
            }
        }
        else {
            for (int i = 0; i < _pointDensity - 1; i++) {
                Vector2 from = BezierUtility.GetInterpolationPosition(pointList, (float) (i) / _pointDensity);
                Vector2 to = BezierUtility.GetInterpolationPosition(pointList, (float) (i + 1) / _pointDensity);

                Gizmos.DrawLine(from, to);
            }
        }

        // Draw spheres
        Gizmos.color = Color.red;
        for (int i = 0; i < pointList.Count; i++) {
            Gizmos.DrawSphere(pointList[i], 20f);
        }
    }
    #endregion
}
