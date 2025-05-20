using UnityEngine;

public struct TowerStats
{
    public float health;
    public float damage;
    public float range;
    public float fireRate;
    public float rotationSpeed;
}

public struct EnemyStats
{
    public float health;
    public float damage;
    public float speed;
}

public class CombatStatusSystem : MonoBehaviour
{
    [Header("Normal Tower Stats")]
    public float normalTowerHealth = 100f;
    public float normalTowerDamage = 10f;
    public float normalTowerRange = 5f;
    public float normalTowerFirerate = 1f;

    [Header("Heavy Tower Stats")]
    public float heavyTowerHealth = 100f;
    public float heavyTowerDamage = 10f;
    public float heavyTowerRange = 5f;
    public float heavyTowerFirerate = 1f;

    public float heavyTowerLaserLockonDuration = 0.7f;


    [Header("Missile Tower Stats")]
    public float missileTowerHealth = 100f;
    public float missileTowerDamage = 10f;
    public float missileTowerRange = 5f;
    public float missileTowerFirerate = 1f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupTower(NormalTowerScript normalTowerShoot)
    {
        TowerStats towerStats = new TowerStats
        {
            health = normalTowerHealth,
            damage = normalTowerDamage,
            range = normalTowerRange,
            fireRate = normalTowerFirerate
        };

        normalTowerShoot.SetNormalTowerStats(towerStats);
    }

    public void SetupTower(HeavyTowerScript heavyTower)
    {
        TowerStats towerStats = new TowerStats
        {
            health = heavyTowerHealth,
            damage = heavyTowerDamage,
            range = heavyTowerRange,
            fireRate = heavyTowerFirerate
        };

        heavyTower.laserLockonDuration = heavyTowerLaserLockonDuration;
        heavyTower.SetHeavyTowerStats(towerStats);
    }

    //public void SetupTower(MissileTowerScript missileTower)
    //{

    //}
}
