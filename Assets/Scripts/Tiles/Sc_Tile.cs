using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum TileType
{
    Mana,
    Defense,
    Life,
    Attack,
}

public class Sc_Tile : MonoBehaviour, IDragHandler, IEndDragHandler
{
    Sc_TileManager tileManager;
    Camera mainCam;
    SpriteRenderer spriteRender;

    public TileType myType;
    public Vector2Int coordinates;
    [SerializeField] TextMeshPro myText;
    public int scoreValue = 100;
    [SerializeField] GameObject fx;
    [SerializeField] bool debug;
    [SerializeField] Material glowSprite;
    Material baseMat;

    Vector3 mousePos;
    Vector3 mouseNextPos;
    public bool highlight;
    bool inDrag;

    private void Awake()
    {
        tileManager = FindObjectOfType<Sc_TileManager>();
        mainCam = Camera.main;
        spriteRender = GetComponentInChildren<SpriteRenderer>();
        baseMat = spriteRender.material;
    }

    public void Creation(Sprite spr, int index)
    {
        Vector3 baseScale = transform.localScale;
        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(baseScale, tileManager.tileBirthDuration);

        System.Array array = System.Enum.GetValues(typeof(TileType));
        spriteRender.sprite = spr;
        myType = (TileType)array.GetValue(index);
    }

    public bool IsSameOf(Sc_Tile obj2)
    {
        return myType == obj2.myType;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!inDrag)
            inDrag = true;

        if (!tileManager.gameManager.canPlay)
            return;

        mouseNextPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int direction = Vector2Int.zero;
        if ((mousePos - mouseNextPos).normalized.x > 0.9f)
            direction = new Vector2Int(-1, 0);
        else if ((mousePos - mouseNextPos).normalized.x < -0.9f)
            direction = new Vector2Int(1, 0);
        else if ((mousePos - mouseNextPos).normalized.y > 0.9f)
            direction = new Vector2Int(0, 1);
        else if ((mousePos - mouseNextPos).normalized.y < -0.9f)
            direction = new Vector2Int(0, -1);

        tileManager.highlightedTile = tileManager.GetAdjacentCell(direction, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (tileManager.highlightedTile != null)
            tileManager.Swap(this, tileManager.highlightedTile.GetComponent<Sc_Tile>());

        tileManager.highlightedTile = null;
    }

    public void Death(float offsetDelay)
    {
        float delay = offsetDelay + tileManager.tileDeathDuration;
        transform.DOScale(transform.localScale * 1.4f, delay);
        transform.DOScale(0.01f, delay / 2).SetDelay(delay / 2);
        Destroy(gameObject, delay);
        GameObject newFX = Instantiate(fx, transform.position, Quaternion.Euler(90, 0, 0));
        newFX.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, spriteRender.sprite);
    }

    private void OnMouseDown()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        myText.gameObject.SetActive(debug);
        myText.text = "[" + coordinates.x + "," + coordinates.y + "]";

        spriteRender.material = highlight ? glowSprite : baseMat;
    }

    public override string ToString()
    {
        return myType + " " + coordinates;
    }
}
