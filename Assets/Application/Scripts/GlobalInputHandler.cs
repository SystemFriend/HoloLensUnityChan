using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInputHandler : MonoBehaviour, IMixedRealityPointerHandler
{
    public UnityEvent globalTapped;
    private float lastTimeTapped = 0f;
    private float coolDownTime = 1.0f;

    void Start()
    {
        this.lastTimeTapped = Time.time + this.coolDownTime;
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.PushFallbackInputHandler(this.gameObject);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.PopFallbackInputHandler();
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
    }

    public void OnSourceLost(SourceStateEventData eventData)
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
        this.globalTapped?.Invoke();
    }


    //private void OnHoldCompleted(HoldEventData eventData)
    //{
    //}

    //public void OnHoldStarted(HoldEventData eventData)
    //{
    //}

    //void IHoldHandler.OnHoldCompleted(HoldEventData eventData)
    //{
    //    this.controller.appBar.gameObject.SetActive(true);
    //}

    //public void OnHoldCanceled(HoldEventData eventData)
    //{
    //}
}
