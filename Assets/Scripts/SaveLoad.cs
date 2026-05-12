using System.Resources;
using UnityEngine;

[System.Serializable]
public class Serializable
{
    [Header("Campaign Resources")]
    public int campaignFunds = 2000;
    public int popularity = 50;
    public int credibility = 50;

    [Header("Voter Group Support")]
    public int youthSupport = 50;
    public int workingClassSupport = 50;
    public int corporateSupport = 50;

    [Header("Action Costs")]
    public int adsCost = 100;
    public int rallyCost = 150;
    public int prCost = 200;

    [Header("ROI Values")]
    public int popularityDollarValue = 25;
    public int credibilityDollarValue = 20;

    [Header("Route Bonuses and Penalties")]
    public float fundraisingBonus = 0f;
    public float prBonus = 0f;
    public float rallyBonus = 0f;
    public float adsPenalty = 0f;

    [Header("Current Route")]
    public string selectedRoute = "None";
    public bool routeChosen = false;

    [Header("Action Stats")]
    private int adsBought = 0;
    private int ralliesHeld = 0;
    private int prCampaignsRun = 0;

    [Header("Secret Unlocks")]
    private bool secretEndorsementUnlocked = false;
    private bool grassrootsMovementUnlocked = false;
    private bool corporateBackerUnlocked = false;
    private bool viralMomentUnlocked = false;

    [Header("Timeline")]
    public int currentDay = 1;

    public Serializable
    (
        int[] _campaignResources, int[] _groupSupport, 
        int[] _actCosts, int[] _roiVals, float[] _bonuses, 
        string _route, int[] _actions, bool[] _unlocks,
        int _curDay
    )
    {
        campaignFunds = _campaignResources[0];
        popularity = _campaignResources[1];
        credibility = _campaignResources[2];
        youthSupport = _groupSupport[0];
        workingClassSupport = _groupSupport[1];
        corporateSupport = _groupSupport[2];
        adsCost = _actCosts[0];
        rallyCost = _actCosts[1];
        prCost = _actCosts[2];
        popularityDollarValue = _roiVals[0];
        credibilityDollarValue = _roiVals[1];
        fundraisingBonus = _bonuses[0];
        prBonus = _bonuses[1];
        rallyBonus = _bonuses[2];
        adsPenalty = _bonuses[3];
        selectedRoute = _route;
        routeChosen = selectedRoute != "None";
        adsBought = _actions[0];
        ralliesHeld = _actions[1];
        prCampaignsRun = _actCosts[2];
        secretEndorsementUnlocked = _unlocks[0];
        grassrootsMovementUnlocked = _unlocks[1];
        corporateBackerUnlocked = _unlocks[2];
        viralMomentUnlocked = _unlocks[3];
        currentDay = _curDay;
    }
}
public class SaveLoad : MonoBehaviour
{
    public ResourceManager resourceManager;
    public CampaignMenuManager campaignMenuManager;
    public CampaignTimelineManager campaignTimelineManager;
    public ScenarioManager scenarioManager;

    public bool autoSave = true;
    public string fileName = "SAVE.dat";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (autoSave)
        {
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
