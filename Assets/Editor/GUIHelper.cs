using UnityEngine;

public static class GUIHelper
{

    private static GUIStyle s_TempStyle = new GUIStyle();

    public static void DrawTexture(Rect position, Texture2D texture)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        s_TempStyle.normal.background = texture;

        s_TempStyle.Draw(position, GUIContent.none, false, false, false, false);
    }

}
