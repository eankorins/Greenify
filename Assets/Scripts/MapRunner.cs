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
    public Transform[][] tileGrid;
    public int stepTurnCount, depthCount;
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
        getGrid();
    }
    void getGrid()
    {
        var t = map.GetComponentsInChildren<Transform>();
        var initials = t.Select(i => i.name.Substring(0, i.name.IndexOf(',') + 1)).Distinct().ToList();
        var columns = initials.Select(i => t.Where(x => i == x.name.Substring(0, x.name.IndexOf(',') + 1)).ToList()).ToList();
        columns.Remove(columns[0]);
        tileGrid = columns.Select(i=>i.ToArray()).ToArray();    
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
                while(counter < stepTurnCount)
                {
                    int depth = (int)(OpenTiles.Count / filledCount) * depthCount + 1;
                    bool check = OpenTiles.Where(i => i.tag != "Taken").Select(i => i.GetComponent<Tile>().checkIfPath(depth, false)).Any(i=> i == false);
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
                        Debug.Log("Runner has Path: " + temp.hasPath() + " Path Check: " + !check + " Sweep Right: " + sweepForWallRight() + " Sweep Left: " +  sweepForWallLeft());
                        reverseRunner(-temp.dirMove);
                        filledCount++;
                        temp = temp.Prev;
                    }
                    else
                    {
                        Debug.Log("Runner has Path: " + temp.hasPath() + " Path Check: " + !check + " Sweep Right: " + sweepForWallRight() + " Sweep Left: " + sweepForWallLeft());
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
            if (tileHit.collider.tag == "Player")
            {  
                return true;
            }
        }

        return false;
    }

    public bool sweepForWallRight()
    {
        bool playerHit = false;
        int c = 0;
        while (c < tileGrid[0].Length)
        {
            var tiles = tileGrid[c].Where(i=>i.tag =="Free").ToList();
                List<Transform> tempTiles = new List<Transform>();
                int obstacleCount = 0;
                foreach (var item in tiles)
                {
                    if (item.position.x < runner.position.x)
                    {
                        Ray ray = new Ray(item.position, move.Right);
                        RaycastHit tileHit;
                        Debug.DrawRay(item.position, move.Right, Color.black, 2f);
                        if (Physics.Raycast(ray, out tileHit, 6f))
                        {
                            if (tileHit.collider.tag == "Free")
                            {
                                obstacleCount--;
                            }
                            if (tileHit.collider.tag == "Player")
                            {
                                playerHit = true;
                            }
                            else
                                obstacleCount++;
                        }
                    }
                    else
                    {
                        playerHit = false;
                        return true;
                    }
                }
                if (playerHit)
                {
                    playerHit = false;
                    return true;
                }
                if (obstacleCount >= tiles.Count() && !playerHit)
                {
                    playerHit = false;
                    return false;
                }
            
            c++;
        }
        playerHit = false;
        return true;
       
    }
    public bool sweepForWallLeft()
    {
        bool playerHit = false;
        int c = 0;
        while (c < tileGrid[0].Length)
        {
            var tiles = tileGrid[tileGrid.Length - (c + 1)].Where(i => i.tag == "Free").ToList();
                List<Transform> tempTiles = new List<Transform>();
                int obstacleCount = 0;
                foreach (var item in tiles)
                {
                    if (item.position.x > runner.position.x)
                    {
                        Ray ray = new Ray(item.position, move.Left);
                        RaycastHit tileHit;
                        Debug.DrawRay(item.position, move.Left, Color.red, 2f);
                        if (Physics.Raycast(ray, out tileHit, 6f))
                        {
                            if (tileHit.collider.tag == "Free")
                            {
                                obstacleCount--;
                            }
                            if (tileHit.collider.tag == "Player")
                            {
                                playerHit = true;
                            }
                            else
                                obstacleCount++;
                        }
                    }
                    else
                    {
                        playerHit = false;
                        return true;
                    }

                }
                if (playerHit)
                {
                    playerHit = false;
                    return true;
                }
                if (obstacleCount >= tiles.Count() && !playerHit)
                {
                    playerHit = false;
                    return false;
                }
            c++;

        }
        playerHit = false;
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
