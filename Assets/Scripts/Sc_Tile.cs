using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sc_Tile : MonoBehaviour, IDragHandler, IEndDragHandler
{
    Sc_TileGenerator generator => FindObjectOfType<Sc_TileGenerator>();
    Camera mainCam => Camera.main;
    public SpriteRenderer spr => GetComponentInChildren<SpriteRenderer>();

    [SerializeField] TextMeshPro myText;
    Vector3 draggedPos;
    Vector3 basePos;
    bool inDrag;
    public Vector2 coordinates;
    public int scoreValue = 100;
    [SerializeField] GameObject fx;
    [SerializeField] bool debug;

    Vector3 mdr;
    Vector3 mdr2;

    public bool IsSameOf(Sc_Tile obj1, Sc_Tile obj2)
    {
        if (obj1.spr == obj2.spr)
            return true;
        else
            return false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!inDrag)
        {
            basePos = transform.position;
            inDrag = true;
        }

        draggedPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        draggedPos.z = 0;

        if (draggedPos.x > mainCam.orthographicSize / 2)
            draggedPos.x = mainCam.orthographicSize / 2;

        if (draggedPos.x < -mainCam.orthographicSize / 2)
            draggedPos.x = -mainCam.orthographicSize / 2;

        Vector3 screenEdges = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        if (draggedPos.y > screenEdges.y)
            draggedPos.y = screenEdges.y;

        if (draggedPos.y < screenEdges.y - mainCam.orthographicSize*2)
            draggedPos.y = screenEdges.y - mainCam.orthographicSize*2;

        transform.position = draggedPos;
        generator.GetClosest(gameObject);
    }

    private void OnDestroy()
    {
        GameObject newFX = Instantiate(fx, transform.position, Quaternion.Euler(90,0,0));
        newFX.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, spr.sprite);
    }

    private void OnMouseDown()
    {
        mdr = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        mdr2 = mainCam.ScreenToWorldPoint(Input.mousePosition);
        print((mdr - mdr2).normalized);

        if ((mdr - mdr2).normalized.x > 0.9f)
            print("left"); //left
        else if ((mdr - mdr2).normalized.x < -0.9f)
            print("right"); //right
        else if ((mdr - mdr2).normalized.y > 0.9f)
            print("down"); //down
        else if ((mdr - mdr2).normalized.y < -0.9f)
            print("up"); //up
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inDrag = false;
        generator.Swap(gameObject, basePos);
    }

    private void Update()
    {
        myText.gameObject.SetActive(debug);
        myText.text = "[" + coordinates.x + "," + coordinates.y + "]";
    }
}
