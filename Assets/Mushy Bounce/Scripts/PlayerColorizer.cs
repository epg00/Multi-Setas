using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColorizer : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] renderers;

    public override void OnNetworkSpawn() //Se llama cuando el jugador u obejto aparece en el network
    {
        base.OnNetworkSpawn();

        if(!IsServer && IsOwner){ //Solo lo hace para el owner del player
            ColorizeServerRpc(Color.red);
        }
    }

    [ServerRpc] //Mensaje enviado del cliente al servidor
    private void ColorizeServerRpc(Color color){ //Metodo tiene que terminar con "ServerRpc" siempre que tengamos NetworkBehaviour y el [ServerRpc] justo encima del metodo
        ColorizeClientRpc(color);
    }

    [ClientRpc] //Mensaje enviado del servidor al cliente
    private void ColorizeClientRpc(Color color){
        foreach(SpriteRenderer renderer in renderers){
            renderer.color = color;
        }
    }
}
