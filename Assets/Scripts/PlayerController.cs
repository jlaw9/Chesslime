using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayerController : NetworkBehaviour
{

    public int score;
    // TODO
    public int player_id;
    //MatchController matchController = FindObjectOfType(MatchController);
    MatchController matchController;
    GameController gameController;

    public override void OnStartClient()
    {
        base.OnStartClient();
        gameController = FindObjectOfType<GameController>();
        matchController = gameController.matchController;
        matchController.OnPlayerStarted(this);
        //this.PostNotification(Started);
    }
    public override void OnStartLocalPlayer()
    {
        Debug.Log("local player started");
        base.OnStartLocalPlayer();
        //gameController = this.GetComponentInParent<GameController>();
        gameController = FindObjectOfType<GameController>();
        matchController = gameController.matchController;
        matchController.OnPlayerStartedLocal(this);
        //this.PostNotification(StartedLocal);
    }
    void OnDestroy()
    {
        matchController.OnPlayerDestroyed(this);
        //this.PostNotification(Destroyed);
    }

    [Command]
    public void CmdStartGame()
    {
        // TODO
        Debug.Log("Starting Game Cmd");
        RpcStartGame();
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        // TODO
        Debug.Log("Starting Game");
        gameController.StartGame();
    }

    [Command]
    public void CmdRestartGame()
    {
        // TODO
        Debug.Log("Re-Starting Game Cmd");
        RpcRestartGame();
    }

    [ClientRpc]
    public void RpcRestartGame()
    {
        // TODO
        Debug.Log("Re-Starting Game");
        gameController.RestartGame();
    }

    [Command]
    public void CmdMarkSquare(int index)
    {
        RpcMarkSquare(index);
    }

    [ClientRpc]
    public void RpcMarkSquare(int index)
    {
        // have the player take the turn
        gameController.squaresList[index].TakeTurn(); 
    }

    [Command]
    public void CmdUndo()
    {
        RpcUndo();
    }

    [ClientRpc]
    public void RpcUndo()
    {
        gameController.Undo();
    }

}
