using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using UnityEngine.SceneManagement;
using Naninovel.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        await RuntimeInitializer.InitializeAsync();
        var UIPlayer = Engine.GetService<IUIManager>();


        if (SceneManager.GetActiveScene().name == "Prototype")
        {

            UIPlayer.ResetService();

        }
        


        //if (Engine.Initialized && SceneManager.GetActiveScene().name == "Prototype")
        //{

        //    var scriptPlayer = Engine.GetService<IScriptPlayer>();
        //    await scriptPlayer.PreloadAndPlayAsync("TestScript");



        //    var inputManager = Engine.GetService<IInputManager>();
        //    inputManager.ProcessInput = true;


        //}
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Prototype")
        {

            Naninovel.Engine.Destroy();

        }
        

    }


}
