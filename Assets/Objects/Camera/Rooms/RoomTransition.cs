using UnityEngine;
using static Room;

public class RoomTransition : MonoBehaviour
{
    [Header("Transition Data")]
    [SerializeField] TransitionSide transitionSide;
    [SerializeField] int newRoomID;

    public TransitionSide TransitionSide { get => transitionSide;}
    private BoxCollider2D bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    public void SetData(TransitionSide _transitionSide, int _newRoomID)
    {
        transitionSide = _transitionSide; 
        newRoomID = _newRoomID;
    }

    public void SetRoomActive(bool active)
    {
        bc.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            RoomManager.Instance.SetNewRoom(newRoomID, true);
        }
    }
}
