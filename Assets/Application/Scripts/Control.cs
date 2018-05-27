using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : InteractionReceiver
{
    public TextMesh guidanceText;
    public GameObject unityChan;
    public SpatialMappingManager spatialMappingManager;
    public Material spatialMappingMaterialWireframe;
    public Material spatialMappingMaterialOcclusion;
    public AudioSource audioSource;
    public GameObject floor;
    public AppBar appBar;
    public IList<GameObject> copyActors = new List<GameObject>();
    private bool IsDrawSpatialMappingWireframe { get; set; }
    private float lastTimeTapped = 0f;
    private float coolDownTime = 0.5f;

    void Start()
    {
        this.StartCoroutine(this.Process());
        this.StartCoroutine(this.MusicStarter());

        this.floor.SetActive(MixedRealityCameraManager.Instance.CurrentDisplayType == MixedRealityCameraManager.DisplayType.Opaque);
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
        CameraCache.Main.nearClipPlane = 0.01f;
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

    protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
    {
        if (Time.time < this.lastTimeTapped + this.coolDownTime)
        {
            return;
        }
        this.lastTimeTapped = Time.time;

        switch (obj.name)
        {
            case "Close":
                this.appBar.gameObject.SetActive(false);
                break;
            case "Mapping":
                this.IsDrawSpatialMappingWireframe = !this.IsDrawSpatialMappingWireframe;
                this.spatialMappingManager.SurfaceMaterial = this.IsDrawSpatialMappingWireframe ? this.spatialMappingMaterialWireframe : this.spatialMappingMaterialOcclusion;
                break;
            case "BGM":
                this.audioSource.volume = (audioSource.volume > 0f) ? 0f : 0.5f;
                break;
            default:
                base.InputClicked(obj, eventData);
                break;
        }
    }
}
