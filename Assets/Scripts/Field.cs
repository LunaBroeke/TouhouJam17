using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] protected float padding;
    [SerializeField] protected float cellW;
    [SerializeField] protected float cellH;
    [Header("Read Only")]
    [SerializeField] protected Camera cam;
    [SerializeField] protected float width;
    [SerializeField] protected float height;
    [SerializeField] protected float startX;
    [SerializeField] protected float startY;
    public virtual void Place()
    {
        cam = Camera.main;
        width = (2f * cam.orthographicSize * cam.aspect) - (2f * padding);
        height = (2f * cam.orthographicSize) - (2f * padding);
        startX = cam.transform.position.x - (width * 0.5f) + (cellW * 0.5f);
        startY = cam.transform.position.y - (height * 0.5f) + (cellH * 0.5f);
    }
    public virtual void PlaceAtPosition(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }
}
