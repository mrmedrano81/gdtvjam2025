using System;
using UnityEngine;

public struct TowerStats
{
    public float health;
    public float damage;
    public float range;
    public float fireRate;
    public float rotationSpeed;
}

public class StructureManager : MonoBehaviour
{
    [Header("Normal Tower Stats")]
    public float normalTowerHealth = 100f;
    public float normalTowerDamage = 10f;
    public float normalTowerRange = 5f;
    public float normalTowerFirerate = 1f;
    public float normalTowerRotationSpeed = 100f;
    public NormalBulletLinePool normalTowerbulletTrailPool;
    public EnemyHitEffectPool normalTowerhitEffectPool;

    [Header("Heavy Tower Stats")]
    public float heavyTowerHealth = 100f;
    public float heavyTowerDamage = 10f;
    public float heavyTowerRange = 5f;
    public float heavyTowerFirerate = 1f;
    public float heavyTowerRotationSpeed = 100f;

    public float heavyTowerLaserLockonDuration = 0.7f;

    [Header("Missile Tower Stats")]
    public float missileTowerHealth = 100f;
    public float missileTowerDamage = 10f;
    public float missileTowerRange = 5f;
    public float missileTowerFirerate = 1f;
    public float missileTowerRotationSpeed = 100f;

    [Header("Wall Stats")]
    public float wallHealth = 100f;

    [Header("Structure Count Settings")]
    public int maxNormalTowers = 24;
    public int maxHeavyTowers = 8;
    public int maxMissileTowers = 4;
    public int maxWalls = 1000;
    public int maxGatherers = 10;
    public int maxHQs = 1;

    [Header("Structure Count")]
    public int numNormalTowers = 0;
    public int numHeavyTowers = 0;
    public int numMissileTowers = 0;
    public int numWalls = 0;
    public int numGatherers = 0;
    public int numHQs = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddStructureCount(EStructureType structureType, int count = 1)
    {
        switch (structureType)
        {
            case EStructureType.HQ:
                numHQs += count;
                break;
            case EStructureType.Gatherer:
                numGatherers += count;
                break;
            case EStructureType.NormalTower:
                numNormalTowers += count;
                break;
            case EStructureType.HeavyTower:
                numHeavyTowers += count;
                break;
            case EStructureType.MissileTower:
                numMissileTowers += count;
                break;
            case EStructureType.Wall:
                numWalls += count;
                break;
            default:
                Debug.LogError("Unknown structure type: " + structureType);
                break;
        }
    }

    public void OnStructureRemoved(GameObject structureObjectToBeRemoved, EStructureType structureType)
    {
        switch (structureType)
        {
            case EStructureType.HQ:
                break;
            case EStructureType.Gatherer:
                //SetupGatherer(structureObject.GetComponent<GathererScript>());
                break;
            case EStructureType.NormalTower:
                //SetupStructure(structureObject.GetComponent<NormalTowerScript>());
                break;
            case EStructureType.HeavyTower:
                //SetupStructure(structureObject.GetComponent<HeavyTowerScript>());
                break;
            case EStructureType.MissileTower:
                //SetupTower(structureObject.GetComponent<MissileTowerScript>());
                break;
            case EStructureType.Wall:
                structureObjectToBeRemoved.GetComponent<WallScript>().SetupWall(EWallPlacementStatus.Removing);
                break;
            default:
                Debug.LogError("Unknown structure type: " + structureType);
                break;
        }
    }

    public void SetupStructure(GameObject structureObject, EStructureType structureType, GridData gridData, Vector3 position, Vector3Int gridPosition)
    {
        switch(structureType)
        {
            case EStructureType.HQ:
                break;

            case EStructureType.Gatherer:
                //SetupGatherer(structureObject.GetComponent<GathererScript>());
                break;
            case EStructureType.NormalTower:
                SetupStructure(structureObject.GetComponent<NormalTowerScript>());
                break;
            case EStructureType.HeavyTower:
                SetupStructure(structureObject.GetComponent<HeavyTowerScript>());
                break;
            case EStructureType.MissileTower:
                //SetupTower(structureObject.GetComponent<MissileTowerScript>());
                break;
            case EStructureType.Wall:
                SetupWall(structureObject.GetComponent<WallScript>(), gridData, gridPosition);
                break;
            default:
                Debug.LogError("Unknown structure type: " + structureType);
                break;
        }

        AddStructureCount(structureType);
    }


    #region --- Remove Methods ---
    #endregion

    #region --- Setup Methods ---
    private void SetupWall(WallScript wallScript, GridData gridData, Vector3Int gridPosition)
    {
        wallScript.structureData = gridData;
        wallScript.health = wallHealth;
        wallScript.gridPosition = gridPosition;

        wallScript.SetupWall(EWallPlacementStatus.Placing);
    }

    public void SetupGatherer()
    {

    }

    public void SetupStructure(NormalTowerScript normalTowerScript)
    {
        TowerStats towerStats = new TowerStats
        {
            health = normalTowerHealth,
            damage = normalTowerDamage,
            range = normalTowerRange,
            fireRate = normalTowerFirerate,
            rotationSpeed = normalTowerRotationSpeed
        };

        normalTowerScript.SetNormalTowerStats(towerStats);
        normalTowerScript.bulletTrailPool = normalTowerbulletTrailPool;
        normalTowerScript.hitEffectPool =  normalTowerhitEffectPool;
    }

    public void SetupStructure(HeavyTowerScript heavyTowerScript)
    {
        TowerStats towerStats = new TowerStats
        {
            health = heavyTowerHealth,
            damage = heavyTowerDamage,
            range = heavyTowerRange,
            fireRate = heavyTowerFirerate,
            rotationSpeed = heavyTowerRotationSpeed
        };

        heavyTowerScript.laserLockonDuration = heavyTowerLaserLockonDuration;
        heavyTowerScript.SetHeavyTowerStats(towerStats);
    }

    //public void SetupTower(MissileTowerScript missileTower)
    //{

    //}

    #endregion
}
