using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private string targetSceneName;

    private void Start()
    {
        continueButton.onClick.AddListener(ContinueToTargetScene);
    }

    public void ContinueToTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }

}
