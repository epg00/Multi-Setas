using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Collider2D collider;

    public void Enable(){
        EnableClientRpc();
    }

    [ClientRpc]
    private void EnableClientRpc(){

        collider.enabled = true;

        foreach(SpriteRenderer renderer in renderers){
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
    }


    public void Disable(){
        DisableClientRpc();
    }

    [ClientRpc]
    private void DisableClientRpc(){

        collider.enabled = false;

        foreach(SpriteRenderer renderer in renderers){
            Color color = renderer.color;
            color.a = 0.2f;
            renderer.color = color;
        }
    }
}
