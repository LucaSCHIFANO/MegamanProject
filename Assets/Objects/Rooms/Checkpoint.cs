using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            LevelManager.Instance.SetNewSpawnPoint(respawnPoint);
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void SetRespawnPoint(Vector3 newSpawn)
    {
        respawnPoint.position = newSpawn;
    }

}
