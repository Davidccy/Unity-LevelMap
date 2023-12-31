using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIPlayer : MonoBehaviour {
    public enum MoveType {
        FixedSpeed,
        FixedTime,
    }

    #region Serialized Fields
    [SerializeField] private UILevelNode _initLevel = null;
    [SerializeField] private MoveType _moveType = MoveType.FixedSpeed;
    [SerializeField] private float _fixedSpeed = 100; // Distance per second
    [SerializeField] private float _fixedTime = 10; // Seconds
    #endregion

    #region Internal Fields
    private UILevelNode _currentLevel = null;
    private bool _isMoving = false;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        SetCurrentLevel(_initLevel);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(_initLevel.UIRouteUp);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(_initLevel.UIRouteDown);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(_initLevel.UIRouteLeft);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(_initLevel.UIRouteRight);
        }
    }
    #endregion

    #region APIs
    public void SetCurrentLevel(UILevelNode levelNode) {
        _currentLevel = levelNode;
        RefreshPosition();
    }
    #endregion

    #region Internal Methods
    private void RefreshPosition() {
        if (_currentLevel == null) {
            return;
        }

        RectTransform rect = (RectTransform) this.transform;
        Vector2 pos = Utility.ConvertRectTransformToCanvasPosition((RectTransform) _currentLevel.transform);
        rect.anchoredPosition = pos;
    }

    private async void Move(UIRoute route) {
        if (route == null) {
            return;
        }

        //if (!route.IsOpened) {
        //    return;
        //}

        if (_isMoving) {
            return;
        }

        _isMoving = true;

        RectTransform rectSelf = (RectTransform) this.transform;

        bool isFromStart = _currentLevel == route.LevelNodeStart;
        List<Vector2> pointList = route.GetRoutePoints(isFromStart);
        UIRoute.RouteType rType = route.RType;

        if (_moveType == MoveType.FixedSpeed) {
            if (rType == UIRoute.RouteType.Linear) {
                float totalDistance = 0;
                List<float> distanceList = new List<float>();
                for (int i = 0; i < pointList.Count - 1; i++) {
                    float distance = (pointList[i + 1] - pointList[i]).magnitude;
                    totalDistance += distance;
                    distanceList.Add(distance);
                }

                for (int i = 0; i < pointList.Count - 1; i++) {
                    float duration = distanceList[i] / _fixedSpeed;
                    Tweener t = rectSelf.DOAnchorPos(pointList[i + 1], duration).SetUpdate(true).SetEase(Ease.Linear);
                    while (t != null && t.IsActive()) {
                        await Task.Delay(1);
                    }
                }
            }
            else if (rType == UIRoute.RouteType.Bezier) {
                // TODO:

                // NOTE:
                // Convert speed to interpolation
            }
            else {
                Debug.LogErrorFormat("Unexpected route type {0}", rType);
            }
        }
        else if (_moveType == MoveType.FixedTime) {
            if (rType == UIRoute.RouteType.Linear) {
                float totalDistance = 0;
                List<float> distanceList = new List<float>();
                for (int i = 0; i < pointList.Count - 1; i++) {
                    float distance = (pointList[i + 1] - pointList[i]).magnitude;
                    totalDistance += distance;
                    distanceList.Add(distance);
                }

                for (int i = 0; i < pointList.Count - 1; i++) {
                    float duration = distanceList[i] / totalDistance * _fixedTime;
                    Tweener t = rectSelf.DOAnchorPos(pointList[i + 1], duration).SetUpdate(true).SetEase(Ease.Linear);
                    while (t != null && t.IsActive()) {
                        await Task.Delay(1);
                    }
                }
            }
            else if (rType == UIRoute.RouteType.Bezier) {
                float progress = 0;
                Tweener t = DOTween.To(
                    () => progress,
                    (v) => {
                        progress = v;
                        rectSelf.localPosition = BezierUtility.GetInterpolationPosition(pointList, progress);
                    },
                    1.0f,
                    _fixedTime)
                    .SetUpdate(true).SetEase(Ease.Linear);

                while (t != null && t.IsActive()) {
                    await Task.Delay(1);
                }
            }
            else {
                Debug.LogErrorFormat("Unexpected route type {0}", rType);
            }
        }
        else {
            Debug.LogErrorFormat("Unexpected move type {0}", _moveType);
        }

        _isMoving = false;
        _currentLevel = isFromStart ? route.LevelNodeEnd : route.LevelNodeStart;
    }
    #endregion
}
