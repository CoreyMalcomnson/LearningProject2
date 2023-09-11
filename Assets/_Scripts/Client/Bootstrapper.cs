using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    enum State
    {
        IDLE,

        INITIALIZING_UNITY_SERVICES,
        SUCCESSFULLY_INITIALIZED_UNITY_SERVICES,
        FAILED_TO_INITIALIZE_UNITY_SERVICES,

        SIGNING_IN_ANONYMOUSLY,
        SUCCESSFULLY_SIGNED_IN,
        FAILED_TO_SIGN_IN,

        INITIALIZING_MATCHMAKING_SERVICE,
        SUCCESSFULLY_INITIALIZED_MATCHMAKING_SERVICE,
        FAILED_TO_INITIALIZE_MATCHMAKING_SERVICE,

        STARTING_GAME,
        BLOCKED
    }

    private State _currentState = State.IDLE;

    private void Awake()
    {
        SwitchState(State.INITIALIZING_UNITY_SERVICES);
    }

    private void Update()
    {
        // Update state
        switch (_currentState)
        {
            case State.SUCCESSFULLY_INITIALIZED_UNITY_SERVICES:
                SwitchState(State.SIGNING_IN_ANONYMOUSLY);
                break;
            case State.SUCCESSFULLY_SIGNED_IN:
                SwitchState(State.INITIALIZING_MATCHMAKING_SERVICE);
                break;
            case State.SUCCESSFULLY_INITIALIZED_MATCHMAKING_SERVICE:
                SwitchState(State.STARTING_GAME);
                break;
            case State.FAILED_TO_INITIALIZE_MATCHMAKING_SERVICE:
            case State.FAILED_TO_SIGN_IN:
            case State.FAILED_TO_INITIALIZE_UNITY_SERVICES:
                SwitchState(State.BLOCKED);
                break;
        }
    }

    private void SwitchState(State newState)
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    Debug.Log(newState.ToString());
#endif
        _currentState = newState;

        // Enter state
        switch (_currentState)
        {
            case State.INITIALIZING_UNITY_SERVICES:
                IntializeUnityServices();
                break;
            case State.SIGNING_IN_ANONYMOUSLY:
                SignInAnonymously();
                break;
            case State.INITIALIZING_MATCHMAKING_SERVICE:
                InitializeMatchmakingService();
                break;
            case State.STARTING_GAME:
                StartGame();
                break;
        }
    }

    private void InitializeMatchmakingService()
    {
        try
        {
            MatchmakingService.Initialize();
            SwitchState(State.SUCCESSFULLY_INITIALIZED_MATCHMAKING_SERVICE);
        } catch (Exception e)
        {
            Debug.LogException(e);
            SwitchState(State.FAILED_TO_INITIALIZE_MATCHMAKING_SERVICE);
        }
    }

    private async void IntializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            SwitchState(State.SUCCESSFULLY_INITIALIZED_UNITY_SERVICES);
        } catch(Exception e)
        {
            Debug.LogException(e);
            SwitchState(State.FAILED_TO_INITIALIZE_UNITY_SERVICES);
        }
    }

    private async void SignInAnonymously()
    {
        try
        {
            if(!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            SwitchState(State.SUCCESSFULLY_SIGNED_IN);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            SwitchState(State.FAILED_TO_SIGN_IN);
        }
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
