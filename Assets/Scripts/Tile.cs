using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Tile : MonoBehaviour {
    public static bool tileHasPath = true;
    Vector3 curPosition;
    public Transform plane;
    public Material mat;
    public Material freeMat;
    RaycastHit tileHit;
    PlayerMovement move = new PlayerMovement();
    bool playerHit;
    public void taken()
    {
        collider.tag = "Taken";
        plane.renderer.material = mat;
    }
    public void clear()
    {
        collider.tag = "Free";
        plane.renderer.material = freeMat;
    }


    public bool isValidMove(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin , direction );
        
        Debug.DrawRay(origin, direction, Color.red, 1f);
        if (Physics.Raycast(ray, out tileHit, 6f))
        {
            if (tileHit.collider.tag == "Free")
            {
                return true;
            }
            if (tileHit.collider.tag == "Player")
            {
                playerHit = true;
                return true;
            }
        }

        return false;
    }
    public bool checkIfPath(int depth)
    {
        
        Path p = new Path();
        int counter = 0;
        p.freePaths = freeMoves(plane.transform.position, 0, new Vector3());
        var paths = p.freePaths;
        if (p.freePaths.Count > 0)
        {
            List<Path> newMoves = paths;
            while (counter < depth)
            {
                newMoves = newMoves.SelectMany(i => freeMoves(i.currentPosition + i.dirMove, i.pathDepth, i.dirMove)).ToList();
                counter++;
                if (playerHit)
                {
                    playerHit = false;
                    return true;
                }
                    
            }
            if (newMoves.Count == 0 || newMoves.Any(i => i.pathDepth < depth))
            {
                playerHit = false;
                return false;
            }
            else
            {
                playerHit = false;
                return true;
            }
        }
        else
        {
            playerHit = false;
            return false;
        }
    }
    public List<Path> freeMoves(Vector3 currentPos, int depth, Vector3 lastDirection)
    {
        var directions = move.Directions.ToList();
        directions.Remove(-lastDirection);
        var paths = directions.Where(i => isValidMove(currentPos, i)).Select(i => new Path() { currentPosition = currentPos, dirMove = i, pathDepth = depth + 1 }).ToList();
        return paths;
    }
}
