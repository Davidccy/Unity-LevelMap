using UnityEngine;

public static class Utility {
    public static Vector2 ConvertRectTransformToCanvasPosition(RectTransform rect) {
        if (rect == null) {
            return Vector2.zero;
        }

        // Check target is canvas or not
        Canvas c = GetCanvasOfRectTransform(rect);
        RectTransform rectCanvas = (RectTransform) c.transform;
        if (rect == rectCanvas) {
            return Vector2.zero;
        }

        // If target is not canvas, then compute offset from anchor and pivot
        RectTransform rectParent = (RectTransform) rect.parent;

        Vector2 refAnchor = new Vector2(0.5f, 0.5f);
        Vector2 diffAnchor = (rect.anchorMax + rect.anchorMin) / 2 - refAnchor;
        Vector2 diffAnchorPos = new Vector2(
            diffAnchor.x * rectParent.rect.width,
            diffAnchor.y * rectParent.rect.height
            );

        Vector2 refPivot = new Vector2(0.5f, 0.5f);
        Vector2 diffPivot = refPivot - rect.pivot;
        Vector2 diffPivotPixel = new Vector2(
            diffPivot.x * rect.rect.width,
            diffPivot.y * rect.rect.height
            );

        Vector2 centerPos = rect.anchoredPosition + diffAnchorPos + diffPivotPixel;
        if (rect.parent != null && (RectTransform) rect.parent != rectCanvas) {
            return centerPos + ConvertRectTransformToCanvasPosition((RectTransform) rect.parent);
        }
        
        return centerPos;
    }

    public static Canvas GetCanvasOfRectTransform(RectTransform rect) {
        // NOTE:
        // Get the Canvas on most outer layer

        Canvas c = rect.GetComponent<Canvas>();
        if (c != null) {
            return c;
        }

        if (c == null && rect.parent != null) {
            return GetCanvasOfRectTransform((RectTransform) rect.parent);
        }

        return null;
    }
}
