//Name: David Sargent
//Date: 11/16/26
//Desc: Mainframe for Enemy Logic, and spawning in
//using the six SpawnPoints attached on a array; Using Randint(6 - 11)
//Attach: EnemyStateManager
//==============================================
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    [Header("Spawnpoints & Prefabs")]
    [SerializeField]
    private Transform[] Spawnpoints;
    //Uses Prefabs
    [SerializeField]
    private GameObject[] EnemyPrefabs;
    //Enclaves are the ground units for the Player to avoid
    [SerializeField]
    private GameObject[] EnclavePrefabs;
    [Header("Spawn Parameters")]
    //Random between 6 - 11, if 'OverdriveSpawn'
    [SerializeField]
    private float SpawnNumber;
    //Seconds between Spawning [1 - 3, {if >= 3; make 0.5} >:)]
    //Make a SpawnMode[OverdriveSpawn], to offset their spawnpoint
    //If this, then offset X axis by +/- 1 AND
    //Override SpawnNumber to a good threshold
    private float SpawnSeconds;
    private bool IsOverdrive = false;
    
    //Make these public for GameStateManager to infer
    public void AssembleAirEnemies(float SpawnCount, float SpawnTime, int SpawnIndex, int ShipID)
    {
        //SpawnCount is SpawnNumber, SpawnTime is SpawnSeconds
        //SpawnIndex refers to our SpawnPoint Array to dispense our enemies
        //ShipID grabs the Sprite for spawning in our prefab objects
        //This makes it a utility function for aiding main gameflow, since
        SpawnNumber = SpawnCount;
        SpawnSeconds = SpawnTime;
        if(SpawnSeconds >= 3)
        {
            IsOverdrive = true;
        }
        else
        {
            IsOverdrive = false;
        }
        Vector3 EnemySpawn = Spawnpoints[SpawnIndex].transform.position;
        GameObject EnemySprite = EnemyPrefabs[ShipID];
        //Coroutine the Spawning using the yield return WaitForSeconds
        StartCoroutine(Spawner());
        IEnumerator Spawner()
        {
            if(IsOverdrive)
            {
                SpawnTime= 0.5f;
                SpawnCount = 11;
                for (int i = 0; i < SpawnCount; i++)
                {
                    int RandomOffsetInt = Random.Range(0, 3);
                    //Switch Case for the Offset Spawn (0.5 sec between gives buffer)
                    switch (RandomOffsetInt)
                    {
                        case 0:
                            Vector3 LeftSpawn = EnemySpawn + new Vector3(-1f, 0, 0);
                            Instantiate(EnemySprite, LeftSpawn, Quaternion.identity);
                            break;
                        case 1:
                            Instantiate(EnemySprite, EnemySpawn, Quaternion.identity);
                            break;
                        case 2:
                            Vector3 RightSpawn = EnemySpawn + new Vector3(1f, 0, 0);
                            Instantiate(EnemySprite, RightSpawn, Quaternion.identity);
                            break;
                        default:
                            Instantiate(EnemySprite, EnemySpawn, Quaternion.identity);
                            break;
                    }
                    yield return new WaitForSeconds(SpawnTime);
                }
            }
            else
            {
                
                for (int i = 0; i < SpawnCount; i++)
                {
                    //Regular Spawn Logic
                    Instantiate(EnemySprite, EnemySpawn, Quaternion.identity);
                    yield return new WaitForSeconds(SpawnTime);
                }
            }
        }
        
    }
    public void AssembleEnclaves(float SpawnCount, float SpawnTime, int[] SpawnIndexs, int PrefabID)
    {
        //SpawnCount is SpawnNumber, SpawnTime is SpawnSeconds
        //SpawnIndexs refers to our SpawnPoint Array to dispense our enemies
        //PrefabID grabs the Sprite for spawning in our prefab objects
        SpawnNumber = SpawnCount;
        SpawnSeconds = SpawnTime;
        GameObject PrefabChoice = EnclavePrefabs[PrefabID];
        StartCoroutine(EnclaveSpawner());
        IEnumerator EnclaveSpawner()
        {
            for (int i = 0; i < SpawnIndexs.Length; i++)
            {
                Instantiate(PrefabChoice, Spawnpoints[SpawnIndexs[i]].position, Quaternion.identity);
                yield return new WaitForSeconds(SpawnTime);
            }
        }
    }
}
