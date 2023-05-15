public class GameAttempt
{
    public bool isWin;
    public bool isDraw;
    public float gameDuration;
    public int totalCardsPlayed;
    public int cardsPlayerPlayed;
    public int cardsEnemyPlayed;
    public int enemyBlood;
    public int playerBlood;
    public string difficulty;

    public GameAttempt(bool IsWin, bool IsDraw, float GameDuration, int TotalCardsPlayed, int CardsPlayerPlayed, int CardsEnemyPlayed, int EnemyBlood, int PlayerBlood, string Difficulty)
    {
        isWin = IsWin;
        isDraw = IsDraw;
        gameDuration = GameDuration;
        totalCardsPlayed = TotalCardsPlayed;
        cardsPlayerPlayed = CardsPlayerPlayed;
        cardsEnemyPlayed = CardsEnemyPlayed;
        enemyBlood = EnemyBlood;
        playerBlood = PlayerBlood;
        difficulty = Difficulty;
    }
}
