using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public enum EnemyTypes
    {
        EVOLVER,
        JUMPER
    };

    public int EnemyCount
    {
        get { return m_EnemyCoordinates.Count; }
    }

    public GameObject m_EvolverEnemyLevel1Prefab;
    public GameObject m_EvolverEnemyLevel2Prefab;
    public GameObject m_EvolverEnemyLevel3Prefab;
    public GameObject m_JumperEnemyPrefab;
    public AudioClip m_EvolutionSound;

    private GameObject m_EnemyParent;
    private List<int> m_EnemyCoordinates = new List<int>();

    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyManager>();
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance.Init();
            Destroy(this.gameObject);
            return;
        }


        _instance = this;
        transform.parent = null;
        DontDestroyOnLoad(_instance.gameObject);

        Init();
    }
    
    protected void Init()
    {
        
    }

    //Public functions

    public void CreateEnemyParent()
    {
        m_EnemyParent = new GameObject("Enemy Parent");
        m_EnemyParent.transform.parent = null;
    }

    public void CreateEnemy(EnemyTypes a_Type, Vector3 a_Pos)
    {
        GameObject go = null;
        Vector3 spawnPos;
        switch (a_Type)
        {
            case EnemyTypes.EVOLVER:
                spawnPos = new Vector3(a_Pos.x, m_EvolverEnemyLevel1Prefab.transform.position.y, a_Pos.z);
                go = (GameObject)Instantiate(m_EvolverEnemyLevel1Prefab, spawnPos, Quaternion.identity);

                break;
            case EnemyTypes.JUMPER:
                spawnPos = new Vector3(a_Pos.x, m_JumperEnemyPrefab.transform.position.y, a_Pos.z);
                go = (GameObject)Instantiate(m_JumperEnemyPrefab, spawnPos, Quaternion.identity);
                break;
            default:
                throw new ArgumentOutOfRangeException("a_Type");
        }

        go.transform.parent = m_EnemyParent.transform;
        m_EnemyCoordinates.Add((int)a_Pos.x * (int)a_Pos.z);
        go.GetComponent<Enemy>().Init();
    }

    public void CreateRandomEnemy(EnemyTypes a_Type, Vector3 a_Pos)
    {
        GameObject go = null;

        Array arr = Enum.GetValues(typeof (EnemyTypes));

        EnemyTypes randType = (EnemyTypes)arr.GetValue(Random.Range(0, arr.Length) );
        Vector3 spawnPos;
        switch (randType)
        {
            case EnemyTypes.EVOLVER:
                spawnPos = new Vector3(a_Pos.x, m_EvolverEnemyLevel1Prefab.transform.position.y, a_Pos.z);
                go = (GameObject)Instantiate(m_EvolverEnemyLevel1Prefab, spawnPos, Quaternion.identity);
                break;
            case EnemyTypes.JUMPER:
                spawnPos = new Vector3(a_Pos.x, m_JumperEnemyPrefab.transform.position.y, a_Pos.z);
                go = (GameObject)Instantiate(m_JumperEnemyPrefab, spawnPos, Quaternion.identity);
                break;
            default:
                throw new ArgumentOutOfRangeException("a_Type");
        }

        go.transform.parent = m_EnemyParent.transform;
        m_EnemyCoordinates.Add((int)a_Pos.x * (int)a_Pos.z);
        go.GetComponent<Enemy>().Init();
    }

    public bool IsCellFilledWithEnemy(int a_X, int a_Y)
    {
        return m_EnemyCoordinates.Contains(a_X*a_Y);
    }

    public void ClearEnemies()
    {
        m_EnemyCoordinates.Clear();
        Destroy(m_EnemyParent);
    }

    public void EvolveEnemy(GameObject enemyEvolver)
    {
        //Create a new enemy of the next evolution in the same place as this one
        //Copy and needed attributes

        var evolveLevel = enemyEvolver.GetComponent<Enemy_Evolver>().GetEvolutionStage();
        if (evolveLevel >= 2)
            return;

        evolveLevel++;
        var spawnPos = enemyEvolver.transform.position;
        var spawnRot = enemyEvolver.transform.rotation;
        AudioSource.PlayClipAtPoint(m_EvolutionSound, spawnPos);
        Destroy(enemyEvolver);
        switch (evolveLevel)
        {
            case 1:
                spawnPos.y = m_EvolverEnemyLevel2Prefab.transform.position.y;
                Instantiate(m_EvolverEnemyLevel2Prefab, spawnPos, spawnRot);
                break;

            case 2:
                spawnPos.y = m_EvolverEnemyLevel3Prefab.transform.position.y;
                Instantiate(m_EvolverEnemyLevel3Prefab, spawnPos, spawnRot);
                break;
        }
    }
}