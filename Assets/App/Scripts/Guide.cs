using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guide : MonoBehaviour {

    public GameObject dotPrefab;
    public bool isActive = true;
    public float dotInterval = 1.0f;
    public Vector3 areaSize = new Vector3(8f, 8f, 8f);

    private IList<GameObject> dots;

    // Use this for initialization
    void Start () {
        this.dots = new List<GameObject>();
        GameObject dotRoot = new GameObject();
        for (float z = 0; z < this.areaSize.z; z += this.dotInterval)
        {
            for (float y = 0; y < this.areaSize.y; y += this.dotInterval)
            {
                for (float x = 0; x < (this.areaSize.x / 2); x += this.dotInterval)
                {
                    this.dots.Add(GameObject.Instantiate(this.dotPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject);
                    this.dots.Add(GameObject.Instantiate(this.dotPrefab, new Vector3(-x, y, z), Quaternion.identity) as GameObject);
                }
            }
        }
        foreach (var dot in this.dots)
        {
            dot.transform.parent = dotRoot.transform;
        }
        dotRoot.transform.parent = this.transform;

    }

    // Update is called once per frame
    void Update () {
	
	}
}
