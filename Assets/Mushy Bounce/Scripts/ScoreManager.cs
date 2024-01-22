using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Elements")]
    private int hostScore;
    private int clientScore;


    public override void OnNetworkSpawn() //Cuando se llama a GameManager en el NetWork
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    void Start(){
        UpdateScoreText();
    }

    private void NetworkManager_OnServerStarted(){
        
        if(!IsServer){ //Si es host
            return;
        }

        Egg.onFellInWater += EggFellInWaterCallBack;
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState){
        switch(gameState){
            case GameManager.State.Game:
                ResetScore();
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        Egg.onFellInWater -= EggFellInWaterCallBack;
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void EggFellInWaterCallBack(){
        if(PlayerSelector.instance.IsHostTurn()){
            clientScore++;
        }
        else{
            hostScore++;
        }
        UpdateScoreClientRpc(hostScore,clientScore);
        UpdateScoreText();
        CheckForEndGame();
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int hostScore, int clientScore){
        this.hostScore = hostScore;
        this.clientScore = clientScore;
    }

    private void UpdateScoreText(){
        UpdateScoreTextClientRpc();
    }

    [ClientRpc]
    private void UpdateScoreTextClientRpc(){
        scoreText.text = "<color=#0055ffff>" + hostScore + "</color>" + " - " + "<color=#ff0055ff>" + clientScore + "</color>";
    }

    private void ResetScore(){
        hostScore = 0;
        clientScore = 0;

        UpdateScoreClientRpc(hostScore,clientScore);
        UpdateScoreText();
    }

    private void CheckForEndGame(){
        if(hostScore >= 3){
            HostWin();
        }
        else if(clientScore >= 3){
            ClientWin();
        }
        else{
            //Reinciamos pelota
            ReuseEgg();
        }
    }

    private void ReuseEgg(){
        EggManager.instance.ReuseEgg();
    }

    private void HostWin(){
        HostWinClientRpc();
    }

    [ClientRpc]
    private void HostWinClientRpc(){
        if(IsServer){
            GameManager.instance.SetGameState(GameManager.State.Win);
        }
        else{
            GameManager.instance.SetGameState(GameManager.State.Lose);
        }
    }

    private void ClientWin(){
        ClientWinClientRpc();
    }

    [ClientRpc]
    private void ClientWinClientRpc(){
        if(IsServer){
            GameManager.instance.SetGameState(GameManager.State.Lose);
        }
        else{
            GameManager.instance.SetGameState(GameManager.State.Win);
        }
    }
}
