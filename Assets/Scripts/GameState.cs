using System;
public class GameState
{
    protected GameState()
    {


    }
    private static GameState _instance = null;

    // health
    public const int PlayerMaxHealth = 5;
    public int PlayerHealth = PlayerMaxHealth;

    public static GameState Instance
    {
        get
        {
            if (GameState._instance == null)
            {
                _instance = new GameState();
            }
            return GameState._instance;
        }
    }
}
