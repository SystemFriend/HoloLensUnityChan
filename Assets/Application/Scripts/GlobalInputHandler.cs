using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInputHandler : MonoBehaviour, IMixedRealityGestureHandler
{
    public UnityEvent globalTapped;
    public UnityEvent globalHold;

    private void OnEnable()
    {
        CoreServices.InputSystem?.PushFallbackInputHandler(this.gameObject);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.PopFallbackInputHandler();
    }

    public void OnGestureStarted(InputEventData eventData)
    {
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        var action = eventData.MixedRealityInputAction.Description;
        Debug.Log($"action={action}");
        if (action == "Hold Action")
        {
            this.globalHold?.Invoke();
        }
        else if (action == "Select")
        {
            this.globalTapped?.Invoke();
        }
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
    }
}
