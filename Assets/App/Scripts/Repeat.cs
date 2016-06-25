using UnityEngine;
using System.Collections;
using System;

public class Repeat : MonoBehaviour {

    public GameObject unityChan;
    private DateTime startTime;
    private Rigidbody unityChanRigidbody;
    private Vector3 initialPosition;

    // Use this for initialization
    void Start() {
        this.startTime = DateTime.Now;
        this.unityChanRigidbody = this.unityChan.GetComponent<Rigidbody>();
        this.unityChanRigidbody.isKinematic = true;
        this.initialPosition = this.unityChan.transform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        if (this.unityChanRigidbody.isKinematic && ((DateTime.Now - this.startTime) > new TimeSpan(0, 0, 10)))
        {
            this.unityChanRigidbody.isKinematic = false;
        }

        if ((DateTime.Now - this.startTime) > new TimeSpan(0, 4, 0))
        {
            this.unityChan.SetActive(false);
            this.unityChan.transform.localPosition = this.initialPosition;
            this.unityChan.SetActive(true);
            this.startTime = DateTime.Now;
        }
    }
}
