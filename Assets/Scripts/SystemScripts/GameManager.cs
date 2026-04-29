using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReader), typeof(PlayerStateData))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private MonsterIdData _monsterIdData;

    public static GameManager Instance { get; private set; }

    private InputReader _inputReader;
    private PlayerStateData _playerStateData;

    public InputReader InputReader => _inputReader;
    public PlayerStateData PlayerStateData => _playerStateData;
    public MonsterIdData MonsterIdData => _monsterIdData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _inputReader = GetComponent<InputReader>();
            _playerStateData = GetComponent<PlayerStateData>();
            _monsterIdData.InitMonsterIdDict();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
