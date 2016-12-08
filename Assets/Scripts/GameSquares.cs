using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameSquares : MonoBehaviour {

    public GameObject Square;
    private GameObject piece;
    public int player = 0; // What player owns this square, 0 for neither, 1 for player 1, and 2 for player 2.
    public int limit; //type of square/limit of pieces before it's set to explode
    public int current_slime = 0;  // amount of slime currently in this square
    public Button button;
    public Text squareText;  // text to display
    //int squarePos; //where the square is in the board array.
    public List<int> neighbors;  // the neighboring squares of this square
    private GameController gameController;


    public void SetGameControllerReference(GameController controller)
    {
        gameController = controller;
    }


    public void TakeTurn()
    {
        // get the game square status before this turn was taken so the user can undo.
        //gameController.prevSquaresList = gameController.CopySquareStatus(gameController.squaresList, gameController.prevSquaresList);
        // TODO Move this for loop to a function in the game controller
        for (int i = 0; i < gameController.squaresList.Length; i++)
        {
            gameController.prevSquaresList[i][0] = gameController.squaresList[i].player;
            gameController.prevSquaresList[i][1] = gameController.squaresList[i].current_slime;
        }
        // get the current list of players
        gameController.prevPlayers = new List<int>(gameController.players);

        bool added = AddSlime(takeover:false);
        if (added)
        {
            gameController.EndTurn();
        }
        else
        {
            // don't end the turn until the player makes a legal move.
            Debug.Log("Try again");
        }

    }


    public bool AddSlime(bool takeover=false)
    {
        int current_player = gameController.GetCurrentPlayer();
        if (player == 0 || takeover)
        {
            SetPlayer(current_player);
        }
        if (current_player != player)
        {
            // illegal move
            Debug.Log("square owned by: " + this.player);
            return false;
        }
        else
        {
            SetSlime(current_slime + 1);
            return true;
        }
    }


    public void SetPlayer(int player)
    {
        this.player = player;

        foreach (Transform child in transform)
        {
           if(child.name != "Text")
                GameObject.Destroy(child.gameObject);
        }
        //removes current piece if one currently exists

        // set the color of the text to be the player's color.
        //squareText.color = gameController.GetPlayerColor(player);
        // TODO set the game peice animation rather than the button image
        //Square.GetComponent<Image>().sprite = gameController.GetPlayerSlime(player);

        piece = gameController.GetPlayerSlime(player);
        GameObject myPiece = (GameObject) Instantiate(piece, Square.transform.position, Quaternion.identity);
        myPiece.transform.parent = Square.transform;
    }


    public void SetSlime(int slime)
    {
        current_slime = slime;
        // TODO this should be replaced with slime sprite animation rather than text
        squareText.text = current_slime.ToString();

        if (current_slime == 0)
        {
            SetPlayer(0);
        }
    }

}