using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingGui : MonoBehaviour
{
    [SerializeField] private GameObject inputPanel;

    [SerializeField] private TMP_Text infoText;

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField serverCodeInput;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button quitButton;

    private async void Awake()
    {
        ConnectButtons();

        inputPanel.SetActive(false);

        await WaitForAuthentication();

        inputPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        DisconnectButtons();
    }

    private void ConnectButtons()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void DisconnectButtons()
    {
        hostButton.onClick.RemoveListener(OnHostButtonClicked);
        joinButton.onClick.RemoveListener(OnJoinButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private async Task WaitForAuthentication()
    {
        int dotCount = 0;
        while (!MatchmakingManager.Initialized)
        {
            infoText.text = "Waiting for Authentication" + string.Concat(Enumerable.Repeat(".", dotCount));
            await Task.Delay(500);

            dotCount++;
            dotCount %= 4;
        }

        infoText.text = "Authenticated";
    }

    private async void OnHostButtonClicked()
    {
        if (!ValidateUsernameInput())
            return;

        infoText.text = "Hosting Server";
        bool success = await MatchmakingManager.TryHostServer();
        infoText.text = success ? "Successfully hosted server" : "Failed to host server";
    }

    private async void OnJoinButtonClicked()
    {
        if (!ValidateUsernameInput())
            return;

        if (!ValidateServerCodeInput())
            return;

        infoText.text = "Joining Server";
        bool success = await MatchmakingManager.TryJoinServer(serverCodeInput.text);
        infoText.text = success ? "Successfully joined server" : "Failed to join server";
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool ValidateUsernameInput()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            infoText.text = "Invalid username {username cannot be empty}";
            return false;
        }

        return true;
    }

    private bool ValidateServerCodeInput()
    {
        if (string.IsNullOrEmpty(serverCodeInput.text))
        {
            infoText.text = "Invalid server code {server code cannot be empty}";
            return false;
        }

        return true;
    }
}
