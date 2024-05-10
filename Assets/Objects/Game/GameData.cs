public class GameData
{
    public const float roomTransitionTime = 1.4f;
    public const float roomColliderThickness = 0.1f;
    public const float roomTransitionDistance = 4f;

    public const float gridX = 2.4f;
    public const float gridY = 1.8f;

    public const float megamanSizeX = 0.2f;
    public const float megamanSizeY = 0.24f;

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
