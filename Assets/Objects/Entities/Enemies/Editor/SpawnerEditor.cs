using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        var spawner = target as Spawner;

        if (spawner.GetComponent<SpriteRenderer>() == null || spawner.GetComponent<BoxCollider2D>() == null) return;

        if (spawner.Enemy != null)
        {
            spawner.transform.localScale = spawner.Enemy.transform.localScale;
            spawner.GetComponent<SpriteRenderer>().sprite = spawner.Enemy.GetComponent<SpriteRenderer>().sprite;
            spawner.GetComponent<BoxCollider2D>().offset = spawner.Enemy.GetComponent<BoxCollider2D>().offset;
            spawner.GetComponent<BoxCollider2D>().size = spawner.Enemy.GetComponent<BoxCollider2D>().size;

        }else
        {
            spawner.GetComponent<SpriteRenderer>().sprite = null;
            spawner.GetComponent<BoxCollider2D>().size =Vector2.zero;
        }

    }

}
