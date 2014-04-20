using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class CreatorWindow : EditorWindow {

	bool isPressed;
    int gridWidth;
    int gridHeight;
	int tileCount;
    public Object source;
	public Object map;
	public Object obstacle;
    public List<Object> tiles;
    List<List<Transform> tileGrid;
    GameObject grid;
	bool doRemove = false;
	[MenuItem("Window/Map Editor")]
    public static void ShowWindow()
    {
        CreatorWindow window = (CreatorWindow)EditorWindow.GetWindow(typeof(CreatorWindow));
        window.Show();
    }
    void OnGUI() {
		EditorGUILayout.BeginVertical ();
        gridWidth = EditorGUILayout.IntField("Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Height", gridHeight);
        source = EditorGUILayout.ObjectField("Build Tile", source, typeof(Object), true);
        map = EditorGUILayout.ObjectField("Standard Map", map, typeof(Object), true);
		EditorGUILayout.EndVertical ();

        if (GUILayout.Button("Search!"))
        {
			isPressed = true;
			if(isPressed)
			{
                
				grid = Instantiate(map, new Vector3(), Quaternion.identity) as GameObject;
				grid.name = "Map";
				grid.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tileGrid = new List<List<Transform>>();
				for (int w = 0; w < gridWidth; w++)
	            {
                    var tempList = new List<Transform>();
	                for (int h = 0; h < gridHeight; h++)
	                {
						GameObject tempTile = Instantiate(source, new Vector3(w * 5, h*5, 0), Quaternion.identity) as GameObject;
                        tempTile.name = w.ToString() +", " + h.ToString() + " - " + tempTile.name;
						tempTile.transform.localEulerAngles = new Vector3(-90, 0, 0);
						tempTile.transform.parent = grid.transform;
                        tempList.Add(tempTile.transform);
	                }
                    tileGrid.Add(tempList);
	            }
                var m = (GameObject)grid;
                m.GetComponent<Map>().tileGrid = tileGrid;
				isPressed = false;
			}

		}
		TileList ();
		BtnList();
	}
    void TileList()
    {
	    EditorGUILayout.BeginVertical();
	    tileCount = EditorGUILayout.IntField("Tile Count", tileCount);
		if( tiles == null)
		{
			tiles = new List<Object>();

		}
		if(tiles.Count == 0)
			tiles.Add(new Object());
			if(tileCount >= tiles.Count)
				doRemove = false;
			else
				doRemove = true;

			if(doRemove)
			{
				for(int i = tiles.Count - 1; i > tileCount; i--)
				{
					Debug.Log("Removed Tile");
					tiles.Remove(tiles[i]);
				}
			}
			else
			{
				for(int i = tiles.Count - 1; i < tileCount; i++)
				{
					Debug.Log("Added Tile");
					tiles.Add(new Object());
				}
			}
	    for (int i = 0; i < tileCount; i++)
	    {
			tiles[i] = EditorGUILayout.ObjectField("Tile " + i.ToString(), tiles[i], typeof(Object), true);
	    }
	    EditorGUILayout.EndVertical();
    }
    void BtnList()
    {
	    EditorGUILayout.BeginVertical();
	    foreach (var item in tiles)
	    {
				if(item != null)
				{
	                if (GUILayout.Button(item.name))
	                {
						var selections = Selection.gameObjects;
						foreach(var tile in selections)
						{
							string tempName = tile.name.Substring(0, tile.name.IndexOf("-") + 2);
							var newItem = Instantiate(item) as GameObject;
							newItem.transform.position = tile.transform.position;
							newItem.transform.parent = tile.transform.parent;
							newItem.name = tempName + newItem.name;
                            if (newItem.name.Contains("MapRunner"))
                            {
                                var runner = newItem.GetComponentsInChildren<Transform>();
                                var m = runner[1].GetComponent<MapRunner>();
                                m.map = grid;
                                m.tileGrid = grid.GetComponent<Map>().tileGrid;
                                
                            }
							DestroyImmediate(tile);
						}
	                }
				}
	    }
	    EditorGUILayout.EndVertical();
    }
}
