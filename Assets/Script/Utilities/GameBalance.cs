using UnityEngine;

public static class GameBalance
{
    private static GameConfig _config;
    public static GameConfig Config
        => _config ??= Resources.Load<GameConfig>("GameConfig"); // place l’asset dans Resources/
}
