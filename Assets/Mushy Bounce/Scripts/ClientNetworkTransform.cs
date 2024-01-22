using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority{

    [DisallowMultipleComponent]
    public class ClientWorkTransform: NetworkTransform{

        protected override bool OnIsServerAuthoritative()
        {
            return false; //Para que el Host no controle al Cliente
        }
    }
}