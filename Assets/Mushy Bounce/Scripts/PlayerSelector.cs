using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSelector : NetworkBehaviour
{
    public static PlayerSelector instance;

    private bool isHostTurn;

    private void Awake() {
        if(instance==null){
            instance=this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn() //Cuando se llama a GameManager en el NetWork
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        GameManager.onGameStateChanged -= GameStateChangedCallback;
        Egg.onHit -= SwitchPlayers;
    }

    private void NetworkManager_OnServerStarted(){
        
        if(!IsServer){ //Si es host
            return;
        }

        GameManager.onGameStateChanged += GameStateChangedCallback;
        Egg.onHit += SwitchPlayers;
    }

    private void GameStateChangedCallback(GameManager.State gameState){
        switch(gameState){
            case GameManager.State.Game:
                Initilize();
                break;
        }
    }

    [System.Obsolete]
    private void Initilize(){
        //Look for every player in the game

        PlayerStateManager[] playerStateManagers = FindObjectsOfType<PlayerStateManager>();

        for(int i=0; i<playerStateManagers.Length;i++){
            if(playerStateManagers[i].GetComponent<NetworkObject>().IsOwnedByServer){
                //This is the host
                //If it is the host turn enable the host, disable the client

                if(isHostTurn){
                    playerStateManagers[i].Enable();
                }
                else{
                    playerStateManagers[i].Disable();
                }
            }
            else{
                if(isHostTurn){
                    playerStateManagers[i].Disable();
                }
                else{
                    playerStateManagers[i].Enable();
                }
            }
        }
    }

    private void SwitchPlayers(){
        isHostTurn = !isHostTurn;

        Initilize();
    }

    public bool IsHostTurn(){
        return isHostTurn;
    }

}
