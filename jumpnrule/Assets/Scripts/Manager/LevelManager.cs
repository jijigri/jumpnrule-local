using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] GameObject _runner;
    private LevelState _levelState;

    public int CurrentRound { get; private set; } = 6;

    public bool IsRoundPlaying { get; private set; } = false;

    public bool IsRoundTimePerfect { get; private set; } = false;
    public bool IsRoundFlawless { get; private set; } = false;

    float _timeSinceRoundStarted = 0f;

    bool _isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.onPlayerDamaged += OnPlayerDamaged;

        StartLevel();
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onPlayerDamaged -= OnPlayerDamaged;
    }

    void OnPlayerDamaged(float damage, PlayerHealthManager healthManager)
    {
        if (IsRoundPlaying)
        {
            IsRoundFlawless = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RunnerStat runnerStat = _runner.GetComponent<RunnerStat>();

        // JNR-22: Allow runner to live on pixelhealth
        double health = runnerStat.GetHealth();
        if (0 < health && health < 0.01)
        {
            runnerStat.IncreaseHealth(0.01);
        }

        if (runnerStat.IsDead())
        {
            EndLevel("Runner died");
        }

        if (IsRoundPlaying)
        {
            _timeSinceRoundStarted += Time.deltaTime;
        }

        if (_isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    // (Re)starts the run
    public void StartLevel()
    {
        _levelState = new LevelState(_runner);

        //_levelState.GetRunner().GetComponent<RunnerControl>().StartLevel();
        _levelState.GetRunner().GetComponent<RunnerStat>().StartLevel();

        //StartRound();
    }

    public void StartRound()
    {
        if (IsRoundPlaying)
        {
            Debug.Log("Can't start a round while another round is already playing!");
            return;
        }

        Debug.Log("Starting Round!");


        IsRoundFlawless = true;
        IsRoundTimePerfect = true;

        CurrentRound++;

        IsRoundPlaying = true;

        GameEvents.Instance.WaveStart(CurrentRound);
        RoundManager.Instance.StartRound();
    }

    public void EndRound()
    {
        IsRoundPlaying = false;

        if(_timeSinceRoundStarted > RoundManager.Instance.CurrentRoundPerfectTime)
        {
            IsRoundTimePerfect = false;
        }

        _timeSinceRoundStarted = 0;

        GameEvents.Instance.WaveCleared(CurrentRound);
        CancelInvoke("StartRound");
    }

    public void EndLevel(string reason = "Game over")
    {
        _isGameOver = true;
    }

    public LevelState GetLevelState()
    {
        return _levelState;
    }
}

public class LevelState
{
    private List<GameObject> _runners;
    private ItemInventory _itemInventory;

    public LevelState(GameObject runner)
    {
        // TODO Extend for more Runners
        _runners = new List<GameObject> { runner };

        _itemInventory = new ItemInventory(_runners);
    }

    public GameObject GetRunner()
    {
        return _runners[0]; // TODO Extend for more Runners
    }

    public ItemInventory GetItemInventory()
    {
        return _itemInventory;
    }
}
