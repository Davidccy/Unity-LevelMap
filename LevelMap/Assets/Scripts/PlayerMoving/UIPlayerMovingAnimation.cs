using UnityEngine;

public class UIPlayerMovingAnimation : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private MoveDirection _defaultDir = MoveDirection.None;
    [SerializeField] private Animator _animator = null;
    #endregion

    #region Internal Fields
    private MoveDirection _curDir = MoveDirection.None;
    private Vector2 _previousPos = Vector2.zero;
    #endregion

    #region Mono Behaviours
    private void OnEnable() {
        _previousPos = transform.position;
        UpdateDirection(_defaultDir);
    }

    private void Update() {
        UpdatePosition();
    }
    #endregion

    #region Internal Methods
    private void UpdateDirection(MoveDirection newDir) {
        if (newDir == MoveDirection.None) {
            return;
        }

        if (_curDir == newDir) {
            return;
        }

        // Direction changed
        _curDir = newDir;
        PlayDirection(_curDir);
    }

    private void UpdatePosition() {
        Vector2 currentPos = transform.position;
        Vector2 diff = currentPos - _previousPos;
        _previousPos = currentPos;

        MoveDirection dir = GetDirection(diff);
        UpdateDirection(dir);
    }

    private void PlayDirection(MoveDirection dir) {
        if (dir == MoveDirection.Up) {
            _animator.Play("WalkUp");
        }
        else if (dir == MoveDirection.Down) {
            _animator.Play("WalkDown");
        }
        else if (dir == MoveDirection.Left) {
            _animator.Play("WalkLeft");
        }
        else if (dir == MoveDirection.Right) {
            _animator.Play("WalkRight");
        }
        else { 
            // Do nothing
        }
    }

    private MoveDirection GetDirection(Vector2 vector) {
        if (vector == null) {
            return MoveDirection.None;
        }

        if (vector == Vector2.zero) {
            return MoveDirection.None;
        }

        if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y)) {
            if (vector.x >= 0) {
                return MoveDirection.Right;
            }

            return MoveDirection.Left;
        }
        else {
            if (vector.y >= 0) {
                return MoveDirection.Up;
            }

            return MoveDirection.Down;
        }
    }
    #endregion
}
