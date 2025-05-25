using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructurePlaceButtonScript : MonoBehaviour
{
    private StructureManager structureManager;
    private PlacementSystem placementSystem;

    [Header("Buttons")]
    public Button gathererButton;
    public Button normalTowerButton;
    public Button heavyTowerButton;
    public Button missileTowerButton;
    public Button walllButton;

    [Header("Structure Amounts")]
    public TMP_Text gathererAmountText;
    public TMP_Text normalTowerAmountText;
    public TMP_Text heavyTowerAmountText;
    public TMP_Text missileTowerAmountText;
    public TMP_Text wallAmountText;

    private void Awake()
    {
        // Find the StructureManager in the scene
        structureManager = FindFirstObjectByType<StructureManager>();


        if (structureManager == null)
        {
            Debug.LogError("StructureManager not found in the scene.");
        }

        // Find the PlacementSystem in the scene
        placementSystem = FindFirstObjectByType<PlacementSystem>();
    
    }

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        UpdateStructureAmounts();

        if (!structureManager.CanPlaceStructure(EStructureType.Gatherer))
        {
            if (gathererButton.interactable != false)
            {
                placementSystem.StopPlacement();
            }
            gathererButton.interactable = false;
        }
        else
        {
            gathererButton.interactable = true;
        }

        if (!structureManager.CanPlaceStructure(EStructureType.NormalTower))
        {
            if (normalTowerButton.interactable != false)
            {
                placementSystem.StopPlacement();
            }
            normalTowerButton.interactable = false;
        }
        else
        {
            normalTowerButton.interactable = true;
        }

        if (!structureManager.CanPlaceStructure(EStructureType.HeavyTower))
        {
            if (heavyTowerButton.interactable != false)
            {
                placementSystem.StopPlacement();
            }
            heavyTowerButton.interactable = false;
        }
        else
        {
            heavyTowerButton.interactable = true;
        }

        if (!structureManager.CanPlaceStructure(EStructureType.MissileTower))
        {
            if (missileTowerButton.interactable != false)
            {
                placementSystem.StopPlacement();
            }
            missileTowerButton.interactable = false;
        }
        else
        {
            missileTowerButton.interactable = true;
        }

        if (!structureManager.CanPlaceStructure(EStructureType.Wall))
        {
            if (walllButton.interactable != false)
            {
                placementSystem.StopPlacement();
            }
            walllButton.interactable = false;
        }
        else
        {
            walllButton.interactable = true;
        }

    }

    public void UpdateStructureAmounts()
    {
        int remainingGatherers = structureManager.maxGatherers - structureManager.numGatherers;
        int remainingNormalTowers = structureManager.maxNormalTowers - structureManager.numNormalTowers;
        int remainingHeavyTowers = structureManager.maxHeavyTowers - structureManager.numHeavyTowers;
        int remainingMissileTowers = structureManager.maxMissileTowers - structureManager.numMissileTowers;
        int remainingWalls = structureManager.maxWalls - structureManager.numWalls;

        gathererAmountText.text = remainingGatherers.ToString();
        normalTowerAmountText.text = remainingNormalTowers.ToString();
        heavyTowerAmountText.text = remainingHeavyTowers.ToString();
        missileTowerAmountText.text = remainingMissileTowers.ToString();
        wallAmountText.text = remainingWalls.ToString();

        ChangeColorToRed(gathererAmountText, remainingGatherers);
        ChangeColorToRed(normalTowerAmountText, remainingNormalTowers);
        ChangeColorToRed(heavyTowerAmountText, remainingHeavyTowers);
        ChangeColorToRed(missileTowerAmountText, remainingMissileTowers);
        ChangeColorToRed(wallAmountText, remainingWalls);
    }

    private void ChangeColorToRed(TMP_Text textComponent, int count)
    {
        if (count == 0)
        {
            textComponent.color = Color.red;
        }
        else
        {
            textComponent.color = Color.black;
        }

    }


}
