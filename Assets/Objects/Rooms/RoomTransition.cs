using UnityEngine;
using static Room;

public class RoomTransition : MonoBehaviour
{
    [Header("Transition Data")]
    [SerializeField] TransitionSide transitionSide;
    [SerializeField] int newRoomID;
    [SerializeField] bool onlyOnLadder;

    private MegamanController megaman;

    public TransitionSide TransitionSide { get => transitionSide;}
    public bool OnlyOnLadder { get => onlyOnLadder;}
    public int NewRoomID { get => newRoomID; set => newRoomID = value; }

    private BoxCollider2D bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        newRoomID = -1;
    }

    public void SetData(TransitionSide _transitionSide, bool _onlyOnLadder)
    {
        transitionSide = _transitionSide; 
        onlyOnLadder = _onlyOnLadder;
    }

    public void SetRoomActive(bool active)
    {
        bc.enabled = active;
    }

    private void Update()
    {
        if (megaman == null) return;

        if (megaman.IsClimbing)
        {
            RoomManager.Instance.SetNewRoom(newRoomID, true);
            megaman = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            var megamanController = collision.GetComponent<MegamanController>();
            if (!onlyOnLadder || megamanController.IsClimbing) RoomManager.Instance.SetNewRoom(newRoomID, true);
            else megaman = megamanController;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            megaman = null;
        }
    }
}
