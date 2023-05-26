using UnityEngine;

public static class AnimatorHashedInts
{
    //Animator hashed strings
    public static int Horizontal = Animator.StringToHash("Horizontal");
    public static int Vertical = Animator.StringToHash("Vertical");
    public static int IsRunning = Animator.StringToHash("IsRunning");
    public static int IsSoldier = Animator.StringToHash("IsSoldier");
    public static int IsDead = Animator.StringToHash("IsDead");
}

public static class UIPrefabStrings
{
    //Room List Prefab
    public static string RoomNameText = "RoomNameText";
    public static string RoomPlayersText = "RoomPlayersText";
    public static string JoinRoomButton = "JoinRoomButton";
    //Player List Prefab
    public static string PlayerName = "PlayerNameText";
    public static string PlayerIndicator = "PlayerIndicator";
}

public static class GameManagerStrings
{
    //prefab string reference of enemy player model
    public static string PlayerPrefab = "PlayerPrefab";
    public static string Player1Prefab = "Player1Prefab";
    public static string Player2Prefab = "Player2Prefab";
    public static string BallPrefab = "BallPrefab";
}

public static class PlayerSetupStrings
{
    public static string FixedJoystick = "Fixed Joystick";
    public static string RotationTouchField = "RotationTouchField";
    public static string FireButton = "FireButton";
    public static string RespawnText = "RespawnText";
}

public static class PrefKeys
{
    public static string PlayerName = "PlayerName";
}


public static class ShootingVariables
{
    public static Vector3 ViewportRayPoint = new Vector3(0.5f, 0.5f);
    public static float StartHealth = 100;
}

public enum Players
{
    RedPlayer,
    BluePlayer
}
