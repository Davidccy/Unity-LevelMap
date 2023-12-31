using UnityEngine;

public class UIPlayerMovingArrow : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private GameObject _target = null;
    #endregion

    #region Internal Fields
    private Vector2 _previousPos = Vector2.zero;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        _previousPos = _target.transform.position;
    }

    private void Update() {
        UpdateDirection();

        _previousPos = _target.transform.position;
    }
    #endregion

    #region Internal Methods
    private void UpdateDirection() {
        Vector2 currentPos = _target.transform.position;

        float diffX = currentPos.x - _previousPos.x;
        float diffY = currentPos.y - _previousPos.y;
        float tanX = diffX != 0 ? diffY / diffX : 0;
        float x = Mathf.Atan(tanX); // This is radius, return value between -π to π
        if (diffX < 0) {
            x = x + Mathf.PI;
        }
        float xDegree = x * Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(0, 0, xDegree);
    }
    #endregion
}
