public class GameData
{
    public const float bossTransitionHeightEditor = 0.35f;
    public const float bossTransitionHeight = 0.225f;

    public const float roomTransitionTime = 1.4f;
    public const float roomColliderThickness = 0.1f;
    public const float bossRoomColliderThickness = 0.2f;
    public const float roomTransitionDistance = 4f;
    public const float bossRoomTransitionDistance = 2.5f;

    public const float gridX = 2.4f;
    public const float gridY = 1.8f;

    public const float megamanSizeX = 0.2f;
    public const float megamanSizeY = 0.24f;

    public const float megamanRespawnTime = 3f;

    public enum Side
    {
        Player,
        Enemy
    }

    public enum WeaponType
    {
        None,
        MegaBuster
    }
}
