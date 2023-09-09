using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenuGui : MonoBehaviour
{
    [SerializeField] private Button leaveButton;

    public object OnHostButtonClicked { get; private set; }

    private void Awake()
    {
        ConnectButtons();
    }

    private void OnDestroy()
    {
        DisconnectButtons();
    }

    private void ConnectButtons()
    {
        leaveButton.onClick.AddListener(OnLeaveButtonClicked);
    }

    private void DisconnectButtons()
    {
        leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
    }

    private void OnLeaveButtonClicked()
    {
        MatchmakingService.LeaveServer();
    }
}
