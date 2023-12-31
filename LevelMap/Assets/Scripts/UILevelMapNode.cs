using DG.Tweening;
using UnityEngine;

public class UILevelMapNode : MonoBehaviour {
    #region Serialized Fields
    [Header("Basic Info")]
    //[SerializeField] private int _index = 0;
    [SerializeField] private string _levelName = string.Empty;
    [SerializeField] private RectTransform _rectRoot = null;

    [Header("Neighbors")]
    [SerializeField] private UILevelMapNode _nodeUp = null;
    [SerializeField] private UILevelMapNode _nodeDown = null;
    [SerializeField] private UILevelMapNode _nodeLeft = null;
    [SerializeField] private UILevelMapNode _nodeRight = null;

    [Header("Arrows")]
    [SerializeField] private float _arrowFrq = 0;
    [SerializeField] private float _arrowFullFrq = 0;
    [SerializeField] private float _arrowMaxDisplacement = 0;
    [SerializeField] private RectTransform _rtArrowUp = null;
    [SerializeField] private RectTransform _rtArrowDown = null;
    [SerializeField] private RectTransform _rtArrowLeft = null;
    [SerializeField] private RectTransform _rtArrowRight = null;
    #endregion

    #region Internal Fields
    private Vector3 _oriArrowPosUp;
    private Vector3 _oriArrowPosDown;
    private Vector3 _oriArrowPosLeft;
    private Vector3 _oriArrowPosRight;

    private Tween _tweenArrow = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _oriArrowPosUp = _rtArrowUp.localPosition;
        _oriArrowPosDown = _rtArrowDown.localPosition;
        _oriArrowPosLeft = _rtArrowLeft.localPosition;
        _oriArrowPosRight = _rtArrowRight.localPosition;
    }

    private void OnEnable() {
        HideArrows();
    }
    #endregion

    #region Properties
    public string LevelName {
        get {
            return _levelName;
        }
    }

    public Vector3 LocalPosition {
        get {
            return _rectRoot.localPosition;
        }
    }
    #endregion

    #region APIs
    public UILevelMapNode GetNeighbors(MoveDirection md) {
        UILevelMapNode nodeNeighbor = null;
        if (md == MoveDirection.Up) {
            nodeNeighbor = _nodeUp;
        }
        else if (md == MoveDirection.Down) {
            nodeNeighbor = _nodeDown;
        }
        else if (md == MoveDirection.Left) {
            nodeNeighbor = _nodeLeft;
        }
        else if (md == MoveDirection.Right) {
            nodeNeighbor = _nodeRight;
        }

        return nodeNeighbor;
    }

    public void PlayAnimation() {
        this.transform.localScale = Vector3.one * 1.5f;
    }

    public void StopAnimation() {
        this.transform.localScale = Vector3.one * 1.0f;
    }

    public void ShowArrows() {
        _rtArrowUp.gameObject.SetActive(_nodeUp != null);
        _rtArrowDown.gameObject.SetActive(_nodeDown != null);
        _rtArrowLeft.gameObject.SetActive(_nodeLeft != null);
        _rtArrowRight.gameObject.SetActive(_nodeRight != null);
    }

    public void HideArrows() {
        _rtArrowUp.gameObject.SetActive(false);
        _rtArrowDown.gameObject.SetActive(false);
        _rtArrowLeft.gameObject.SetActive(false);
        _rtArrowRight.gameObject.SetActive(false);
    }

    public void PlayArrowAnimation() {
        StopArrowAnimation();

        float progress = 0;
        _tweenArrow = DOTween.To(
            () => progress,
            v => {
                progress = v;

                float degree = Mathf.PI * progress;
                if (_arrowFrq > 0) {
                    degree = degree * (1 / _arrowFrq);
                }

                degree = Mathf.Clamp(degree, 0, Mathf.PI);
                float displacement = Mathf.Sin(degree) * _arrowMaxDisplacement;

                _rtArrowUp.localPosition = _oriArrowPosUp + Vector3.up * displacement;
                _rtArrowDown.localPosition = _oriArrowPosDown + Vector3.down * displacement;
                _rtArrowLeft.localPosition = _oriArrowPosLeft + Vector3.left * displacement;
                _rtArrowRight.localPosition = _oriArrowPosRight + Vector3.right * displacement;
            },
            _arrowFullFrq,
            _arrowFullFrq)
            .SetUpdate(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    public void StopArrowAnimation() {
        if (_tweenArrow != null && _tweenArrow.IsActive()) {
            _tweenArrow.Kill();
        }

        _rtArrowUp.localPosition = _oriArrowPosUp;
        _rtArrowDown.localPosition = _oriArrowPosDown;
        _rtArrowLeft.localPosition = _oriArrowPosLeft;
        _rtArrowRight.localPosition = _oriArrowPosRight;
    }
    #endregion

    #region Gizmos Handlings
    private void OnDrawGizmosSelected() {        
        Gizmos.DrawIcon(transform.position + Vector3.up * 60, "MinecraftGuardian.png", false);
    }
    #endregion
}
