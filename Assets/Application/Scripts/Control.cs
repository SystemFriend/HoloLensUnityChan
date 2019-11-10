using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public TextMesh guidanceText;
    public GameObject unityChan;
    //public SpatialMappingManager spatialMappingManager;
    public Material spatialMappingMaterialWireframe;
    public Material spatialMappingMaterialOcclusion;
    public AudioSource audioSource;
    ////public AppBar appBar;
    private IList<GameObject> copyActors = new List<GameObject>();
    private bool IsDrawSpatialMappingWireframe { get; set; }
    private float lastTimeTapped = 0f;
    private float coolDownTime = 0.5f;

    void Start()
    {
        this.StartCoroutine(this.Process());
        this.StartCoroutine(this.MusicStarter());

        this.lastTimeTapped = Time.time + this.coolDownTime;
    }

    private IEnumerator MusicStarter()
    {
        this.audioSource.Stop();

        yield return new WaitForSeconds(2f);

        this.audioSource.Play();
    }

    private IEnumerator Process()
    {
        var startTime = DateTime.Now;
        var unityChanRigidbody = this.unityChan.GetComponent<Rigidbody>();
        var initialPosition = this.unityChan.transform.localPosition;
        this.copyActors = new List<GameObject>();

        unityChanRigidbody.isKinematic = true;
        while (true)
        {
            if (unityChanRigidbody.isKinematic && ((DateTime.Now - startTime) > new TimeSpan(0, 0, 10)))
            {
                //10秒後にisKinematicを開放して重力落下を開始させる
                unityChanRigidbody.isKinematic = false;
            }
            else if (unityChanRigidbody.isKinematic)
            {
                yield return 0;
                continue;
            }

            if ((DateTime.Now - startTime) > new TimeSpan(0, 4, 0))
            {
                //４分たったら初期状態からダンス開始
                this.unityChan.SetActive(false);
                this.unityChan.transform.localPosition = initialPosition;
                this.unityChan.SetActive(true);

                foreach (var copy in this.copyActors)
                {
                    GameObject.Destroy(copy);
                }
                this.copyActors.Clear();

                startTime = DateTime.Now;
                this.StartCoroutine(this.MusicStarter());
            }

            if  (Vector3.Distance(this.unityChan.transform.position, Vector3.zero) > 20.0f)
            {
                //Unityちゃん落下消失対策
                this.unityChan.transform.localPosition = initialPosition;
            }
            this.guidanceText.text = string.Empty;

            yield return 0;
        }
    }

    public void SummonNewAvator()
    {
        var animHash = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
        var animTime = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
        var copy = GameObject.Instantiate(this.unityChan);
        copy.GetComponent<Animator>().Play(animHash, -1, animTime);

        var position = CoreServices.InputSystem.GazeProvider.HitPosition;
        if (position == Vector3.zero)
        {
            position = Camera.main.transform.TransformDirection(Camera.main.transform.localPosition + new Vector3(0, 0, 1.5f));
        }
        copy.transform.position = position;
        copy.transform.LookAt(Camera.main.transform);
        copy.transform.rotation = Quaternion.Euler(0f, copy.transform.rotation.eulerAngles.y, 0f);
        this.copyActors.Add(copy);
    }

    //protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
    //{
    //    if (Time.time < this.lastTimeTapped + this.coolDownTime)
    //    {
    //        return;
    //    }
    //    this.lastTimeTapped = Time.time;

    //    switch (obj.name)
    //    {
    //        case "Close":
    //            this.appBar.gameObject.SetActive(false);
    //            break;
    //        case "Mapping":
    //            this.IsDrawSpatialMappingWireframe = !this.IsDrawSpatialMappingWireframe;
    //            this.spatialMappingManager.SurfaceMaterial = this.IsDrawSpatialMappingWireframe ? this.spatialMappingMaterialWireframe : this.spatialMappingMaterialOcclusion;
    //            break;
    //        case "BGM":
    //            this.audioSource.volume = (audioSource.volume > 0f) ? 0f : 0.5f;
    //            break;
    //        default:
    //            base.InputClicked(obj, eventData);
    //            break;
    //    }
    //}
}
