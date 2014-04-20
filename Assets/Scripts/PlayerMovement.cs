using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement {

    public IEnumerable<Vector3> Directions { get { return new List<Vector3>() { Up, Down, Left, Right }; } private set { } }
    public Vector3 Up { get { return new Vector3(0f, 5f, 0f); } private set { } }
    public Vector3 Down { get { return new Vector3(0f, -5f, 0f); } private set { } }
    public Vector3 Left { get { return new Vector3(-5f, 0f, 0f); } private set { } }
    public Vector3 Right { get { return new Vector3(5f, 0f, 0f); } private set { } }
}
