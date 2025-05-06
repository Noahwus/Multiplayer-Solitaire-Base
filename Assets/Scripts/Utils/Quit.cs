using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : Singleton<Quit>
{
    private void Awake()
    {
        NewInstance(this);
    }
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
