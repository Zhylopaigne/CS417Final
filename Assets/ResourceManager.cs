using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Campaign Resources")]
    public int campaignFunds = 2000;
    public int popularity = 50;
    public int credibility = 50;

    [Header("Voter Group Support")]
    public int youthSupport = 50;
    public int workingClassSupport = 50;
    public int corporateSupport = 50;

    [Header("UI Text")]
    public TMP_Text fundsText;
    public TMP_Text popularityText;
    public TMP_Text credibilityText;
    public TMP_Text feedbackText;
    public TMP_Text roiText;

    [Header("Voter UI Text")]
    public TMP_Text globalApprovalText;
    public TMP_Text youthSupportText;
    public TMP_Text workingClassSupportText;
    public TMP_Text corporateSupportText;
    public TMP_Text voterFeedbackText;


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

    [Header("Cooldown Settings")]
    public float adsCooldown = 5f;
    public float rallyCooldown = 10f;
    public float prCooldown = 12f;
    public float fundraisingDinnerCooldown = 15f;
    public float riskyDonationCooldown = 20f;

    private float nextAdsTime = 0f;
    private float nextRallyTime = 0f;
    private float nextPRTime = 0f;
    private float nextFundraisingDinnerTime = 0f;
    private float nextRiskyDonationTime = 0f;

    [Header("Cooldown UI")]
    public TMP_Text cooldownText;

    [Header("Route Bonuses and Penalties")]
    public float fundraisingBonus = 0f;
    public float prBonus = 0f;
    public float rallyBonus = 0f;
    public float adsPenalty = 0f;
    [Header("Current Route")]
    public string selectedRoute = "None";

    public int adsBought = 0;
    public int ralliesHeld = 0;
    public int prCampaignsRun = 0;

    [Header("Secret Unlocks")]
    public TMP_Text secretText;

    public bool secretEndorsementUnlocked = false;
    public bool grassrootsMovementUnlocked = false;
    public bool corporateBackerUnlocked = false;
    public bool viralMomentUnlocked = false;

    void Start()
    {
        UpdateUI();
        ShowFeedback("Choose a campaign action.");
        ShowROI("ROI: No action taken yet.");
        ShowVoterFeedback("Voter groups are waiting for your campaign decisions.");
    }

    private bool IsOnCooldown(float nextAvailableTime, string actionName)
    {
        if (Time.time < nextAvailableTime)
        {
            float remainingTime = nextAvailableTime - Time.time;
            ShowFeedback(actionName + " is on cooldown. Wait " + remainingTime.ToString("F1") + " seconds.");
            ShowCooldown(actionName + " cooldown: " + remainingTime.ToString("F1") + "s");
            return true;
        }

        return false;
    }

    private void ShowCooldown(string message)
    {
        if (cooldownText != null)
        {
            cooldownText.text = message;
        }
    }
    public void BuyAds()
    {
        if (IsOnCooldown(nextAdsTime, "Ads"))
        {
            return;
        }

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
            nextAdsTime = Time.time + adsCooldown;
            ShowCooldown("Ads are now on cooldown for " + adsCooldown + "s.");
            UpdateUI();
        }
    }

    public void HoldRally()
    {
        if (IsOnCooldown(nextRallyTime, "Rally"))
        {
            return;
        }

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
            nextRallyTime = Time.time + rallyCooldown;
            ShowCooldown("Rally is now on cooldown for " + rallyCooldown + "s.");
            UpdateUI();
        }
    }

    public void RunPRCampaign()
    {
        if (IsOnCooldown(nextPRTime, "PR Campaign"))
        {
            return;
        }

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
            nextPRTime = Time.time + prCooldown;
            ShowCooldown("PR Campaign is now on cooldown for " + prCooldown + "s.");
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

        ApplyRoutePassiveEffect(actionName);
        ClampStats();
        UpdateUI();
        TryRandomDonorDonation();
        CheckSecretUnlocks();
        return true;
    }

    private void ApplyNormalOutcome(string actionName, int cost, int popularityGain, int credibilityGain, int fundsGain)
    {
        popularity += popularityGain;
        credibility += credibilityGain;
        campaignFunds += fundsGain;

        ApplyVoterGroupResponse(actionName, 1f);

        ShowFeedback(actionName + " worked as expected.");
        ShowROIDetails(actionName, cost, popularityGain, credibilityGain, fundsGain);
    }

    private void ApplyGreatOutcome(string actionName, int cost, int popularityGain, int credibilityGain, int fundsGain)
    {
        int bonusPopularity = popularityGain + 3;
        int bonusCredibility = credibilityGain + 2;
        int bonusFunds = fundsGain + 50;

        popularity += bonusPopularity;
        credibility += bonusCredibility;
        campaignFunds += bonusFunds;

        ApplyVoterGroupResponse(actionName, 1.5f);

        ShowFeedback("Great result! " + actionName + " was more successful than expected.");
        ShowROIDetails(actionName, cost, bonusPopularity, bonusCredibility, bonusFunds);
    }

    private void ApplyBadOutcome(string actionName, int cost)
    {
        int popularityLoss = 3;
        int credibilityLoss = 2;

        popularity -= popularityLoss;
        credibility -= credibilityLoss;

        ApplyVoterGroupResponse(actionName, -1f);

        ShowFeedback("Bad result! " + actionName + " received negative media attention.");
        ShowROIDetails(actionName, cost, -popularityLoss, -credibilityLoss, 0);
    }

    
    private string FormatChange(int amount)
    {
        if (amount > 0)
        {
            return "+" + amount;
        }

        return amount.ToString();
    }

    private float CalculateROI(int cost, int popularityChange, int credibilityChange, int fundsGain)
    {
        int benefitValue = CalculateBenefitValue(popularityChange, credibilityChange, fundsGain);
        float roi = ((float)(benefitValue - cost) / cost) * 100f;
        return roi;
    }

    private int CalculateBenefitValue(int popularityChange, int credibilityChange, int fundsGain)
    {
        int benefitValue = 0;
        benefitValue += popularityChange * popularityDollarValue;
        benefitValue += credibilityChange * credibilityDollarValue;
        benefitValue += fundsGain;
        return benefitValue;
    }
    private void ShowROIDetails(string actionName, int cost, int popularityChange, int credibilityChange, int fundsGain)
    {
        int benefitValue = CalculateBenefitValue(popularityChange, credibilityChange, fundsGain);
        int netValue = benefitValue - cost;
        float roi = ((float)netValue / cost) * 100f;

        string netText = netValue >= 0 ? "+$" + netValue : "-$" + Mathf.Abs(netValue);

        ShowROI(
            actionName + " ROI: " + roi.ToString("F1") + "%\n" +
            "Net Value: " + netText + "\n" +
            "Impact: Popularity " + FormatChange(popularityChange) +
            ", Credibility " + FormatChange(credibilityChange)
        );
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
        if (IsOnCooldown(nextFundraisingDinnerTime, "Fundraising Dinner"))
        {
            return;
        }

        int dinnerCost = 100;

        if (campaignFunds < dinnerCost)
        {
            ShowFeedback("Not enough funds to host a fundraising dinner.");
            ShowROI("ROI: Fundraising dinner failed because you could not afford it.");
            return;
        }

    campaignFunds -= dinnerCost;

        int randomEvent = Random.Range(0, 100);

        if (randomEvent < 20)
        {
            int fundsGained = 50;
            campaignFunds += fundsGained;

            popularity -= 3;
            credibility -= 2;

            youthSupport -= 5;
            workingClassSupport -= 4;
            corporateSupport -= 3;

            float roi = ((float)(fundsGained - dinnerCost) / dinnerCost) * 100f;

            ClampStats();
            UpdateUI();

            ShowFeedback("Bad dinner! You spent $100, only raised $50, Popularity -3, Credibility -2.");
            ShowVoterFeedback("Voter reaction: Youth -5, Working Class -4, Corporate -3.");
            ShowROI("ROI from Fundraising Dinner: " + roi.ToString("F1") + "%");
        }
        else
        {
            int fundsGained = 250;

            if (fundraisingBonus > 0f)
            {
                fundsGained = Mathf.RoundToInt(fundsGained * (1f + fundraisingBonus));
            }

            campaignFunds += fundsGained;

            popularity -= 2;
            credibility += 1;

            youthSupport -= 3;
            workingClassSupport -= 2;
            corporateSupport += 6;

            float roi = ((float)(fundsGained - dinnerCost) / dinnerCost) * 100f;

            ClampStats();
            UpdateUI();

            ShowFeedback("Dinner succeeded! You spent $100, raised $" + fundsGained + ", Popularity -2, Credibility +1.");
            ShowVoterFeedback("Voter reaction: Youth -3, Working Class -2, Corporate +6.");
            ShowROI("ROI from Fundraising Dinner: " + roi.ToString("F1") + "%");
        }
        CheckSecretUnlocks();
        nextFundraisingDinnerTime = Time.time + fundraisingDinnerCooldown;
    }

    public void DonorDonation()
    {
        GainFunds(150);
        corporateSupport += 2;
        ClampStats();
        UpdateUI();

        ShowVoterFeedback("A donor helped your campaign. Corporate support +2.");
        ShowROI("ROI: Donation gained campaign funds without direct spending.");
    }

    private void TryRandomDonorDonation()
    {
        int donationChance = 15;

        if (fundraisingBonus > 0f)
        {
            donationChance = 25;
        }

        int randomRoll = Random.Range(0, 100);

        if (randomRoll < donationChance)
        {
            DonorDonation();
        }
    }

    public void AcceptRiskyDonation()
    {
        if (IsOnCooldown(nextRiskyDonationTime, "Risky Donation"))
        {
            return;
        }

        int donationAmount = 300;
        campaignFunds += donationAmount;

        int scandalChance = Random.Range(0, 100);

        if (scandalChance < 30)
        {
            popularity -= 8;
            credibility -= 5;

            youthSupport -= 8;
            workingClassSupport -= 6;
            corporateSupport -= 4;

            ClampStats();
            UpdateUI();

            ShowFeedback("You accepted a risky donation and got $300, but a scandal broke out! Popularity -8, Credibility -5.");
            ShowVoterFeedback("Scandal reaction: Youth -8, Working Class -6, Corporate -4.");
            ShowROI("ROI: Risky donation gave funds, but damaged your campaign image.");
        }
        else
        {
            popularity += 2;

            youthSupport -= 1;
            workingClassSupport -= 1;
            corporateSupport += 5;

            ClampStats();
            UpdateUI();

            ShowFeedback("You accepted a risky donation and gained $300. No scandal was discovered.");
            ShowVoterFeedback("Voter reaction: Youth -1, Working Class -1, Corporate +5.");
            ShowROI("ROI: Risky donation was successful, but it carried political risk.");
        }
        CheckSecretUnlocks();
        nextRiskyDonationTime = Time.time + riskyDonationCooldown;
    }

    private void UpdateUI()
    {
        int globalApproval = CalculateGlobalApproval();

        if (fundsText != null)
        {
            fundsText.text = "Funds: $" + campaignFunds;
        }

        if (popularityText != null)
        {
            popularityText.text = "Popularity: " + popularity + "%";
        }

        if (credibilityText != null)
        {
            credibilityText.text = "Credibility: " + credibility + "%";
        }

        if (globalApprovalText != null)
        {
            globalApprovalText.text =
                "Overall Approval: " + globalApproval + "%\n" +
                MakeTextBar(globalApproval);
        }

        if (youthSupportText != null)
        {
            youthSupportText.text =
                "Youth Support: " + youthSupport + "%\n" +
                MakeTextBar(youthSupport);
        }

        if (workingClassSupportText != null)
        {
            workingClassSupportText.text =
                "Working Class Support: " + workingClassSupport + "%\n" +
                MakeTextBar(workingClassSupport);
        }

        if (corporateSupportText != null)
        {
            corporateSupportText.text =
                "Corporate Support: " + corporateSupport + "%\n" +
                MakeTextBar(corporateSupport);
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

    private void ApplyVoterGroupResponse(string actionName, float multiplier)
        {
            int youthChange = 0;
            int workingChange = 0;
            int corporateChange = 0;

            if (actionName == "Ads")
            {
                youthChange = 4;
                workingChange = 2;
                corporateChange = 1;
            }
            else if (actionName == "Rally")
            {
                youthChange = 6;
                workingChange = 5;
                corporateChange = -1;
            }
            else if (actionName == "PR Campaign")
            {
                youthChange = 1;
                workingChange = 2;
                corporateChange = 6;
            }

            youthChange = Mathf.RoundToInt(youthChange * multiplier);
            workingChange = Mathf.RoundToInt(workingChange * multiplier);
            corporateChange = Mathf.RoundToInt(corporateChange * multiplier);

            youthSupport += youthChange;
            workingClassSupport += workingChange;
            corporateSupport += corporateChange;

            ClampStats();

            ShowVoterFeedback(
                actionName + " voter reaction: Youth " + FormatChange(youthChange) +
                ", Working Class " + FormatChange(workingChange) +
                ", Corporate " + FormatChange(corporateChange) + "."
            );
        }

    private void ShowVoterFeedback(string message)
    {
        if (voterFeedbackText != null)
        {
            voterFeedbackText.text = message;
        }
    }

    private void ApplyRoutePassiveEffect(string actionName)
        {
            if (selectedRoute == "Fundraising")
            {
                int donationRoll = Random.Range(0, 100);

                if (donationRoll < 30)
                {
                    int bonusFunds = 75;

                    campaignFunds += bonusFunds;

                    ShowVoterFeedback("Fundraising route bonus: Donors contributed an extra $" + bonusFunds + ".");
                }
                else
                {
                    ShowVoterFeedback("Fundraising route active: Donor network is building support.");
                }
            }
            else if (selectedRoute == "Media Outreach")
            {
                credibility += 1;
                corporateSupport += 1;

                ShowVoterFeedback("Media Outreach route bonus: Credibility +1, Corporate Support +1.");
            }
            else if (selectedRoute == "Voter Engagement")
            {
                youthSupport += 1;
                workingClassSupport += 1;

                ShowVoterFeedback("Voter Engagement route bonus: Youth Support +1, Working Class Support +1.");
            }

            ClampStats();
        }
    private void ClampStats()
    {
        popularity = Mathf.Clamp(popularity, 0, 100);
        credibility = Mathf.Clamp(credibility, 0, 100);

        youthSupport = Mathf.Clamp(youthSupport, 0, 100);
        workingClassSupport = Mathf.Clamp(workingClassSupport, 0, 100);
        corporateSupport = Mathf.Clamp(corporateSupport, 0, 100);
    }
    private int CalculateGlobalApproval()
    {
        return Mathf.RoundToInt((popularity + youthSupport + workingClassSupport + corporateSupport) / 4f);
    }
    private string MakeTextBar(int value)
    {
        int filledBars = Mathf.RoundToInt(value / 10f);
        int emptyBars = 10 - filledBars;

        string bar = "[";

        for (int i = 0; i < filledBars; i++)
        {
            bar += "|";
        }

        for (int i = 0; i < emptyBars; i++)
        {
            bar += ".";
        }

        bar += "]";

        return bar;
    }
    private void CheckSecretUnlocks()
    {
        if (!secretEndorsementUnlocked &&
            prCampaignsRun >= 4 &&
            credibility >= 80 &&
            corporateSupport >= 65)
        {
            secretEndorsementUnlocked = true;

            popularity += 5;
            credibility += 5;
            corporateSupport += 3;

            ClampStats();
            UpdateUI();

            ShowSecret(
                "Secret Unlocked: Major Endorsement!\n" +
                "Requirement met: 4 PR campaigns, 80 credibility, and 65 corporate support.\n" +
                "Reward: Popularity +5, Credibility +5, Corporate Support +3."
            );
        }

        if (!grassrootsMovementUnlocked &&
            selectedRoute == "Voter Engagement" &&
            ralliesHeld >= 5 &&
            youthSupport >= 75 &&
            workingClassSupport >= 75)
        {
            grassrootsMovementUnlocked = true;

            popularity += 7;
            youthSupport += 5;
            workingClassSupport += 5;

            ClampStats();
            UpdateUI();

            ShowSecret(
                "Secret Unlocked: Grassroots Movement!\n" +
                "Requirement met: Voter Engagement route, 5 rallies, 75 youth support, and 75 working class support.\n" +
                "Reward: Popularity +7, Youth Support +5, Working Class Support +5."
            );
        }

        if (!corporateBackerUnlocked &&
            selectedRoute == "Fundraising" &&
            corporateSupport >= 80 &&
            campaignFunds >= 2000 &&
            popularity >= 45)
        {
            corporateBackerUnlocked = true;

            campaignFunds += 500;
            corporateSupport += 5;
            popularity -= 3;

            ClampStats();
            UpdateUI();

            ShowSecret(
                "Secret Unlocked: Corporate Backer!\n" +
                "Requirement met: Fundraising route, 80 corporate support, $2000 funds, and at least 45 popularity.\n" +
                "Reward: Funds +$500, Corporate Support +5, Popularity -3."
            );
        }

        if (!viralMomentUnlocked &&
            adsBought >= 4 &&
            ralliesHeld >= 2 &&
            popularity >= 75 &&
            youthSupport >= 70)
        {
            viralMomentUnlocked = true;

            popularity += 6;
            youthSupport += 6;

            ClampStats();
            UpdateUI();

            ShowSecret(
                "Secret Unlocked: Viral Media Moment!\n" +
                "Requirement met: 4 ads, 2 rallies, 75 popularity, and 70 youth support.\n" +
                "Reward: Popularity +6, Youth Support +6."
            );
        }
    }
    private void ShowSecret(string message)
    {
        if (secretText != null)
        {
            secretText.text = message;
        }

        ShowFeedback("A secret campaign opportunity was unlocked!");
    }
}

