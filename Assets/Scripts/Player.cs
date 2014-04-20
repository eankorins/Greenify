using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Transform currentTile;
    public Transform player;
    RaycastHit tileHit;
    private PlayerMovement move;

    void Update()
    {
        move = new PlayerMovement();
        if (Input.GetButtonUp("Backward"))
        {
			Debug.Log("Move");
            Vector3 toMove = move.Down;

            if (isValidMove(transform.position, toMove))
            {
                newTile();
                takeTile();
                transform.Translate(toMove);
            }
        }
        if (Input.GetButtonUp("Forward"))
        {
            Vector3 toMove = move.Up;

            if (isValidMove(transform.position, toMove))
            {

                newTile();
                takeTile();
                transform.Translate(toMove);
            }
        }
        if (Input.GetButtonUp("Left"))
        {
            Vector3 toMove = move.Left;

            if (isValidMove(transform.position, toMove))
            {
                newTile();
                takeTile();
                transform.Translate(toMove);
            }
        }
        if (Input.GetButtonUp("Right"))
        {
            Vector3 toMove = move.Right;

            if (isValidMove(transform.position, toMove))
            {
                newTile();
                takeTile();
                transform.Translate(toMove);
            }
        }
    }
    public bool isValidMove(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction, Color.red, 5f);
        if (Physics.Raycast(ray, out tileHit, 6f))
        {
            if (tileHit.collider.tag != "Free")
            {
                return false;
            }

        }
        
        return true;
    }
    public void newTile()
    {
        currentTile = tileHit.collider.gameObject.transform;
    }
    public void takeTile()
    {
        currentTile.SendMessage("taken");
    }
}
