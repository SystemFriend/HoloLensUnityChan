using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    public GameObject unityChan;
    public HoloToolkit.Unity.InputModule.Cursor cursor;
    public int actionChangeInterval = 1000;

    void Start()
    {
        this.StartCoroutine(this.Process());
    }

    private IEnumerator Process()
    {
        var startTime = DateTime.Now;
        var unityChanRigidbody = this.unityChan.GetComponent<Rigidbody>();
        var initialPosition = this.unityChan.transform.localPosition;

        var prevCursorState = this.cursor.CheckCursorState();
        var selectStart = DateTime.MinValue;
        var isDragging = false;

        var copies = new List<GameObject>();


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

                foreach (var copy in copies)
                {
                    GameObject.Destroy(copy);
                }
                copies.Clear();

                startTime = DateTime.Now;
            }

            if  (Vector3.Distance(this.unityChan.transform.position, Vector3.zero) > 20.0f)
            {
                //Unityちゃん落下消失対策
                this.unityChan.transform.localPosition = initialPosition;
            }



            var dragSpan = (DateTime.Now - selectStart).TotalMilliseconds;
            var actionIndex = ((int)dragSpan / this.actionChangeInterval);

            var cursorState = this.cursor.CheckCursorState();
            if (prevCursorState != cursorState)
            {
                Debug.Log(cursorState + " " + ((GazeManager.Instance.HitObject != null) ? GazeManager.Instance.HitObject.name : ""));
                if (cursorState == HoloToolkit.Unity.InputModule.Cursor.CursorStateEnum.Select)
                {
                    if ((GazeManager.Instance.HitObject == null) ||
                        ((GazeManager.Instance.HitObject != null) && (GazeManager.Instance.HitObject.name.StartsWith("Surface-"))))
                    {
                        selectStart = DateTime.Now;
                        isDragging = true;
                    }
                }
                else if (isDragging)
                {
                    switch (actionIndex)
                    {
                        case 0:
                            // nothing to do.
                            break;
                        case 1:
                        case 2:
                            var animHash = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
                            var animTime = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
                            var copy = GameObject.Instantiate(this.unityChan);
                            copy.GetComponent<Animator>().Play(animHash, -1, animTime);
                            copy.transform.position = this.cursor.Position;
                            copy.transform.LookAt(Camera.main.transform);
                            copy.transform.rotation = Quaternion.Euler(0f, copy.transform.rotation.eulerAngles.y, 0f);
                            copies.Add(copy);
                            break;
                    }
                    isDragging = false;
                }
            }

            prevCursorState = cursorState;
            yield return 0;
        }
    }
}
