using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignMenuManager : MonoBehaviour
{
    [Header("Resource Manager")]
    public ResourceManager resourceManager;

    [Header("UI Text")]
    public TMP_Text routeText;
    public TMP_Text feedbackText;
    public TMP_Text routeEffectText;

    [Header("Route Settings")]
    public string currentRoute = "None";

    private bool routeChosen = false;

    public void RestartCampaign()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ChooseFundraisingRoute()
    {
        if (routeChosen)
        {
            ShowFeedback("You already chose a campaign route.");
            return;
        }

        currentRoute = "Fundraising";
        routeChosen = true;

        if (resourceManager != null)
        {
            resourceManager.GainFunds(400);
            resourceManager.DecreasePopularity(5);
            resourceManager.DecreaseCredibility(3);

            resourceManager.fundraisingBonus = 0.10f;
        }

        UpdateRouteUI(
            "Current Route: Fundraising",
            "Perks: +$400 funds, +10% future fundraising bonus\nLosses: -5 popularity, -3 credibility"
        );

        ShowFeedback("You chose Fundraising. You gained money, but some voters distrust your donor-focused strategy.");
    }

    public void ChooseMediaOutreachRoute()
    {
        if (routeChosen)
        {
            ShowFeedback("You already chose a campaign route.");
            return;
        }

        currentRoute = "Media Outreach";
        routeChosen = true;

        if (resourceManager != null)
        {
            resourceManager.LoseFunds(150);
            resourceManager.IncreasePopularity(10);
            resourceManager.IncreaseCredibility(6);

            resourceManager.prBonus = 0.15f;
        }

        UpdateRouteUI(
            "Current Route: Media Outreach",
            "Perks: +10 popularity, +6 credibility, +15% stronger PR campaigns\nLosses: -$150 funds"
        );

        ShowFeedback("You chose Media Outreach. Your public image improved, but it cost campaign money.");
    }

    public void ChooseVoterEngagementRoute()
    {
        if (routeChosen)
        {
            ShowFeedback("You already chose a campaign route.");
            return;
        }

        currentRoute = "Voter Engagement";
        routeChosen = true;

        if (resourceManager != null)
        {
            resourceManager.LoseFunds(100);
            resourceManager.IncreasePopularity(8);
            resourceManager.IncreaseCredibility(4);

            resourceManager.rallyBonus = 0.20f;
            resourceManager.adsPenalty = 0.10f;
        }

        UpdateRouteUI(
            "Current Route: Voter Engagement",
            "Perks: +8 popularity, +4 credibility, +20% stronger rallies\nLosses: -$100 funds, ads are 10% less effective"
        );

        ShowFeedback("You chose Voter Engagement. Grassroots support increased, but your ad strategy became weaker.");
    }

    private void UpdateRouteUI(string routeMessage, string effectMessage)
    {
        if (routeText != null)
        {
            routeText.text = routeMessage;
        }

        if (routeEffectText != null)
        {
            routeEffectText.text = effectMessage;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }
}