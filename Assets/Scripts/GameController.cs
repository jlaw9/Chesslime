using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController: MonoBehaviour {

    int masterRows = 7;  // number of rows in the master matrix
    int masterCols = 7;  // number of columns in the master matrix
    int totalSquare;
    // matrix of pointers to the game board squares list
    // c# doesn't use pointers, so this is the next best thing I think
    public int[,] squaresMatrix;
    public GameSquares[] squaresList;  // container of all of the game board squares
    public List<List<int>> prevSquaresList;  // contains the player and slime of each square from the previous turn
    int currentPlayer; //this shows if it’s player 1 or player 2’s turn.
    int prevPlayer; //this shows if it’s player 1 or player 2’s turn.
    int currentTurn; //how many turns since the game started.
    int rows;  // number of rows specified by the user
    int cols;  // number of columns specified by the user
    public List<int> players;  // list of players
    public List<int> prevPlayers;  // list of players
    public List<float> origXCoord;  // original x coordinate of squares

    bool exploding = false;  // boolean to stop time momentarilly when exploding

    Dictionary<int, Color32> playerColors = new Dictionary<int, Color32>();  // dictionary of colors for players
    Dictionary<int, String> playerColorsText = new Dictionary<int, String>();  // dictionary of colors names for players
    Dictionary<int, GameObject> playerSlimeColor = new Dictionary<int, GameObject>();  // dictionary of colors for players
    public GameObject redSlime;
    public GameObject blueSlime;
    public GameObject greenSlime;
    public GameObject purpleSlime;
    public GameObject defaultButton;

    // TODO create a game rules button so people can learn how to play!
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject resetButton;
    public GameObject startGame;
    public Dropdown selectRows;
    public Dropdown selectCols;
    public Dropdown selectPlayers;
    public GameObject gameBoardBackground;
    public GameObject undoButton;
    public GameObject currentPlayerPanel;
    public Text currentPlayerText;


    void Awake()
    {
        squaresMatrix = new int[masterRows, masterCols];
        prevSquaresList = new List<List<int>>();
        origXCoord = new List<float>();
        SetGameControllerReferenceOnButtons();
        playerColors[0] = new Color32(0, 0, 0, 150);
        playerColors[1] = new Color32(223, 60, 60, 255);
        playerColorsText[1] = "Player 1";
        playerColors[2] = new Color32(59, 173, 229, 255);
        playerColorsText[2] = "Player 2";
        playerColors[3] = new Color32(19, 199, 139, 255);
        playerColorsText[3] = "Player 3";
        playerColors[4] = new Color32(156, 67, 223, 255);
        playerColorsText[4] = "Player 4";
        playerColors[5] = Color.cyan;
        playerColors[6] = Color.yellow;

        playerSlimeColor[0] = defaultButton;
        playerSlimeColor[1] = redSlime;
        playerSlimeColor[2] = blueSlime;
        playerSlimeColor[3] = greenSlime;
        playerSlimeColor[4] = purpleSlime;

        // change the opacity of the slimes
        //for (int i = 1; i < 5; i++)
        //{
        //    Color tmp = playerSlimeColor[i].GetComponent<SpriteRenderer>().color;
        //    tmp.a = 0.6f;
        //    playerSlimeColor[i].GetComponent<SpriteRenderer>().color = tmp;
        //}

        InitializeBoard();
        SelectOptions();
    }


    // links it to the square pieces in Unity
    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < squaresList.Length; i++)
        {
            squaresList[i].SetGameControllerReference(this);
            prevSquaresList.Add(new List<int>());
            prevSquaresList[i].Add(0);
            prevSquaresList[i].Add(0);
        }
    }


    public void SelectOptions()
    {
        // Set selection options to true
        selectRows.gameObject.SetActive(true);
        selectCols.gameObject.SetActive(true);
        selectPlayers.gameObject.SetActive(true);
        startGame.SetActive(true);

        // set game over text to inactive
        gameOverPanel.SetActive(false);
        currentPlayerPanel.SetActive(false);
        resetButton.SetActive(false);
        undoButton.SetActive(false);
        gameBoardBackground.SetActive(false);
        for (int i = 0; i < squaresList.Length; i++)
        {
            squaresList[i].gameObject.SetActive(false);
            squaresList[i].neighbors = new List<int>();
        }
    }


    public void StartGame()
    {
        selectRows.gameObject.SetActive(false);
        selectCols.gameObject.SetActive(false);
        selectPlayers.gameObject.SetActive(false);
        startGame.SetActive(false);
        gameBoardBackground.SetActive(true);
        undoButton.SetActive(true);
        undoButton.GetComponent<Button>().interactable = false;
        currentPlayerPanel.SetActive(true);

        // these should be selected by the user
        rows = int.Parse(selectRows.captionText.text);
        cols = int.Parse(selectCols.captionText.text);
        int num_players = int.Parse(selectPlayers.captionText.text);
        Debug.Log(rows.ToString() +","+ cols.ToString());

        // number of players
        players = new List<int>();
        for (int i = 1; i <= num_players; i++)
        {
            players.Add(i);
        }

        BuildBoard(rows, cols);

        currentPlayer = 1;
        currentTurn = 1;
        // set the message to be the current player's turn
        currentPlayerText.text = playerColorsText[currentPlayer] + "'s Turn";
        // also set the color
        currentPlayerText.color = playerColors[currentPlayer];
    }


    public void SetBoardBackground(int rows, int cols)
    {
        // set the size of the game board background
        gameBoardBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * cols + 10, 50 * rows + 10);

        // set the coordinates
        // middle x coord of the squares 
        float middleXCoord = 0;
        float middleYCoord = 0;
        for (int col = 0; col < cols; col++)
        {
            middleXCoord += squaresList[squaresMatrix[0, col]].transform.position.x;
        }
        for (int row = 0; row < rows; row++)
        {
            middleYCoord += squaresList[squaresMatrix[row, 0]].transform.position.y;
        }
        // divide by the number of columns to get the average x coordinate
        middleXCoord = middleXCoord / cols;

        middleYCoord = middleYCoord / rows;
        // move it to the center of the active rows and columns
        gameBoardBackground.transform.position = new Vector3(middleXCoord, middleYCoord, gameBoardBackground.transform.position.z);

    }


    void InitializeBoard()
    {
        // add the index of the square in the squaresList to the matrix
        int index = 0;
        for (int row = 0; row < masterRows; row++)
        {
            for (int col = 0; col < masterCols; col++)
            {
                squaresMatrix[row, col] = index;
                index++;
            }
        }
    }


    // Initializes the squares with their neighbors and limit
    void BuildBoard(int rows, int cols)
    {
        int index;
        // add the limit and neighbors to each square
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                //Debug.Log("Adding " + row + "," + col);
                index = squaresMatrix[row, col];
                squaresList[index].gameObject.SetActive(true);
                //Debug.Log("Testing index: " + index.ToString() + ", row: " + row.ToString() + ", col: " + col.ToString());
                if (row - 1 >= 0)
                {
                    squaresList[index].neighbors.Add(squaresMatrix[row - 1, col]);  // add the left neighbor
                }
                if (row + 1 < rows)
                {
                    squaresList[index].neighbors.Add(squaresMatrix[row + 1, col]);  // add the left neighbor
                }
                if (col - 1 >= 0)
                {
                    squaresList[index].neighbors.Add(squaresMatrix[row, col - 1]);  // add the left neighbor
                }
                if (col + 1 < cols)
                {
                    squaresList[index].neighbors.Add(squaresMatrix[row, col + 1]);  // add the left neighbor
                }
                // set the limit of the square equal to the number of neighbors
                squaresList[index].limit = squaresList[index].neighbors.Count;
            }
        }


        // if this is the first game, then get the original x coordinates
        if (origXCoord.Count == 0)
        {
            // the original has 7 columns total
            for (int i = 0; i < 7; i++)
            {
                origXCoord.Add(squaresList[i].transform.position.x);
            }
        }
        //origXCoord.Add(squaresList[squaresMatrix[0, col]].transform.position.x);

        // Move the board to the center
        // set the coordinates
        // middle x coord of the squares 
        float middleXCoord = 0;
        for (int col = 0; col < cols; col++)
        {
            middleXCoord += origXCoord[col];
        }
        // divide by the number of columns to get the average x coordinate
        middleXCoord = middleXCoord / cols;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // move it to the center of the camera (x coordinate of 0)
                float transformX = origXCoord[col] - middleXCoord;
                float y = squaresList[squaresMatrix[row, col]].transform.position.y;
                squaresList[squaresMatrix[row, col]].transform.position = new Vector3(transformX, y, gameBoardBackground.transform.position.z);
            }
        }
        // now set the board background behind the board
        SetBoardBackground(rows, cols);
    }


    // check if there is a winner or move to the next player's turn.
    public IEnumerator EndTurn()
    {
        // set the board to inactive so that no one can place slime while the game is waiting
        SetBoardInteractable(toggle: false);
        // check for explosions
        while (true)
        {
            List<int> explodingSquares = GetExplodingSquares();
            // if there are no squares to explode, then break out of this while loop
            if (explodingSquares.Count == 0)
                break;

            // if there are squares to explode, then wait for 1 second(s) between each explosion to give a better effect
            yield return new WaitForSeconds(0.5f);

            // now actually explode the squares
            foreach (int index in explodingSquares)
                ExplodeSquare(index);

            // check for a winner after each explosion
            if (CheckWin())
            {
                GameOver();
                // if there was a winner, break out of the explosion loop
                break;
            }
        }

        //reactivate the board after all explosions are done
        SetBoardInteractable(toggle: true);

        prevPlayer = currentPlayer;
        // also activate the undo button
        undoButton.GetComponent<Button>().interactable = true;
        
        NextPlayer();
        currentTurn++;
    }


    // checks the board to see if there are any explosions. If there are, it explodes the square.
    // returns true if at least one square exploded. Otherwise returns false
    List<int> GetExplodingSquares()
    {
        //bool explosion = false;
        List<int> explodingSquares = new List<int>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = squaresMatrix[row, col];
                if (squaresList[index].current_slime >= squaresList[index].limit)
                {
                    // explode this square
                    //explosion = true;
                    // This will allow the explosions to happen in stages.
                    explodingSquares.Add(index);
                }
            }
        }

        return explodingSquares;
    }


    // set slime of square to 0, and take over and add slime to the square's neighbors
    void ExplodeSquare(int squareIndex)
    {
        // set the slime in the current square to 0 (empty)
        squaresList[squareIndex].SetSlime(0);
        // add 1 to all of the neighbors of the exploding square
        foreach (int neighbor in squaresList[squareIndex].neighbors)
        {
            // No need to check if the square is owned. It will be taken over 
            squaresList[neighbor].AddSlime();
        }
    }


    //checks the board to see if player on is the only one left on the board or if player to is the only one left on the board.Breaks out of the loop if the player is found
    bool CheckWin()
    {
        bool hasWinner = false;
        HashSet<int> currentPlayers = new HashSet<int>();
        for (int i = 0; i < squaresList.Length; i++)
        {
            currentPlayers.Add(squaresList[i].player);
        }
        // remove the null player
        currentPlayers.Remove(0);

        // remove any players that aren't on the board anymore
        List<int> newPlayersList = new List<int>();
        foreach (int player in players)
        {
            if (!currentPlayers.Contains(player))
            {
                Debug.Log("Player " + player + " DEFEATED");
            }
            else
            {
                newPlayersList.Add(player);
            }
        }
        players = newPlayersList;

        if (currentPlayers.Count == 1)
        {
            //Debug.Log("Player " + currentPlayer.ToString() + " wins!");
            // only one player left, so we have a winner!
            hasWinner = true;
        }
        return hasWinner;
    }


    void GameOver()
    {
        gameOverPanel.SetActive(true);
        // the player who took the last turn won
        gameOverText.text = playerColorsText[currentPlayer] + " Wins!";
        gameOverText.color = playerColors[currentPlayer];
        SetBoardInteractable(toggle: false);
        // give the option to play again
        resetButton.SetActive(true);
        undoButton.SetActive(false);
        currentPlayerPanel.SetActive(false);
    }


    public void SetBoardInteractable(bool toggle=false)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                squaresList[squaresMatrix[row, col]].button.interactable = toggle;
            }
        }
    }


    public void RestartGame()
    {
        SetBoardInteractable(toggle: true);
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // reset every square to 0 slime
                squaresList[squaresMatrix[row, col]].SetSlime(0);
            }
        }
        SelectOptions();
    }

    public void Undo()
    {
        undoButton.GetComponent<Button>().interactable = false;
        //squaresList = CopySquareStatus(prevSquaresList, squaresList);
        for (int i = 0; i < squaresList.Length; i++)
        {
            //Debug.Log("Setting slime and player of square " + i + " to " + prevSquaresList[i][0] + " " + prevSquaresList[i][1]);
            if (squaresList[i].player != prevSquaresList[i][0])
            {
                squaresList[i].SetPlayer(prevSquaresList[i][0]);
            }
            if (squaresList[i].current_slime != prevSquaresList[i][1])
            {
                squaresList[i].SetSlime(prevSquaresList[i][1]);
            }
        }
        SetCurrentPlayer(prevPlayer);
        players = new List<int>(prevPlayers);
        currentTurn--;
    }

    // Next player's turn
    void NextPlayer()
    {
        int curr_player_index = players.IndexOf(currentPlayer);
        int next_player_index = (curr_player_index + 1) % players.Count;
        SetCurrentPlayer(players[next_player_index]);
    }

    public void SetCurrentPlayer(int player)
    {
        this.currentPlayer = player;
        // set the message to be the current player's turn
        currentPlayerText.text = playerColorsText[currentPlayer] + "'s Turn";
        // also set the color
        currentPlayerText.color = playerColors[currentPlayer];
    }


    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public Color32 GetPlayerColor(int player)
    {
        return playerColors[player];
    }

    public GameObject GetPlayerSlime(int player)
    {
        return playerSlimeColor[player];
    }

/*
    IEnumerator PauseFunction()
    {
        // set exploding back to false to resume game after this function
        exploding = false;
        Debug.Log("I'm pausing the game");
        yield return new WaitForSeconds(2);
        Debug.Log("Done pausing the game");
    }

    void Update()
    {
        if (exploding == true)
        {
            StartCoroutine(PauseFunction());
            Debug.Log("Game Paused");
        }
        if (exploding)
        {
            // wait here
            Debug.Log("waiting");
        }
    }
*/

}