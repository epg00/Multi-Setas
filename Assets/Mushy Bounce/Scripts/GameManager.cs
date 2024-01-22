using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.VisualScripting;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;

    public enum State {Menu, Game, Win, Lose}
    private State gameState;

    private int connectedPlayers;

    public static Action<State> onGameStateChanged; //Evento para tener un seguimiento del estado del juego

    private void Awake() {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }


    public override void OnDestroy() //Using Network behaviour se usa override
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
    }
    

    public override void OnNetworkSpawn() //Cuando se llama a GameManager en el NetWork
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted(){
        
        if(!IsServer){ //Si es host
            return;
        }

        connectedPlayers++; //host tambien cuenta como jugador 
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback; //Para saber que un cliente se ha conectado          
    }

    private void Singleton_OnClientConnectedCallback(ulong obj){
        connectedPlayers++;

        if(connectedPlayers>=2){
            StartGame();
        }
    }

    public void SetGameState(State gameState){
        this.gameState = gameState;
        onGameStateChanged?.Invoke(gameState);
    }
    

    void Start(){
        gameState = State.Menu;
    }

    private void StartGame(){
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc(){
        gameState = State.Game;

        onGameStateChanged?.Invoke(gameState);
    }
    
}
