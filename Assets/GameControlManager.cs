using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameControlManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference restartAction;
    public InputActionReference quitAction;

    [Header("Settings")]
    public float cooldownTime = 0.75f;

    private bool restartWasPressed = false;
    private bool quitWasPressed = false;
    private float nextAllowedInputTime = 0f;

    private void Start()
    {
        EnableActions();
    }

    private void OnEnable()
    {
        EnableActions();
    }

    private void Update()
    {
        EnableActions();

        if (Time.time < nextAllowedInputTime)
        {
            return;
        }

        bool restartPressed = IsActionPressed(restartAction);
        bool quitPressed = IsActionPressed(quitAction);

        if (restartPressed && !restartWasPressed)
        {
            nextAllowedInputTime = Time.time + cooldownTime;
            RestartGame();
        }

        if (quitPressed && !quitWasPressed)
        {
            nextAllowedInputTime = Time.time + cooldownTime;
            QuitGame();
        }

        restartWasPressed = restartPressed;
        quitWasPressed = quitPressed;
    }

    private void EnableActions()
    {
        if (restartAction != null && restartAction.action != null && !restartAction.action.enabled)
        {
            restartAction.action.Enable();
        }

        if (quitAction != null && quitAction.action != null && !quitAction.action.enabled)
        {
            quitAction.action.Enable();
        }
    }

    private bool IsActionPressed(InputActionReference actionReference)
    {
        if (actionReference == null || actionReference.action == null)
        {
            return false;
        }

        float value = actionReference.action.ReadValue<float>();
        return value > 0.5f;
    }

    public void RestartGame()
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
}