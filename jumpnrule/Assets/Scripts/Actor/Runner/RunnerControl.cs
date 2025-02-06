using UnityEngine;

public class RunnerControl : MonoBehaviour
{
    [SerializeField] Camera myCamera;
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] float _speed;
    [SerializeField] LayerMask _solidMask; // Any layer the runner can detect as the ground to reset his jump

    Vector3 _resetPosition;
    bool _resetPositionSet = false;

    private void Awake()
    {
    }

    void Start()
    {
        InitLevel();
    }

    void Update()
    { 
        if (Input.GetButtonDown("Jump"))
        {
            DashAbility dash = this.gameObject.GetComponentInChildren<DashAbility>();
            if (!dash)
            {
                Debug.Log("Runner has no such ability");
            }
            else
            {
                if (!dash.TryCast())
                {
                    Debug.Log("Dash is on cooldown for " + dash.GetCooldown() + " seconds");
                }
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Blue");
            SpawnBullet(BulletBehavior.BulletType.eBlue);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Red");
            SpawnBullet(BulletBehavior.BulletType.eRed);
        }
    }

    private void SpawnBullet(BulletBehavior.BulletType bulletType)
    {
        Vector2 velocity = GetTranslatetoMouse();
        velocity = velocity.normalized * 20.0f; // TODO Implement properly

        BulletSpawner bulletSpawner = this.gameObject.GetComponentInChildren<BulletSpawner>();
        bulletSpawner.Spawn(velocity, bulletType);
    }

    public void AddImpulse(Vector2 impulse)
    {
        Debug.Log("Adding impulse of " + impulse);
        _rigidBody.transform.position += (Vector3)impulse;
    }

    public void AddImpulse(float scale)
    {
        AddImpulse(_rigidBody.velocity.normalized * scale);
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector2 velocity = new Vector2(xInput, yInput);
        velocity.Normalize();
        velocity *= _speed;

        _rigidBody.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Runner Collide: collision tag = " + collision.tag);

        if (collision.tag == "Finish")
        {
            Debug.Log("Collide: with Finish");
            LevelManager.Instance.EndLevel("Finish has been reached");
        }
    }

    private Vector2 GetTranslatetoMouse()
    {
        // Get mouse position (in screen-space) and convert to world-space
        Vector3 mousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the vector between the mouse position and object
        Vector2 translate = mousePos - transform.position;

        return translate;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    // Should be called once, when the level is loaded
    private void InitLevel()
    {
        _resetPosition = GetPosition();
        _resetPositionSet = true;
    }

    // (Re)starts the run
    public void StartLevel()
    {
        if (_resetPositionSet)
        {
            transform.position = _resetPosition;
        }
        _rigidBody.velocity = Vector2.zero;
    }
}
