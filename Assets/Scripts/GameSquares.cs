using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameSquares : MonoBehaviour {

    public GameObject Square;
    private GameObject piece;
    private GameObject slimeSprite;
    private Vector3 orig_scale;  // original scale of the slime object. Is not 1,1,1 like expected, but something like 2.2, 2.2, 20. Keep track of it here to change the scale relative to the original
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
        // check to see if the square clicked is owned by another player
        bool owned = CheckSquareOwnedByOtherPlayer();
        if (owned)
        {
            // don't end the turn until the player makes a legal move.
            Debug.Log("Try again");
        }
        else
        {
            // setup the undo button
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

            // Add slime to this square 
            AddSlime();

            // End Turn performs the explosions, then checks for a winner
            // start a coroutine here to allow for explosions to be staggered
            StartCoroutine(gameController.EndTurn());
        }

    }

    public bool CheckSquareOwnedByOtherPlayer()
    {
        int current_player = gameController.GetCurrentPlayer();
        // If this square is owned by another player, then don't allow adding slime to it
        if (this.player != 0 && current_player != this.player)
            return true;
        // otherwise it's not owned and we can add slime to it!
        return false;
    }


    public void AddSlime()
    {
        int current_player = gameController.GetCurrentPlayer();

        SetPlayer(current_player);
        SetSlime(current_slime + 1);
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
        if (player != 0)
        {
            piece = gameController.GetPlayerSlime(player);
            this.slimeSprite = (GameObject)Instantiate(piece, Square.transform.position, Quaternion.identity);
            this.slimeSprite.transform.parent = Square.transform;
            this.orig_scale = this.slimeSprite.GetComponent<SpriteRenderer>().transform.localScale;
        }
    }


    public void SetSlime(int slime)
    {
        this.current_slime = slime;
        // TODO this should be replaced with slime sprite animation rather than text
        //squareText.text = current_slime.ToString();

        if (this.current_slime == 0)
        {
            SetPlayer(0);
        }
        // Also change the size of the slime
        //Debug.Log("Player: " + this.player + ", current_slime: " + this.current_slime + ", limit: " + this.limit);
        //piece = gameController.GetPlayerSlime(player);
        if (this.player != 0)
        {
            float scale = 0;
            // set the size of the slime to the amount of space left until it explodes
            if (this.current_slime == this.limit)
            {
                // leave the slime as the largest it can be
                //myPiece.GetComponent<SpriteRenderer>().size = new Vector2(40, 40);
                scale = 1.33f;
            }
            if (this.current_slime == this.limit - 1)
            {
                // leave the slime as the largest it can be
                //myPiece.GetComponent<SpriteRenderer>().size = new Vector2(40, 40);
                scale = 1;
            }
            else if (this.current_slime == this.limit - 2)
            {
                scale = 0.66f;
                // make the slime 2/3 it's normal size
            }
            else if (this.current_slime == this.limit - 3)
            {
                // make the slime 1/3 it's normal size
                scale = 0.33f;
            }

            if (scale > 0)
            {
                Vector3 new_scale = orig_scale * scale;
                ///Debug.Log("Orig scale: " + this.orig_scale + ", New scale: " + new_scale);
                this.slimeSprite.GetComponent<SpriteRenderer>().transform.localScale = new_scale;
            }
        }
    }

}