using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]

    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    
    void Start()
    {
        ShowConectionPanel();

        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState){
        switch(gameState){
            case GameManager.State.Game:
                ShowGamePanel();
                break;
            case GameManager.State.Win:
                ShowWinPanel();
                break;
            case GameManager.State.Lose:
                ShowLosePanel();
                break;
        }
    }

    private void OnDestroy() {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void ShowConectionPanel(){
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private void ShowWaitingPanel(){
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void ShowGamePanel(){
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void ShowWinPanel(){
        winPanel.SetActive(true);
    }

    private void ShowLosePanel(){
        losePanel.SetActive(true);
    }

    public void HostButtonCallback(){
        NetworkManager.Singleton.StartHost();
        ShowWaitingPanel();
    }

    public void ClientButtonCallback(){
        //Lets grab the IP adrress that the player has entered
        string ipAddress = IPManager.instance.GetInputIP();

        //Configure the Network Manager
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(ipAddress, 7777);

        NetworkManager.Singleton.StartClient();
        ShowWaitingPanel();
    }

    public void GoBackToMenu(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        NetworkManager.Singleton.Shutdown();
    }
}
