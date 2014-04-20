using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
public class MapRunner : MonoBehaviour
{
    public Transform runner;
    public GameObject map;
    public GameObject currentTile;
    public float waitTime;
    public List<List<Transform>> tileGrid;
    bool nextMove;
    Path firstMove;
    Path temp;
    PlayerMovement move;
    List<Vector3> movements;
    List<Transform> OpenTiles;
    
    int counter = 0;
    int stepCount = 0;
    int filledCount = 0;

    float nextMoveTime;
    public void Awake()
    {
        move = new PlayerMovement();
        movements = new List<Vector3>() { move.Up, move.Down, move.Right, move.Left };
        OpenTiles = map.GetComponentsInChildren<Transform>().Where(i => i.name.Contains("PathTile")).ToList();
        filledCount = OpenTiles.Count;
        Path firstMove = new Path();
        temp = firstMove;
        var validMoves = movements.Where(i => isValidMove(gameObject.transform.position, i) == true).ToList();
        firstMove.freePaths = validMoves.Select(i => new Path() { dirMove = i, currentPosition = runner.position }).Where(i => i != null).ToList();
        firstMove.pathTried = firstMove.freePaths.Select(i => false).ToList();
    }
    void Update()
    {
        if(!nextMove)
        { 
            if (nextMoveTime < waitTime)
            {
                nextMoveTime += Time.deltaTime;
            }
            else
            {
                nextMove = true;
                nextMoveTime = 0;
            }
        }
        else
        { 
            if (filledCount > 0)
            {
                int counter = 0;
                while(counter < 1)
                {
                    int depth = (int)(OpenTiles.Count / filledCount);
                    bool check = OpenTiles.Where(i => i.tag != "Taken").Select(i => i.GetComponent<Tile>().checkIfPath(depth)).Any(i=> i == false);
                    if (temp.hasPath() && !check)
                    {
                        Path curMove = new Path();
                        curMove = temp.nextMove();
                        moveRunner(curMove.dirMove);
                        stepCount++;
                        filledCount--;
                        var validMoves = movements.Where(i => isValidMove(runner.position, i) == true).ToList();
                        curMove.freePaths = validMoves.Select(i => new Path() { dirMove = i, currentPosition = runner.position }).Where(i => i != null).ToList();
                        curMove.pathTried = curMove.freePaths.Select(j => false).ToList();
                        temp = curMove;
                        nextMove = false;

                    }
                    else if (temp.Prev != null)
                    {
                        reverseRunner(-temp.dirMove);
                        filledCount++;
                        temp = temp.Prev;
                    }
                    else
                    {
                        Debug.Log("Unsolvable");
                    }
                    counter++;
                }
            }
            else{Debug.Log("Solved");} 
        }
       
    }
    public void reverseRunner(Vector3 dir)
    {

        Ray ray = new Ray(runner.position, dir);
        RaycastHit tileHit;
        if (Physics.Raycast(ray, out tileHit, 6f))
        {
            currentTile.transform.gameObject.SendMessage("clear");
        }
        currentTile = tileHit.transform.gameObject;
        runner.Translate(dir);
    }
    public void moveRunner(Vector3 dir)
    {
        Ray ray = new Ray(runner.position, dir);
        RaycastHit tileHit;
        if (Physics.Raycast(ray, out tileHit, 6f))
        {
            if(tileHit.collider.tag != "Start")
                tileHit.transform.gameObject.SendMessage("taken");
            currentTile = tileHit.transform.gameObject;
        }
        runner.Translate(dir);
    }
    public bool isValidMove(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit tileHit;
        if (Physics.Raycast(ray, out tileHit, 6f))
        {
            if (tileHit.collider.tag == "Free")
            {
                return true;
            }
        }

        return false;
    }

    public bool sweepForWall()
    {
        for (int width = 0; width < tileGrid.Count; width++)
        {
            for (int height = 0; height < tileGrid[width].Count; height++)
            {
                bool moveRight = true;
                while (moveRight)
                {
                    moveRight = isValidMove(tileGrid[width][height].position)
                }
            }
        }
        return true;
    }
}
public class Path
{
    public Path Prev { get; set; }
    public Vector3 dirMove { get; set; }
    public List<Path> freePaths { get; set; }
    public List<bool> pathTried { get; set; }
    public Vector3 currentPosition { get; set; }
    public int pathDepth { get; set; }
    public bool hasPath()
    {
        if (freePaths.Any() && pathTried.Any(i => i == false))
        {
			return true;	
        }
        return false;
    }
    public Path nextMove()
    {
        var index = pathTried.IndexOf(false);
        var nextMove = freePaths[index];
        pathTried[index] = true;
        nextMove.Prev = this;
        return nextMove;
    }
}
