using TMPro;
using UnityEngine;

public class CampaignTimelineManager : MonoBehaviour
{
    [Header("Managers")]
    public ResourceManager resourceManager;
    public ScenarioManager scenarioManager;

    [Header("Timeline Settings")]
    public int currentDay = 1;
    public int maxDays = 12;

    [Header("Phase Settings")]
    public string[] phases = { "Early Campaign", "Mid Campaign", "Final Stretch", "Election Day" };

    [Header("UI")]
    public TMP_Text dayText;
    public TMP_Text phaseText;
    public TMP_Text statusText;
    public GameObject endPanel;
    public TMP_Text endResultText;

    private int currentPhaseIndex = 0;
    private bool gameEnded = false;

    private void Start()
    {
        UpdateTimelineUI();

        if (endPanel != null)
            endPanel.SetActive(false);

        // Start first campaign event immediately
        StartNextScenario();
    }
    
    public void StartNextScenario()
    {
        if (gameEnded)
            return;

        scenarioManager.ShowRandomScenario();
    }

    public void AdvanceDay()
    {
        if (gameEnded)
            return;

        currentDay++;

        CheckEarlyLoss();
        UpdatePhase();
        UpdateTimelineUI();

        if (currentDay > maxDays)
        {
            EndElection();
        }
    }

    private void UpdatePhase()
    {
        if (currentDay <= 4)
            currentPhaseIndex = 0;
        else if (currentDay <= 8)
            currentPhaseIndex = 1;
        else if (currentDay <= 12)
            currentPhaseIndex = 2;
        else
            currentPhaseIndex = 3;
    }

    private void CheckEarlyLoss()
    {
        if (resourceManager.campaignFunds <= 0)
        {
            EndGame("You lost early because your campaign ran out of funds.");
        }
        else if (resourceManager.popularity <= 10)
        {
            EndGame("You lost early because your popularity collapsed.");
        }
        else if (resourceManager.credibility <= 10)
        {
            EndGame("You lost early because your credibility collapsed.");
        }
    }

    private void EndElection()
    {
        int finalScore = resourceManager.popularity + resourceManager.credibility;

        if (resourceManager.campaignFunds <= 0)
        {
            EndGame("You lost the election. Your campaign ran out of money.");
        }
        else if (finalScore >= 120)
        {
            EndGame("You won the election! Your campaign built enough public trust and support.");
        }
        else
        {
            EndGame("You lost the election. Your final popularity and credibility were not strong enough.");
        }
    }

    private void EndGame(string message)
    {
        gameEnded = true;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (endResultText != null)
            endResultText.text = message;

        if (statusText != null)
            statusText.text = message;
    }

    private void UpdateTimelineUI()
    {
        if (dayText != null)
            dayText.text = "Day: " + currentDay + " / " + maxDays;

        if (phaseText != null)
            phaseText.text = "Phase: " + phases[currentPhaseIndex];

        if (statusText != null && !gameEnded)
            statusText.text = "Campaign in progress.";
    }
}