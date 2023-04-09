using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextButton : MonoBehaviour
{
    public void loadNextLevel()
    {
        GameManager.instance.gameMode = GameManager.GameType.Generate;
        GameManager.instance.sceneIsEmpty = true;
    }
}
