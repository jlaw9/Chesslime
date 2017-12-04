using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{

    public GameController gameController;
    public bool IsReady { get { return localPlayer != null && remotePlayer != null; } }
    public PlayerController localPlayer;
    public PlayerController remotePlayer;
    public PlayerController hostPlayer;
    public PlayerController clientPlayer;
    public List<PlayerController> players = new List<PlayerController>();

    public void OnPlayerStarted(PlayerController player)
    {
        players.Add(player);
        Configure();
    }

    public void OnPlayerStartedLocal(PlayerController player)
    {
        Debug.Log("Local player started");
        localPlayer = player;
        Configure();
    }

    public void OnPlayerDestroyed(PlayerController pc)
    {
        if (localPlayer == pc)
            localPlayer = null;
        if (remotePlayer == pc)
            remotePlayer = null;
        if (hostPlayer == pc)
            hostPlayer = null;
        if (clientPlayer == pc)
            clientPlayer = null;
        if (players.Contains(pc))
            players.Remove(pc);
    }

    void Configure()
    {
        if (localPlayer == null || players.Count < 2)
            return;

        Debug.Log("Configuring players");
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].player_id = i + 1;
            if (players[i] != localPlayer)
            {
                remotePlayer = players[i];
                //break;
            }
        }
        hostPlayer = (localPlayer.isServer) ? localPlayer : remotePlayer;
        clientPlayer = (localPlayer.isServer) ? remotePlayer : localPlayer;
        //this.PostNotification(MatchReady);
        // Game is ready to play! Show the beginning of the game
        gameController.selectPlayers.captionText.text = players.Count.ToString();
        // TODO set the options to the # of players connected
        //gameController.selectPlayers.options = players;
        gameController.SelectOptions();
    }

}
