using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class MapBorder : MonoBehaviour {
    public static MapBorder singleton;

    public int mapHalfWidth;
    public int mapHalfHeight;
    
    private void Awake() {
        singleton = this;

        Initialize();
    }

    private void Initialize() {
        GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-mapHalfWidth, mapHalfHeight), new Vector2(-mapHalfWidth, mapHalfHeight*10),
            new Vector2(mapHalfWidth*10, mapHalfHeight*10), new Vector2(mapHalfWidth*10, -mapHalfHeight*10), new Vector2(-mapHalfWidth*10, -mapHalfHeight*10),
            new Vector2(-mapHalfWidth*10, mapHalfHeight*10), new Vector2(-mapHalfWidth, mapHalfHeight*10), new Vector2(-mapHalfWidth, -mapHalfHeight),
            new Vector2(mapHalfWidth, -mapHalfHeight), new Vector2(mapHalfWidth, mapHalfHeight)};
    }
}
