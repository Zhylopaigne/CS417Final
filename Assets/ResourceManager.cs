using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Campaign Resources")]
    public int campaignFunds = 1000;
    public int popularity = 50;
    public int credibility = 50;

    [Header("UI Text")]
    public TMP_Text fundsText;
    public TMP_Text popularityText;
    public TMP_Text credibilityText;
    public TMP_Text feedbackText;
    public TMP_Text roiText;

    [Header("Action Count UI")]
    public TMP_Text adsCountText;
    public TMP_Text ralliesCountText;
    public TMP_Text prCampaignsCountText;

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

    private int adsBought = 0;
    private int ralliesHeld = 0;
    private int prCampaignsRun = 0;

    void Start()
    {
        UpdateUI();
        ShowFeedback("Choose a campaign action.");
        ShowROI("ROI: No action taken yet.");
    }

    public void BuyAds()
    {
        int finalPopularityGain = 5;

        if (adsPenalty > 0f)
        {
            finalPopularityGain = Mathf.RoundToInt(finalPopularityGain * (1f - adsPenalty));
        }

        bool actionWorked = TryCampaignAction(
            "Ads",
            adsCost,
            popularityGain: finalPopularityGain,
            credibilityGain: 1,
            fundsGain: 0
        );

        if (actionWorked)
        {
            adsBought++;
            UpdateUI();
        }
    }

    public void HoldRally()
    {
        int finalPopularityGain = 8;

        if (rallyBonus > 0f)
        {
            finalPopularityGain = Mathf.RoundToInt(finalPopularityGain * (1f + rallyBonus));
        }

        bool actionWorked = TryCampaignAction(
            "Rally",
            rallyCost,
            popularityGain: finalPopularityGain,
            credibilityGain: 2,
            fundsGain: 25
        );

        if (actionWorked)
        {
            ralliesHeld++;
            UpdateUI();
        }
    }

    public void RunPRCampaign()
    {
        int finalCredibilityGain = 10;

        if (prBonus > 0f)
        {
            finalCredibilityGain = Mathf.RoundToInt(finalCredibilityGain * (1f + prBonus));
        }

        bool actionWorked = TryCampaignAction(
            "PR Campaign",
            prCost,
            popularityGain: 3,
            credibilityGain: finalCredibilityGain,
            fundsGain: 0
        );

        if (actionWorked)
        {
            prCampaignsRun++;
            UpdateUI();
        }
    }

    private bool TryCampaignAction(string actionName, int cost, int popularityGain, int credibilityGain, int fundsGain)
    {
        if (campaignFunds < cost)
        {
            ShowFeedback("Not enough funds for " + actionName + ".");
            ShowROI("ROI: Action failed because you could not afford it.");
            return false;
        }

        campaignFunds -= cost;

        int randomEvent = Random.Range(0, 100);

        if (randomEvent < 15)
        {
            ApplyBadOutcome(actionName, cost);
        }
        else if (randomEvent < 35)
        {
            ApplyGreatOutcome(actionName, cost, popularityGain, credibilityGain, fundsGain);
        }
        else
        {
            ApplyNormalOutcome(actionName, cost, popularityGain, credibilityGain, fundsGain);
        }

        ClampStats();
        UpdateUI();

        return true;
    }

    private void ApplyNormalOutcome(string actionName, int cost, int popularityGain, int credibilityGain, int fundsGain)
    {
        popularity += popularityGain;
        credibility += credibilityGain;
        campaignFunds += fundsGain;

        float roi = CalculateROI(cost, popularityGain, credibilityGain, fundsGain);

        ShowFeedback(actionName + " worked as expected.");
        ShowROI("ROI from " + actionName + ": " + roi.ToString("F1") + "%");
    }

    private void ApplyGreatOutcome(string actionName, int cost, int popularityGain, int credibilityGain, int fundsGain)
    {
        int bonusPopularity = popularityGain + 3;
        int bonusCredibility = credibilityGain + 2;
        int bonusFunds = fundsGain + 50;

        popularity += bonusPopularity;
        credibility += bonusCredibility;
        campaignFunds += bonusFunds;

        float roi = CalculateROI(cost, bonusPopularity, bonusCredibility, bonusFunds);

        ShowFeedback("Great result! " + actionName + " was more successful than expected.");
        ShowROI("ROI from " + actionName + ": " + roi.ToString("F1") + "%");
    }

    private void ApplyBadOutcome(string actionName, int cost)
    {
        int popularityLoss = 3;
        int credibilityLoss = 2;

        popularity -= popularityLoss;
        credibility -= credibilityLoss;

        float roi = CalculateROI(cost, -popularityLoss, -credibilityLoss, 0);

        ShowFeedback("Bad result! " + actionName + " received negative media attention.");
        ShowROI("ROI from " + actionName + ": " + roi.ToString("F1") + "%");
    }

    private float CalculateROI(int cost, int popularityChange, int credibilityChange, int fundsGain)
    {
        int benefitValue = 0;

        benefitValue += popularityChange * popularityDollarValue;
        benefitValue += credibilityChange * credibilityDollarValue;
        benefitValue += fundsGain;

        float roi = ((float)(benefitValue - cost) / cost) * 100f;

        return roi;
    }

    public void GainFunds(int amount)
    {
        int finalAmount = amount;

        if (fundraisingBonus > 0f)
        {
            finalAmount = Mathf.RoundToInt(amount * (1f + fundraisingBonus));
        }

        campaignFunds += finalAmount;

        ShowFeedback("Your campaign gained $" + finalAmount + " in funding.");
        UpdateUI();
    }

    public void LoseFunds(int amount)
    {
        campaignFunds -= amount;

        if (campaignFunds < 0)
        {
            campaignFunds = 0;
        }

        ShowFeedback("Your campaign lost $" + amount + ".");
        UpdateUI();
    }

    public void IncreasePopularity(int amount)
    {
        popularity += amount;
        ClampStats();
        ShowFeedback("Popularity increased by " + amount + ".");
        UpdateUI();
    }

    public void DecreasePopularity(int amount)
    {
        popularity -= amount;
        ClampStats();
        ShowFeedback("Popularity decreased by " + amount + ".");
        UpdateUI();
    }

    public void IncreaseCredibility(int amount)
    {
        credibility += amount;
        ClampStats();
        ShowFeedback("Credibility increased by " + amount + ".");
        UpdateUI();
    }

    public void DecreaseCredibility(int amount)
    {
        credibility -= amount;
        ClampStats();
        ShowFeedback("Credibility decreased by " + amount + ".");
        UpdateUI();
    }

    public void FundraisingDinner()
    {
        GainFunds(200);
        IncreasePopularity(2);
        ShowROI("ROI: Fundraising dinner increased funds and slightly improved popularity.");
    }

    public void DonorDonation()
    {
        GainFunds(150);
        ShowROI("ROI: Donation gained campaign funds without spending money.");
    }

    public void CampaignScandal()
    {
        LoseFunds(100);
        DecreasePopularity(8);
        DecreaseCredibility(5);
        ShowROI("ROI: Scandal caused a negative campaign return.");
    }

    private void UpdateUI()
    {
        if (fundsText != null)
        {
            fundsText.text = "Funds: $" + campaignFunds;
        }

        if (popularityText != null)
        {
            popularityText.text = "Popularity: " + popularity;
        }

        if (credibilityText != null)
        {
            credibilityText.text = "Credibility: " + credibility;
        }

        if (adsCountText != null)
        {
            adsCountText.text = "Ads Bought: " + adsBought;
        }

        if (ralliesCountText != null)
        {
            ralliesCountText.text = "Rallies Held: " + ralliesHeld;
        }

        if (prCampaignsCountText != null)
        {
            prCampaignsCountText.text = "PR Campaigns Run: " + prCampaignsRun;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void ShowROI(string message)
    {
        if (roiText != null)
        {
            roiText.text = message;
        }
    }

    private void ClampStats()
    {
        popularity = Mathf.Clamp(popularity, 0, 100);
        credibility = Mathf.Clamp(credibility, 0, 100);
    }
}