using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInputHandler : MonoBehaviour, IMixedRealityGestureHandler, IMixedRealityPointerHandler
{
    [System.Serializable]
    public class PointerClickedEvent : UnityEvent<IMixedRealityPointer> { }
    public PointerClickedEvent globalPointerClicked;
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
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        var result = eventData.Pointer.Result;
        if (result.CurrentPointerTarget?.GetComponent<Interactable>() == null)
        {
            this.globalPointerClicked?.Invoke(eventData.Pointer);
        }
    }
}
