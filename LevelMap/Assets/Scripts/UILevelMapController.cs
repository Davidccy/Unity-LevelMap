using System.Collections;
using TMPro;
using UnityEngine;

public class UILevelMapController : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textLevelName = null;
    [SerializeField] private UILevelMapPlayer _player = null;
    [SerializeField] private UILevelMapNode _nodeInit = null;
    #endregion

    #region Internal Fields
    private UILevelMapNode _nodeCurrent = null;
    #endregion

    #region Properties
    public UILevelMapNode NodeCurrent {
        get {
            return _nodeCurrent;
        }
    }

    private bool IsPerforming {
        get {
            if (_player != null) {
                return _player.IsMoving;
            }

            return false;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        StopAllCoroutines();
        MoveToNode(_nodeInit, true);
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            TryGoToNode(MoveDirection.Up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            TryGoToNode(MoveDirection.Down);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            TryGoToNode(MoveDirection.Left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            TryGoToNode(MoveDirection.Right);
        }
    }
    #endregion

    #region Internal Methods
    private void MoveToNode(UILevelMapNode node, bool skipPerformance = false) {
        if (IsPerforming) {
            return;
        }

        StartCoroutine(Move(node, skipPerformance));
    }

    private IEnumerator Move(UILevelMapNode targetNode, bool skipPerformance) {
        if (_player != null) {
            _player.StartMoving(targetNode, skipPerformance);

            while (_player.IsMoving) {
                yield return new WaitForEndOfFrame();
            }
        }

        _nodeCurrent = targetNode;
        Refresh();
    }

    private void TryGoToNode(MoveDirection md) {
        if (IsPerforming) {
            return;
        }

        if (_nodeCurrent == null) {
            return;
        }

        UILevelMapNode nodeToGo = _nodeCurrent.GetNeighbors(md);
        if (nodeToGo != null) {
            MoveToNode(nodeToGo);
        }
    }

    private void Refresh() {
        _textLevelName.text = _nodeCurrent.LevelName;
    }
    #endregion
}
