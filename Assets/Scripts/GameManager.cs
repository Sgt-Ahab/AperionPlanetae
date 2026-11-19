//Name: David Sargent
//Date: 11/16/26
//Desc: Mainframe for Game Logic, switches backgrounds, calls enemies and enclaves
//whilst maintainging timeframe between. Collectables are 
//Attach: GameStateManager
//==============================================
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Background Data")]
    //Add Background images into this, and use it in order, then return to the end!
    [SerializeField]
    private Sprite[] BackgroundSprites;
    [SerializeField]
    private GameObject BackgroundSprite;
    [Header("Spawn Logic")]
    [SerializeField]
    private float EnemyWaveDelta;
    [SerializeField]
    private float EnclaveSpawnDelta;
    private float PlayerScore;
    private EnemyStateManager EnemySuite;
    private int DistanceScoreInt = 10;
    private float TimeSurvived;
    private float TimerLoop;
    private int TimeSurvivedINT;
    private float EnemyFrameTimer;
    private float EnclaveFrameTimer;
    private string Path;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemySuite = GetComponentInChildren<EnemyStateManager>();
        Path = Application.persistentDataPath + "/Highscores.txt";
    }

    // Update is called once per frame
    void Update()
    {
        AddScore(DistanceScoreInt * Time.deltaTime);
        TimerLoop += Time.deltaTime;
        TimeSurvivedINT = Mathf.FloorToInt(TimerLoop);
        BackgroundCondition();
        //If-checks for enemy summons
        if(EnemyFrameTimer >= EnemyWaveDelta)
        {
            //Wave Spawn logic
            float RandomEnemyCount = Random.Range(6, 12);
            float RandomSpawnTime = Random.Range(1, 6);
            int RandomEnemySpawnpoint = (int)Random.Range(0, 6);
            int RandomShipID = (int)Random.Range(0, 3);
            EnemySuite.AssembleAirEnemies(RandomEnemyCount, RandomSpawnTime, RandomEnemySpawnpoint, RandomShipID);
            EnemyFrameTimer = 0;
        }
        if (EnclaveFrameTimer >= EnclaveSpawnDelta)
        {
            //Enclave Spawn logic
            float RandomEnemyCount = Random.Range(1, 4);
            //1 - 3 Enclaves
            int[] SpawnIndexes = new int[(int)RandomEnemyCount];
            List<int> AvailableSpawns = new List<int> {0, 1, 2, 3, 4, 5};
            float RandomSpawnTime = Random.Range(3, 6);
            int RandomEnclaveID = (int)Random.Range(0, 3);
            for (int i = 0; i < (int)RandomEnemyCount; i++)
            {
                int RandomEnemySpawnpoint = Random.Range(0, AvailableSpawns.Count);
                SpawnIndexes[i] = AvailableSpawns[RandomEnemySpawnpoint];                
                AvailableSpawns.RemoveAt(RandomEnemySpawnpoint);
                
            }
            EnemySuite.AssembleEnclaves(RandomEnemyCount, RandomSpawnTime, SpawnIndexes, RandomEnclaveID);

            EnclaveFrameTimer = 0;
        }
        EnemyFrameTimer += Time.deltaTime;
        EnclaveFrameTimer += Time.deltaTime;
    }
    //I/O File Write and Check, delim is `,`
    private List<int> LoadScores()
    {
        string TextData = null;
        List<int> ScoreList = new List<int>();
        if(File.Exists(Path))
        {
            TextData = File.ReadAllText(Path);
        }
        else
        {
            return new List<int>();
        }
        if(TextData != null)
        {
            string[] ScoreStrings = TextData.Split(',');
            for(int i = 0; i < ScoreStrings.Length; i++)
            {
                ScoreList.Add(int.Parse(ScoreStrings[i]));
            }
        }
        return ScoreList;
    }
    private void SaveScores()
    {
        int ScoreInt = (int)PlayerScore;
        string DataString = "";
        List<int> PlayerScores = LoadScores();
        PlayerScores.Add(ScoreInt);
        PlayerScores.Sort();
        PlayerScores.Reverse();
        if(PlayerScores.Count > 5)
        {
            PlayerScores.RemoveAt(5);
        }
        DataString = string.Join(",", PlayerScores);
        File.WriteAllText(Path, DataString);
       
    }
    //Background Rendition, using the Sprite Backgrounds
    private void BackgroundCondition()
    {
        SpriteRenderer BG = BackgroundSprite.GetComponent<SpriteRenderer>();
        // 0 - 6 within the DB spites
        if(0 <= TimeSurvivedINT && TimeSurvivedINT <= 90)
        {
            BG.sprite = BackgroundSprites[0];
        }
        else if(90 <= TimeSurvivedINT && TimeSurvivedINT <= 180)
        {
            BG.sprite = BackgroundSprites[1];
        }
        else if(180 <= TimeSurvivedINT && TimeSurvivedINT <= 270)
        {
            BG.sprite = BackgroundSprites[2];
        }
        else if(270 <= TimeSurvivedINT && TimeSurvivedINT <= 360)
        {
            BG.sprite = BackgroundSprites[3];
        }
        else if(360 <= TimeSurvivedINT && TimeSurvivedINT <= 450)
        {
            BG.sprite = BackgroundSprites[4];
        }
        else if (450 <= TimeSurvivedINT && TimeSurvivedINT <= 540)
        {
            BG.sprite = BackgroundSprites[5];
        }
        else if (540 <= TimeSurvivedINT && TimeSurvivedINT <= 630)
        {
            BG.sprite = BackgroundSprites[6];
        }
        else
        {
            TimeSurvivedINT = 0;
            TimerLoop = 0;
        }
    }
    public void GameOverState()
    {
        //Game Over state, time stops, and save score
        SaveScores();
        Time.timeScale = 0f;
        //Add the canvas and make it visible
    }
    public void AddScore(float IncrementScore)
    {
        PlayerScore += IncrementScore;
    }
    public void ResetGameState()
    {
        //Reset the Game state
        PlayerScore = 0;

    }
}
