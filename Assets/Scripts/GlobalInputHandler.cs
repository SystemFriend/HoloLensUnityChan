using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class GlobalInputHandler : MonoBehaviour, IHoldHandler, IInputClickHandler
{
    public Control controller;
    private float lastTimeTapped = 0f;
    private float coolDownTime = 1.0f;

    void Start ()
    {
        InputManager.Instance.AddGlobalListener(this.gameObject);
        this.lastTimeTapped = Time.time + this.coolDownTime;
    }

    private void OnHoldCompleted(HoldEventData eventData)
    {
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
    }

    void IHoldHandler.OnHoldCompleted(HoldEventData eventData)
    {
        this.controller.appBar.gameObject.SetActive(true);
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (Time.time < this.lastTimeTapped + this.coolDownTime)
        {
            return;
        }
        this.lastTimeTapped = Time.time;

        if ((eventData.selectedObject != null))
        {
            foreach (var interactive in this.controller.interactables)
            {
                if (interactive.name == eventData.selectedObject.name)
                {
                    return;
                }
            }

            var animHash = this.controller.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
            var animTime = this.controller.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            var copy = GameObject.Instantiate(this.controller.unityChan);
            copy.GetComponent<Animator>().Play(animHash, -1, animTime);
            copy.transform.position = GazeManager.Instance.HitPosition;
            copy.transform.LookAt(Camera.main.transform);
            copy.transform.rotation = Quaternion.Euler(0f, copy.transform.rotation.eulerAngles.y, 0f);
            this.controller.copyActors.Add(copy);
        }
    }
}
