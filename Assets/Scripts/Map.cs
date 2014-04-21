using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
[Serializable]
public class Map : MonoBehaviour {
    public Transform[][] tileGrid;
    public int stuff;
	// Use this for initialization
	void Start () {
        var things = tileGrid;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setTileGrid(Transform[][] grid)
    {
        this.tileGrid = grid;
    }
    
}
