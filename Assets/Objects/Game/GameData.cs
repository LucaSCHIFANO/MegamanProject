public class GameData
{
    public static float roomTransitionTime = 1.4f;
    public static float roomColliderThickness = 0.1f;
    public static float roomTransitionDistance = 4f;

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
