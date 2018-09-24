using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static bool paused = false;
    public static bool gameOver = false;
    public static PlayerVariable playerVariable = null;
    public static PrefabPool bulletTrailPool = null;

    public const string TAG_PLAYER = "Player";

    public const string INPUT_AXIS_X = "Horizontal";
    public const string INPUT_AXIS_Y = "Vertical";
    public const string INPUT_CROUCH = "Crouch";
    public const string INPUT_INTERACT = "Interact";
    public const string INPUT_PAUSE = "Pause";

    public const string INTERACT_DOOR_OPEN = "Open %name%";
    public const string INTERACT_DOOR_CLOSE = "Close %name%";

    public const string KEYWORD_NAME = "%name%";

    public const float TIME_MIN_DETECTION = 0.1f;
}
