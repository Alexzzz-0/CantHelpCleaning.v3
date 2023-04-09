using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GenerateTiles _tileGenerator;
    [SerializeField] private playerController _playerController;
    [SerializeField] private UIController _uiController;
    [SerializeField] private CameraController _cameraController;
    
    //make the game's state: 5 states
    public enum GameType
    {
        Default,
        Generate,
        Clean,
        UnUsed,
        Store
    }

    public GameType gameMode = GameType.Default;

    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
        {
            gameMode = GameType.Clean;
        }
        
        switch (gameMode)
        {
            case GameType.Generate:
                Generate();
                break;
            case GameType.Clean:
                Clean();
                break;
            case GameType.UnUsed:
                UnUsed();
                break;
            case GameType.Store:
                Store();
                break;

        }
    }
    
    public bool sceneIsEmpty = true;
    public bool fileIsEmpty = true;

    public GameObject tile;
    private float tileLength;

    public int level;

    public int dustNumLeft;
    public int dustNumSolved;
    
    public bool CameraMove = false;
    private Vector3 cameraMovDis;
    
    private void Start()
    {
        tileLength = tile.transform.GetComponent<Renderer>().bounds.size.x;
    }
    
    private const string generateSceneObject = "GenerateSceneHolder";
    private const string generateTileHolder = "GenerateTileHolder";
    
    
    void Generate()
    {
        //randomly assign a room to generate
        level = Random.Range(1, 4);

        if (sceneIsEmpty)
        {
            LevelLoader();
            
            gameMode = GameType.Clean;
        }
    }

    //two main functions that generate have: generate File, Map(UI)
    void GenerateASCIIFile()
    {
        transform.Find(generateTileHolder).GetComponent<GenerateTiles>().referenceFile(level);
    }

    void LevelLoader()
    {
        //instantiate the tile map from file
        dustNumLeft = _tileGenerator.InstantiateFromFile(level, tileLength);
        
        //display the UI number for this level
        _uiController.roomUIGenerate(level,dustNumLeft);
        
        //set the initial position for camera(at player's initial position)
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        Debug.Log(playerPos.ToString());
        playerPos.z = -10;
        _cameraController.CameraLoad(playerPos);
        sceneIsEmpty = false;
    }

    // //a class to hold the code for calling the class of generating tiles, for make an invoke
    // void GenerateTile()
    // {
    //     //transform.Find(generateTileHolder).GetComponent<GenerateTiles>().GenerateTile(1 + SceneManager.GetActiveScene().buildIndex,
    //         //startPositionX,startPositionY,tileLength);
    //         transform.Find(generateTileHolder).GetComponent<GenerateTiles>().referenceFile(level);
    //         Debug.Log(level.ToString());
    // }
    
    
    void Clean()
    {
        
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S))
            {
                //Debug.Log(" detect player move ");
                cameraMovDis = _playerController.MoveOrTurn(tileLength);
            }

            
            //if player has completed the level, change the level index now to get it ready for them to go to
            if (dustNumLeft == 0)
            {
                level = Random.Range(1, 4);
            }

            if (CameraMove)
            {
                MoveCamera();
                CameraMove = false;
            }
    }

    void MoveCamera()
    {
        _cameraController.CameraMove(cameraMovDis);
    }
    
    public void RoomUIUpdate()
    {
        transform.Find("UIControllerHolder").GetComponent<UIController>().roomUIUpdate(dustNumSolved, dustNumLeft);
    }

    void UnUsed(){}
    void Store(){}
}
