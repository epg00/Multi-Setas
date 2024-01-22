using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour //Network Manager es un singelton y no se destruye al cambiar de escena
{
    void Start()
    {
        Application.targetFrameRate = 60; //para que intente ir 60 frames por segundo
        SceneManager.LoadScene(1);
    }
}
