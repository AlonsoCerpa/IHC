using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using UnityEngine.SceneManagement;

public class EventNewGame : MonoBehaviour
{
    [SerializeField] private VRInteractiveItem m_VRInteractiveItem;

    void OnEnable()
    {
        m_VRInteractiveItem.OnClick += LoadSceneGame;
    }


    void OnDisable()
    {
        m_VRInteractiveItem.OnClick -= LoadSceneGame;
    }

    void LoadSceneGame()
    {
        SceneManager.LoadScene("Scenes/Game");
    }
}