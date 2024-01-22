using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EggManager : NetworkBehaviour
{
    public static EggManager instance;

    [Header("Elements")]
    [SerializeField] private Egg eggPrefab;
    
    private void Awake() {
        if(instance==null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start(){
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    public override void OnDestroy(){
        base.OnDestroy();
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState){
        switch(gameState){
            case GameManager.State.Game:
                SpawnEgg();
                break;
        }
    }

    private void SpawnEgg(){
        if(!IsServer){
            return;
        }

        Egg eggInstance = Instantiate(eggPrefab,Vector2.up * 5, Quaternion.identity);
        eggInstance.GetComponent<NetworkObject>().Spawn();
        eggInstance.transform.SetParent(transform);
    }

    public void ReuseEgg(){
        if(!IsServer){
            return;
        }

        if(transform.childCount <= 0){
            return;
        }

        transform.GetChild(0).GetComponent<Egg>().Reuse();
    }
}
