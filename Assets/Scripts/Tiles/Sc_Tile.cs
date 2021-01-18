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

    [Header("Bonus")]
    public SpellType currentEffect;
    [SerializeField] SpriteRenderer spellSpriteRenderer;
    [SerializeField] Sprite[] spellSprites;
    public int bonusValue = 0;
    [SerializeField] TextMeshPro bonusText;

    [Header("")]
    [SerializeField] GameObject fx;
    public bool highlight;
    [SerializeField] Material glowSprite;
    [SerializeField] Sprite[] allSprites;
    Material baseMat;

    Vector3 mousePos;
    Vector3 mouseNextPos;
    bool inDrag;

    private void Awake()
    {
        tileManager = FindObjectOfType<Sc_TileManager>();
        mainCam = Camera.main;
        spriteRender = GetComponentInChildren<SpriteRenderer>();
        baseMat = spriteRender.material;
        SetBonus(0);
        SetEffect(SpellType.None);
    }

    public void SetEffect(SpellType type)
    {
        currentEffect = type;
        if (currentEffect == SpellType.None)
        {
            spellSpriteRenderer.gameObject.SetActive(false);
            return;
        }
        else
        {
            spellSpriteRenderer.gameObject.SetActive(true);
            spellSpriteRenderer.sprite = spellSprites[(int)type - 1];
        }
    }

    public void SetBonus(int value)
    {
        bonusValue = value;
        bonusText.text = "+" + bonusValue;
        bonusText.gameObject.SetActive(bonusValue > 0);
        switch (myType)
        {
            case TileType.Mana:
                bonusText.color = Color.blue;
                break;
            case TileType.Defense:
                bonusText.color = Color.green;
                break;
            case TileType.Life:
                bonusText.color = Color.red;
                break;
            case TileType.Attack:
                bonusText.color = Color.yellow;
                break;
        }
    }

    public void Creation(int index)
    {
        Vector3 baseScale = transform.localScale;
        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(baseScale, tileManager.tileBirthDuration);

        System.Array array = System.Enum.GetValues(typeof(TileType));
        spriteRender.sprite = allSprites[index];
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

    public void Death()
    {
        if (transform == null)
            return;

        Sequence sequence = DOTween.Sequence();
        float delay = tileManager.tileDeathDuration;
        sequence.Append(transform.DOScale(transform.localScale * 1.2f, delay));
        sequence.Append(transform.DOScale(0.01f, delay / 2).SetDelay(delay / 2));
        sequence.Play();
        GameObject newFX = Instantiate(fx, transform.position, Quaternion.Euler(90, 0, 0));
        newFX.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, spriteRender.sprite);
        Destroy(gameObject, sequence.Duration());
    }

    private void OnMouseDown()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        spriteRender.material = highlight ? glowSprite : baseMat;
    }

    public override string ToString()
    {
        return myType + " " + coordinates;
    }
}
