using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UINormalLineDrawer : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btn = null;

    [SerializeField] private RectTransform[] _rtSpots = null;
    [SerializeField] private GameObject _goLineRoot = null;
    [SerializeField] private GameObject _goLineRes = null;
    [SerializeField] private float _drawSpeed = 0;

    [Range(0.5f, 40)]
    [SerializeField] private float _routeThickness = 0;
    #endregion

    #region Internal Fields
    private bool _isDrawing = false;
    private List<GameObject> _goLines = new List<GameObject>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void OnDestroy() {
        _btn.onClick.RemoveListener(ButtonOnClick);
    }
    #endregion

    #region Button Handling
    private void ButtonOnClick() {
        DrawLine();
    }
    #endregion

    #region Internal Methods
    private async void DrawLine() {
        if (_isDrawing) {
            Debug.LogErrorFormat("Now is drawing");
            return;
        }

        if (_rtSpots == null || _rtSpots.Length <= 1) {
            Debug.LogErrorFormat("Unexpected count of spots");
            return;
        }

        // Clear lines
        for (int i = 0; i < _goLines.Count; i++) {
            Destroy(_goLines[i]);
        }
        _goLines.Clear();

        // Generate line
        _isDrawing = true;
        for (int i = 0; i < _rtSpots.Length - 1; i++) {
            Vector2 posStart = _rtSpots[i].position;
            Vector2 posEnd = _rtSpots[i + 1].position;

            await GenerateLine(posStart, posEnd);
        }
        _isDrawing = false;
    }

    private async Task GenerateLine(Vector2 posStart, Vector2 posEnd) {
        GameObject newLine = Instantiate(_goLineRes, _goLineRoot.transform);
        _goLines.Add(newLine);

        RectTransform newLineRT = newLine.transform as RectTransform;
        newLineRT.position = posStart;

        Vector2 diffVector = posEnd - posStart;
        newLineRT.localEulerAngles = new Vector3(0, 0, GetVectorDegree(diffVector.normalized));
        float lineLength = diffVector.magnitude;

        float animationLength = 0;
        while (animationLength < lineLength) {
            newLineRT.sizeDelta = new Vector2(animationLength, _routeThickness);
            animationLength += _drawSpeed;
            await Task.Delay(10);
        }

        newLineRT.sizeDelta = new Vector2(lineLength, _routeThickness);
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
    #endregion
}
