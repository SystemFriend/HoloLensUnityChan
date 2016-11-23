using System;
using System.Collections;
using UnityEngine;

public class Control : MonoBehaviour {

    public GameObject unityChan;

    void Start()
    {
        this.StartCoroutine(this.Process());
    }

    private IEnumerator Process()
    {
        var startTime = DateTime.Now;
        var unityChanRigidbody = this.unityChan.GetComponent<Rigidbody>();
        var initialPosition = this.unityChan.transform.localPosition;

        unityChanRigidbody.isKinematic = true;
        while (true)
        {
            if (unityChanRigidbody.isKinematic && ((DateTime.Now - startTime) > new TimeSpan(0, 0, 10)))
            {
                //10秒後にisKinematicを開放して重力落下を開始させる
                unityChanRigidbody.isKinematic = false;
            }

            if ((DateTime.Now - startTime) > new TimeSpan(0, 4, 0))
            {
                //４分たったら初期状態からダンス開始
                this.unityChan.SetActive(false);
                this.unityChan.transform.localPosition = initialPosition;
                this.unityChan.SetActive(true);
                startTime = DateTime.Now;
            }
            yield return 0;
        }
    }
}
