using UnityEngine;
using DG.Tweening;

public class UILevelMapPlayer : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _bouncingExtraScale = 1.5f;
    //[SerializeField] private float _bouncingMaxHeight = 10f;
    [SerializeField] private float _bouncingFreq = 1f;
    [SerializeField] private RectTransform _rectRoot = null;
    #endregion

    #region Internal Fields
    private UILevelMapNode _currentLevel;

    private Tween _tweenMove = null;
    private bool _isMoving = false;
    
    private Tween _tweenBouncing = null;
    private bool _isBouncing = false;
    #endregion

    #region Properties
    public bool IsMoving {
        get {
            return _isMoving;
        }
    }

    public bool IsBouncing {
        get {
            return _isBouncing;
        }
    }
    #endregion

    #region Internal Methods
    private void StartBouncing() {
        StopBouncing();

        _isBouncing = true;

        float progress = 0;
        _tweenBouncing = DOTween.To(
            () => progress,
            v => {
                progress = v;
                _rectRoot.localScale = Vector3.one * (1 + Mathf.Sin(Mathf.PI * progress) * _bouncingExtraScale);
            },
            1.0f,
            _bouncingFreq
            ).SetUpdate(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    private void StopBouncing() {
        if (_tweenBouncing != null && _tweenBouncing.IsActive()) {
            _tweenBouncing.Kill();
        }

        _isBouncing = false;
        _rectRoot.localScale = Vector3.one;
    }
    
    public void StartMoving(UILevelMapNode target, bool skipTween = false) {
        if (_isMoving) {
            return;
        }

        StopBouncing();
        StopMoving();

        _isMoving = true;

        if (_currentLevel != null) {
            _currentLevel.StopAnimation();
            _currentLevel.HideArrows();
            _currentLevel.StopArrowAnimation();
        }

        float progress = 0;
        float duration = skipTween ? 0 : 3.0f / _moveSpeed;

        Vector3 startPos = _rectRoot.localPosition;
        Vector3 endPos = target.LocalPosition;

        _tweenMove = DOTween.To(
            () => progress,
            v => {
                progress = v;
                _rectRoot.localPosition = Vector3.Lerp(startPos, endPos, progress);
            },
            1.0f,
            duration
            ).SetUpdate(true).SetEase(Ease.Linear);
        _tweenMove.onComplete += () => OnMoveComplete(target);
    }

    private void StopMoving() {
        if (_tweenMove != null && _tweenMove.IsActive()) {
            _tweenMove.Kill();
        }

        _isMoving = false;
    }

    private void OnMoveComplete(UILevelMapNode target) {
        _isMoving = false;

        StartBouncing();

        if (target != null) {
            target.PlayAnimation();
            target.ShowArrows();
            target.PlayArrowAnimation();
            _currentLevel = target;
        }
    }
    #endregion
}
