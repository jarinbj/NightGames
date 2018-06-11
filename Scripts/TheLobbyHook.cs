using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class TheLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer,
        GameObject gamePlayer)
    {
        LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        Player gPlayer = gamePlayer.GetComponent<Player>();
        HandleTagged itplayer = gamePlayer.GetComponent<HandleTagged>();
        PlayerTagging itplayertagging = gamePlayer.GetComponent<PlayerTagging>();

        gPlayer.playerName = lPlayer.playerName;
        gPlayer.playerColor = lPlayer.playerColor;
        itplayer.isit = lPlayer.StartAsIt;

        if (itplayer.isit)
        {
            itplayertagging.isit = true;
            itplayer.taggable = false;
        }

    }
}

