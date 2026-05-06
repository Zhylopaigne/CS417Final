using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    [System.Serializable]
    public class ScenarioChoice
    {
        public string choiceText;
        public int fundsChange;
        public int popularityChange;
        public int credibilityChange;
        public string resultText;
    }

    [System.Serializable]
    public class Scenario
    {
        public string title;
        [TextArea] public string context;
        public ScenarioChoice[] choices;
    }

    [Header("Managers")]
    public ResourceManager resourceManager;
    public CampaignTimelineManager timelineManager;

    [Header("UI")]
    public GameObject scenarioPanel;
    public TMP_Text titleText;
    public TMP_Text contextText;
    public TMP_Text resultText;
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonTexts;

    [Header("Scenarios")]
    public Scenario[] scenarios;

    private Scenario currentScenario;

    private void Start()
    {
        if (scenarioPanel != null)
            scenarioPanel.SetActive(false);
    }

    public void ShowRandomScenario()
    {
        if (scenarios == null || scenarios.Length == 0)
            return;

        currentScenario = scenarios[Random.Range(0, scenarios.Length)];

        scenarioPanel.SetActive(true);
        titleText.text = currentScenario.title;
        contextText.text = currentScenario.context;
        resultText.text = "";

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;

            if (i < currentScenario.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtonTexts[i].text = currentScenario.choices[i].choiceText;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseOption(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChooseOption(int index)
    {
        ScenarioChoice choice = currentScenario.choices[index];

        if (choice.fundsChange > 0)
            resourceManager.GainFunds(choice.fundsChange);
        else if (choice.fundsChange < 0)
            resourceManager.LoseFunds(Mathf.Abs(choice.fundsChange));

        if (choice.popularityChange > 0)
            resourceManager.IncreasePopularity(choice.popularityChange);
        else if (choice.popularityChange < 0)
            resourceManager.DecreasePopularity(Mathf.Abs(choice.popularityChange));

        if (choice.credibilityChange > 0)
            resourceManager.IncreaseCredibility(choice.credibilityChange);
        else if (choice.credibilityChange < 0)
            resourceManager.DecreaseCredibility(Mathf.Abs(choice.credibilityChange));

        resultText.text = choice.resultText;

        foreach (Button button in choiceButtons)
            button.interactable = false;

        Invoke(nameof(CloseScenario), 2.5f);
    }

    private void CloseScenario()
    {
        scenarioPanel.SetActive(false);

        foreach (Button button in choiceButtons)
            button.interactable = true;

        if (timelineManager != null)
            timelineManager.AdvanceDay();
    }
}