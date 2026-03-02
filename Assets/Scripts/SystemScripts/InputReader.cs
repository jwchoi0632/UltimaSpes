using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public class ActionHandlers
    {
        public Action OnStarted;
        public Action OnPerformed;
        public Action OnCanceled;
    }

    public static InputReader Instance { get; private set; }
    public PlayerInputAction inputActions { get; private set; }

    private Dictionary<InputAction, ActionHandlers> actionCallbacks = new Dictionary<InputAction, ActionHandlers>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitInputReader();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitInputReader()
    {
        inputActions = new PlayerInputAction();

        foreach (var action in inputActions.PlayerActionMap.Get())
        {
            action.started += ctx => Dispatch(ctx.action, "started");
            action.performed += ctx => Dispatch(ctx.action, "performed");
            action.canceled += ctx => Dispatch(ctx.action, "canceled");
        }
    }

    private void Dispatch(InputAction action, string state)
    {
        if (actionCallbacks.TryGetValue(action, out var handlers))
        {
            if (state == "started") handlers.OnStarted?.Invoke();
            else if (state == "performed") handlers.OnPerformed?.Invoke();
            else if (state == "canceled") handlers.OnCanceled?.Invoke();
        }
    }

    public void BindAction(InputAction action, Action started = null, Action performed = null, Action canceled = null)
    {
        if (!actionCallbacks.ContainsKey(action))
            actionCallbacks[action] = new ActionHandlers();

        if (started != null) actionCallbacks[action].OnStarted += started;
        if (performed != null) actionCallbacks[action].OnPerformed += performed;
        if (canceled != null) actionCallbacks[action].OnCanceled += canceled;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    public void OnPlayerInputEnable() => inputActions.PlayerActionMap.Enable();
    public void OnPlayerInputDisable() => inputActions.PlayerActionMap.Disable();
}