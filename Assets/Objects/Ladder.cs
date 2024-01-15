using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private BoxCollider2D bc;

    [SerializeField] private Transform topHandler;
    [SerializeField] private Transform botHandler;
    [SerializeField] private EdgeCollider2D ladderTile;
    [SerializeField] private Vector2 boxColliderMultiplicator = Vector2.one;

    public Transform TopHandler { get => topHandler; }
    public Transform BotHandler { get => botHandler;}

    private void Awake()
    {
        float width = GetComponent<SpriteRenderer>().size.x * boxColliderMultiplicator.x;
        float height = GetComponent<SpriteRenderer>().size.y * boxColliderMultiplicator.y;
        topHandler.position = new Vector3(transform.position.x, transform.position.y + (height / 2), 0);
        botHandler.position = new Vector3(transform.position.x, transform.position.y - (height / 2), 0);
        ladderTile.offset = new Vector2(0, height / 2);

        bc = GetComponent<BoxCollider2D>();
        bc.offset = Vector2.zero;
        bc.size = new Vector2(width, height);
    }

    public IEnumerator PlayerOnLadder()
    {
        ladderTile.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        ladderTile.gameObject.SetActive(true);
    }
}
