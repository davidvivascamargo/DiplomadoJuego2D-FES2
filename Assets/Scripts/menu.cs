using UnityEngine;
using UnityEngine.SceneManagement   ;
//using UnityEngine.SceneManagement;

//MENU PRINCIPAL, PANTALLA: FUNCONES JUGAR OPCIONES SALIR

public class menu : MonoBehaviour
{
   public GameObject menuop;
   public GameObject menup;

   public void Abriropcionespanel()
    {
        menup.SetActive(false);
        menuop.SetActive(true);
    }

    public void Abriromenupanel()
    {
        menup.SetActive(true);
        menuop.SetActive(false);
    }

    public void Quit()
    {
        
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level_Prototype");
    }



}
