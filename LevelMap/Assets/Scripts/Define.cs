using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection {
    None,
    Up,
    Down,
    Left,
    Right,
}

public enum RouteType { 
    Linear,
    Bezier,
}

public enum PlayerMoveType { 
    FixedSpeed,
    FixedTime,
}
