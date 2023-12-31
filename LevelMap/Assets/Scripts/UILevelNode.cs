using UnityEngine;

public class UILevelNode : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private UIRoute _uiRouteUp = null;
    [SerializeField] private UIRoute _uiRouteDown = null;
    [SerializeField] private UIRoute _uiRouteLeft = null;
    [SerializeField] private UIRoute _uiRouteRight = null;
    #endregion

    #region Properties
    public UIRoute UIRouteUp {
        get {
            return _uiRouteUp;
        }
    }

    public UIRoute UIRouteDown {
        get {
            return _uiRouteDown;
        }
    }

    public UIRoute UIRouteLeft {
        get {
            return _uiRouteLeft;
        }
    }

    public UIRoute UIRouteRight {
        get {
            return _uiRouteRight;
        }
    }
    #endregion
}
