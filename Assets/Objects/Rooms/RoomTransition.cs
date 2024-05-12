using System.Collections;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using static Room;

public class RoomTransition : MonoBehaviour
{
    [Header("Transition Data")]
    [SerializeField] TransitionSide transitionSide;
    int newRoomID;
    bool onlyOnLadder;

    [SerializeField] BossDoor bossDoorPrefab;
    bool isBossTransition;
    BossDoor currentBossDoor;

    private MegamanController megaman;

    public TransitionSide TransitionSide { get => transitionSide;}
    public bool OnlyOnLadder { get => onlyOnLadder;}
    public int NewRoomID { get => newRoomID; set => newRoomID = value; }
    public bool IsBossTransition { get => isBossTransition; }
    public BossDoor CurrentBossDoor { get => currentBossDoor; }

    private BoxCollider2D bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        newRoomID = -1;
    }

    public void SetData(TransitionSide _transitionSide, bool _onlyOnLadder, bool _isBossTransition)
    {
        transitionSide = _transitionSide; 
        onlyOnLadder = _onlyOnLadder;
        isBossTransition = _isBossTransition;
        
        if (isBossTransition)
            currentBossDoor = Instantiate(bossDoorPrefab, transform);
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
            TransitionNewRoom();
            megaman = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            var megamanController = collision.GetComponent<MegamanController>();
            if (!onlyOnLadder || megamanController.IsClimbing) TransitionNewRoom();
            else megaman = megamanController;
        }
    }

    private void TransitionNewRoom()
    {
        if (isBossTransition)
            StartCoroutine(BossTransition());
        else RoomManager.Instance.SetNewRoom(newRoomID, this);
    }
    
    private IEnumerator BossTransition()
    {
        currentBossDoor.Open();
        LevelManager.Instance.Megaman.ChangeState(MegamanController.MegamanState.RoomTransition);
        yield return new WaitForSeconds(currentBossDoor.DoorAnimationLenght);
        RoomManager.Instance.SetNewRoom(newRoomID, this);
        yield return new WaitForSeconds(GameData.roomTransitionTime);
        currentBossDoor.Close();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            megaman = null;
        }
    }
}
