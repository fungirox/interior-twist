using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    public void exit(){
        Application.Quit();
    }
    public void loadScene(string scene){
        SceneManager.LoadScene(scene);
    }
    
}
