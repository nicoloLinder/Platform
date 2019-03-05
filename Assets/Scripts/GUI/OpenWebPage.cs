using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebPage : MonoBehaviour {

    public void OpenPage(string page){
        Application.OpenURL(page);
    }
}
