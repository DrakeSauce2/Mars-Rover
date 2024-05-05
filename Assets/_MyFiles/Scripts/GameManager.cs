using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Next Level")]
    [SerializeField] private SceneAsset targetScene;

    [Header("Stat Counts")]
    [SerializeField] private int moveCount = 0;
    [SerializeField] private int collisionCount = 0;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Start()
    {
        moveCount = 0; 
        collisionCount = 0;  
    }

    public void ContinueToTargetScene()
    {
        SceneManager.LoadScene(targetScene.name);
    }

    public void IncrementMoveCount() => moveCount++;
    public int GetMoveCount() { return moveCount; }

    public void IncrementCollisionCount() => collisionCount++;
    public int GetCollisionsCount() { return collisionCount; }


}
