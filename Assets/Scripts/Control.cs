using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour, IHoldHandler
{
    public TextMesh guidanceText;
    public GameObject unityChan;
    public SpatialMappingManager spatialMappingManager;
    public Material spatialMappingMaterialWireframe;
    public Material spatialMappingMaterialOcclusion;
    private DateTime HoldStartTime { get; set; }
    private bool IsHolding { get; set; }
    private IList<GameObject> copyActors = new List<GameObject>();
    private bool IsDrawSpatialMappingWireframe { get; set; }

    private const int actionChangeInterval = 1000;

    void Start()
    {
        InputManager.Instance.AddGlobalListener(this.gameObject);

        this.StartCoroutine(this.Process());
        this.StartCoroutine(this.MusicStarter());
    }

    private IEnumerator MusicStarter()
    {
        var audioSource = this.GetComponent<AudioSource>();
        audioSource.Stop();

        yield return new WaitForSeconds(2f);

        audioSource.Play();
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

            if (this.IsHolding)
            {
                var holdSpan = (DateTime.Now - this.HoldStartTime).TotalMilliseconds;
                var actionIndex = ((int)holdSpan / actionChangeInterval);
                Debug.Log("actionIndex=" + actionIndex);
                switch (actionIndex)
                {
                    case 1:
                    case 2:
                    case 3:
                        this.guidanceText.text = "Add Unity-chan.";
                        break;
                    case 4:
                    case 5:
                    case 6:
                        this.guidanceText.text = "Switch Spatial Showing.";
                        break;
                    default:
                        this.guidanceText.text = string.Empty;
                        break;
                }
            }
            else
            {
                this.guidanceText.text = string.Empty;
            }

            yield return 0;
        }
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        this.HoldStartTime = DateTime.Now;
        this.IsHolding = true;
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        this.OnHoldReleased();
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        this.OnHoldReleased();
    }

    private void OnHoldReleased()
    {
        if (this.IsHolding)
        {
            try
            {
                var holdSpan = (DateTime.Now - this.HoldStartTime).TotalMilliseconds;
                var actionIndex = ((int)holdSpan / actionChangeInterval);
                Debug.Log("actionIndex=" + actionIndex);

                switch (actionIndex)
                {
                    case 1:
                    case 2:
                    case 3:
                        var animHash = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
                        var animTime = this.unityChan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
                        var copy = GameObject.Instantiate(this.unityChan);
                        copy.GetComponent<Animator>().Play(animHash, -1, animTime);
                        copy.transform.position = GazeManager.Instance.HitPosition;
                        copy.transform.LookAt(Camera.main.transform);
                        copy.transform.rotation = Quaternion.Euler(0f, copy.transform.rotation.eulerAngles.y, 0f);
                        this.copyActors.Add(copy);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        this.IsDrawSpatialMappingWireframe = !this.IsDrawSpatialMappingWireframe;
                        this.spatialMappingManager.SurfaceMaterial = this.IsDrawSpatialMappingWireframe ? this.spatialMappingMaterialWireframe : this.spatialMappingMaterialOcclusion;
                        break;
                }
            }
            finally
            {
                this.IsHolding = false;
            }
        }
    }
}
