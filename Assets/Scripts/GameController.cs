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

    Dictionary<int, Color32> playerColors = new Dictionary<int, Color32>();  // dictionary of colors for players
    Dictionary<int, Sprite> playerSlimeColor = new Dictionary<int, Sprite>();  // dictionary of colors for players
    public Sprite redSlime;
    public Sprite blueSlime;
    public Sprite greenSlime;
    public Sprite purpleSlime;
    public Sprite defaultButton;

    // test animation
    public Sprite[] greenSlimes;
    int spriteIndex = 0;
    int currentIndex = 0;
    int delay = 5;

    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject resetButton;
    public GameObject startGame;
    public Dropdown selectRows;
    public Dropdown selectCols;
    public Dropdown selectPlayers;
    public GameObject gameBoardBackground;
    public GameObject undoButton;


    void Update()
    {
        spriteIndex++;
        if (spriteIndex % delay == 0)
        {
            currentIndex = (spriteIndex / delay) % greenSlimes.Length;
            squaresList[0].GetComponent<Image>().sprite = greenSlimes[currentIndex];
        }
    }

    void Awake()
    {
        squaresMatrix = new int[masterRows, masterCols];
        prevSquaresList = new List<List<int>>();
        SetGameControllerReferenceOnButtons();
        playerColors[0] = new Color32(0, 0, 0, 150);
        playerColors[1] = Color.red;
        playerColors[2] = Color.blue;
        playerColors[3] = Color.green;
        playerColors[4] = Color.magenta;
        playerColors[5] = Color.cyan;
        playerColors[6] = Color.yellow;
        playerSlimeColor[0] = defaultButton;
        playerSlimeColor[1] = redSlime;
        playerSlimeColor[2] = blueSlime;
        playerSlimeColor[3] = greenSlime;
        playerSlimeColor[4] = purpleSlime;


        initializeBoard();
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


    void initializeBoard()
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
        SetBoardBackground(rows, cols);
        int index;
        // add the limit and neighbors to each square
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Debug.Log("Adding " + row + "," + col);
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
        /* 
        // inactivate any unused squares
        // check by rows and then by columns. If the user selects the same number of rows as the master, then the first for loop won't start.
        for (int row = 0; row < masterRows; row++)
        {
            for (int col = 0; col < masterCols; col++)
            {
                if (row >= rows || col >= cols)
                {
                    Debug.Log("Inactivating " + row + "," + col);
                    squaresList[squaresMatrix[row, col]].gameObject.SetActive(false);
                }
            }
        }
        */
    }


    // check if there is a winner and move to the next player's turn.
    public void EndTurn()
    {
        //Debug.Log("EndTurn is not implemented!");
        // check for explosions
        while (checkExplosions())
        {
            // check for a winner after each explosion
            if (checkWin())
            {
                GameOver();
                // if there was a winner, break out of the explosion loop
                break;
            }
        }
        prevPlayer = currentPlayer;
        // also activate the undo button
        undoButton.GetComponent<Button>().interactable = true;
        
        NextPlayer();
        currentTurn++;


    }


    // checks the board to see if there are any explosions. If there are, it explodes the square.
    // returns true if at least one square exploded. Otherwise returns false
    bool checkExplosions()
    {
        bool explosion = false;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = squaresMatrix[row, col];
                if (squaresList[index].current_slime >= squaresList[index].limit)
                {
                    // explode this square
                    explosion = true;
                    ExplodeSquare(index);
                }
            }
        }
        return explosion;
    }


    // set slime of square to 0, and take over and add slime to the square's neighbors
    void ExplodeSquare(int squareIndex)
    {
        squaresList[squareIndex].SetSlime(0);
        foreach (int neighbor in squaresList[squareIndex].neighbors)
        {
            squaresList[neighbor].AddSlime(takeover:true);
        }
    }


    //checks the board to see if player on is the only one left on the board or if player to is the only one left on the board.Breaks out of the loop if the player is found
    bool checkWin()
    {
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
            return true;
        }
        // No winner yet.
        return false;
    }


    void GameOver()
    {
        gameOverPanel.SetActive(true);
        // the player who took the last turn won
        gameOverText.text = "Player " + currentPlayer.ToString() + " Wins!";
        SetBoardInteractable(toggle: false);
        // give the option to play again
        resetButton.SetActive(true);
        undoButton.SetActive(false);
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
        currentPlayer = prevPlayer;
        players = new List<int>(prevPlayers);
        currentTurn--;
    }

    // Next player's turn
    void NextPlayer()
    {
        int curr_player_index = players.IndexOf(currentPlayer);
        int next_player_index = (curr_player_index + 1) % players.Count;
        currentPlayer = players[next_player_index];
    }


    public int GetPlayerSide()
    {
        return currentPlayer;
    }

    public Color32 GetPlayerColor(int player)
    {
        return playerColors[player];
    }

    public Sprite GetPlayerSlime(int player)
    {
        return playerSlimeColor[player];
    }

}