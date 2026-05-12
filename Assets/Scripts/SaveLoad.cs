using System.Collections.Generic;
using System.IO;
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
    public int adsBought = 0;
    public int ralliesHeld = 0;
    public int prCampaignsRun = 0;

    [Header("Secret Unlocks")]
    public bool secretEndorsementUnlocked = false;
    public bool grassrootsMovementUnlocked = false;
    public bool corporateBackerUnlocked = false;
    public bool viralMomentUnlocked = false;

    [Header("Timeline")]
    public int currentDay = 1;

    public Serializable
    (
        List<int> _campaignResources, List<int> _groupSupport, 
        List<int> _actCosts, List<int> _roiVals, List<float> _bonuses, 
        string _route, List<int> _actions, List<bool> _unlocks,
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
        routeChosen = _route != "None";
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
            Load();
        }

    }

    // Update is called once per frame
    private void OnDisable()
    {
        if(autoSave){
            Save();
        }
    }

    private string PrepJson()
    {
        Serializable serializable = new Serializable
        (
            new List<int>{resourceManager.campaignFunds, resourceManager.popularity, resourceManager.credibility}, 
            new List<int>{resourceManager.youthSupport, resourceManager.workingClassSupport, resourceManager.corporateSupport},
            new List<int>{resourceManager.adsCost, resourceManager.rallyCost, resourceManager.prCost}, 
            new List<int>{resourceManager.popularityDollarValue, resourceManager.credibilityDollarValue},
            new List<float>{resourceManager.fundraisingBonus, resourceManager.prBonus, resourceManager.rallyBonus, resourceManager.adsPenalty}, 
            resourceManager.selectedRoute, 
            new List<int>{resourceManager.adsBought, resourceManager.ralliesHeld, resourceManager.prCampaignsRun},
            new List<bool>{
                resourceManager.secretEndorsementUnlocked, resourceManager.grassrootsMovementUnlocked, 
                resourceManager.corporateBackerUnlocked, resourceManager.viralMomentUnlocked
            },
            campaignTimelineManager.currentDay
        );

        return JsonUtility.ToJson(serializable);
    }

    private void LoadJson(string _jsonString)
    {
        Serializable serializable = JsonUtility.FromJson<Serializable>(_jsonString);
        resourceManager.campaignFunds = serializable.campaignFunds;
        resourceManager.popularity = serializable.popularity;
        resourceManager.credibility = serializable.credibility;
        resourceManager.youthSupport = serializable.youthSupport;
        resourceManager.workingClassSupport = serializable.workingClassSupport;
        resourceManager.corporateSupport = serializable.corporateSupport;
        resourceManager.adsCost = serializable.adsCost;
        resourceManager.rallyCost = serializable.rallyCost;
        resourceManager.prCost = serializable.prCost;
        resourceManager.popularityDollarValue = serializable.popularityDollarValue;
        resourceManager.credibilityDollarValue = serializable.credibilityDollarValue;
        resourceManager.fundraisingBonus = serializable.fundraisingBonus;
        resourceManager.prBonus = serializable.prBonus;
        resourceManager.rallyBonus = serializable.rallyBonus;
        resourceManager.adsPenalty = serializable.adsPenalty;
        resourceManager.selectedRoute = serializable.selectedRoute;
        campaignMenuManager.routeChosen = serializable.routeChosen;
        resourceManager.adsBought = serializable.adsBought;
        resourceManager.ralliesHeld = serializable.ralliesHeld;
        resourceManager.prCampaignsRun = serializable.prCampaignsRun;
        resourceManager.secretEndorsementUnlocked = serializable.secretEndorsementUnlocked;
        resourceManager.grassrootsMovementUnlocked = serializable.grassrootsMovementUnlocked;
        resourceManager.corporateBackerUnlocked = serializable.corporateBackerUnlocked;
        resourceManager.viralMomentUnlocked = serializable.viralMomentUnlocked;
        campaignTimelineManager.currentDay = serializable.currentDay;
    }

    public void Save()
    {
        string path = Path.Combine(Application.dataPath, fileName);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(PrepJson()); 
            }
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.dataPath, fileName);
        if (File.Exists(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string loadedString = reader.ReadToEnd();
                    LoadJson(loadedString);
                }
            }
        }
    }

    public void Reset()
    {
        string path = Path.Combine(Application.dataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        resourceManager.campaignFunds = 2000;
        resourceManager.popularity = 50;
        resourceManager.credibility = 50;

        resourceManager.youthSupport = 50;
        resourceManager.workingClassSupport = 50;
        resourceManager.corporateSupport = 50;
        
        resourceManager.adsCost = 100;
        resourceManager.rallyCost = 150;
        resourceManager.prCost = 200;
        
        resourceManager.popularityDollarValue = 25;
        resourceManager.credibilityDollarValue = 20;
        
        resourceManager.fundraisingBonus = 0f;
        resourceManager.prBonus = 0f;
        resourceManager.rallyBonus = 0f;
        resourceManager.adsPenalty = 0f;
        
        resourceManager.selectedRoute = "None";
        campaignMenuManager.routeChosen = false;
        
        resourceManager.adsBought = 0;
        resourceManager.ralliesHeld = 0;
        resourceManager.prCampaignsRun = 0;
        
        resourceManager.secretEndorsementUnlocked = false;
        resourceManager.grassrootsMovementUnlocked = false;
        resourceManager.corporateBackerUnlocked = false;
        resourceManager.viralMomentUnlocked = false;
        
        campaignTimelineManager.currentDay = 1;
        Save();
    }
}
