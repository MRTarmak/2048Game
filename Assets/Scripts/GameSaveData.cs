using System;

[Serializable]
public class GameSaveData
{
    public int[,] CellValues;
    public int currentScore;
    public int highScore;
}