using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }

    //[SerializeField] TMP_Text _healthValue;
    //[SerializeField] TMP_Text _goldValue;
    [Header("Health Fields")]
    [SerializeField] Slider _healthSlider = null;
    [SerializeField] Slider _overflowHealthSlider = null;
    [SerializeField] Slider _backHealthSlider = null;
    [SerializeField] Sprite _backHealthDamageSprite = null;
    [SerializeField] Sprite _backHealthHealSprite = null;
    [Header("Armor Fields")]
    [SerializeField] Slider _armorSlider = null;
    [SerializeField] Slider _backArmorSlider = null;
    [SerializeField] Sprite _backArmorDamageSprite = null;
    [SerializeField] Sprite _backArmorHealSprite = null;
    [Header("Energy Fields")]
    [SerializeField] Slider _energySlider = null;
    [Header("Mobility Ability Fields")]
    [SerializeField] Image _movementAbilityCooldown = null;
    [SerializeField] Image _movementAbilityImage = null;
    [SerializeField] Animator _mobilityAnimation = null;
    [Header("First Ability Fields")]
    [SerializeField] Image _firstAbilityCooldown = null;
    [SerializeField] Image _firstAbilityImage = null;
    [SerializeField] Animator _firstAbilityAnimation = null;
    [SerializeField] Slider _firstAbilityCostSlider = null;
    [SerializeField] Animator _firstAbilityAnimator = null; 
    Animator _firstAbilitySliderFillAnimator = null; 
    [Header("Other Fields")]
    [SerializeField] TMP_Text _currentRoundText = null;
    [SerializeField] GameObject implantPanel = null;

    bool _isRollOnCooldown;
    StarterCharacterMovement _controller;

    float _maxTimeBeforeUpdatingHealth = .5f;
    float _timeLeftBeforeUpdatingHealth = 0;

    float _currentEnergy;

    PlayerHealthManager _playerHealthManager = null;

    CooldownManager _firstAbilityCooldownData;
    CooldownManager _secondAbilityCooldownData;

    PlayerAbility _firstAbility;
    PlayerAbility _secondAbility;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameEvents.Instance.onPlayerRoll += OnPlayerRoll;
        GameEvents.Instance.onRoundStarted += OnWaveStarted;
        GameEvents.Instance.onRoundCleared += OnWaveCleared;
        GameEvents.Instance.onPlayerDamaged += OnPlayerDamaged;
        GameEvents.Instance.onPlayerHealed += OnPlayerHealed;
        GameEvents.Instance.onPlayerGetArmor += OnPlayerGetArmor;
        GameEvents.Instance.onPlayerGetEnergy += OnPlayerGetEnergy;
        GameEvents.Instance.onPlayerConsumeEnergy += OnPlayerConsumeEnergy;

        _firstAbilitySliderFillAnimator = _firstAbilityCostSlider.GetComponentInChildren<Animator>();

        SetHealthBar(1, 1);
        _backHealthSlider.maxValue = 1;
        _backHealthSlider.value = 1;
        _currentRoundText.text = "Start by interacting with the Altar";
        SetOverflowHealthBar(0, 0);

        SetArmorBar(0, 1);

        SetEnergyBar(0, 1);
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onPlayerRoll -= OnPlayerRoll;
        GameEvents.Instance.onRoundStarted -= OnWaveStarted;
        GameEvents.Instance.onRoundCleared -= OnWaveCleared;
        GameEvents.Instance.onPlayerDamaged -= OnPlayerDamaged;
        GameEvents.Instance.onPlayerHealed -= OnPlayerHealed;
        GameEvents.Instance.onPlayerGetArmor -= OnPlayerGetArmor;
        GameEvents.Instance.onPlayerGetEnergy -= OnPlayerGetEnergy;
        GameEvents.Instance.onPlayerConsumeEnergy -= OnPlayerConsumeEnergy;
    }

    public void SetPlayerHealthManager(PlayerHealthManager playerHealthManager)
    {
        _playerHealthManager = playerHealthManager;

        float healthDifference = playerHealthManager.GetMaxHealth() / playerHealthManager.GetMaxOverflowHealth();
        float size = _healthSlider.GetComponent<RectTransform>().rect.width / healthDifference;
        _overflowHealthSlider.GetComponent<RectTransform>().sizeDelta  = new Vector2(size, 18);
    }


    void Update()
    {
        UpdateRollCooldown();
        UpdateAbilityCooldown();

        if(_timeLeftBeforeUpdatingHealth > 0)
        {
            _timeLeftBeforeUpdatingHealth -= Time.deltaTime;
            if (_timeLeftBeforeUpdatingHealth <= 0)
            {
                StartCoroutine(CRT_SetBar(_playerHealthManager.GetHealth(), 4));
            }
        }
        else
        {
            _timeLeftBeforeUpdatingHealth = 0;
        }

        if (_playerHealthManager != null)
        {
            SetArmorBar(_playerHealthManager.GetArmor(), _playerHealthManager.GetMaxArmor());
        }
    }

    void UpdateRollCooldown()
    {
        if (_isRollOnCooldown)
        {
            float currentValue = _controller.GetCurrentCooldown() / _controller.GetMaxCooldown();
            currentValue = (currentValue - 1) * -1;
            _movementAbilityCooldown.fillAmount = currentValue;
            _movementAbilityImage.fillAmount = currentValue;
            _movementAbilityImage.color = new Color(_movementAbilityImage.color.r, _movementAbilityImage.color.g, _movementAbilityImage.color.b, .25f);

            if (currentValue >= 1)
            {
                _isRollOnCooldown = false;
            }
        }
        else
        {
            _movementAbilityImage.color = new Color(_movementAbilityImage.color.r, _movementAbilityImage.color.g, _movementAbilityImage.color.b, 1f);
        }

        if (_mobilityAnimation != null)
        {
            _mobilityAnimation.SetBool("onCooldown", _isRollOnCooldown);
        }
    }

    void UpdateAbilityCooldown()
    {
        bool isOnCooldown = _firstAbilityCooldownData.GetTimeSinceLastCast() > 0 && _firstAbilityCooldownData.GetTimeSinceLastCast() < 60.0f;
        if (_firstAbilityCooldownData != null)
        {
            if (isOnCooldown)
            {
                float currentValue = _firstAbilityCooldownData.GetTimeSinceLastCast() / 60.0f;
                _firstAbilityCooldown.fillAmount = currentValue;
                _firstAbilityImage.fillAmount = currentValue;
                _firstAbilityImage.color = new Color(_firstAbilityImage.color.r, _firstAbilityImage.color.g, _firstAbilityImage.color.b, .25f);
            }
            else
            {
                if (_currentEnergy >= _firstAbilityCostSlider.maxValue)
                {
                    _firstAbilityImage.color = new Color(_firstAbilityImage.color.r, _firstAbilityImage.color.g, _firstAbilityImage.color.b, 1f);
                }
                else
                {
                    _firstAbilityImage.color = new Color(_firstAbilityImage.color.r, _firstAbilityImage.color.g, _firstAbilityImage.color.b, .25f);
                }
            }
        }

        if (_firstAbilityAnimator != null)
        {
            _firstAbilityAnimator.SetBool("onCooldown", isOnCooldown || _currentEnergy < _firstAbilityCostSlider.maxValue);
        }

        _firstAbilityCostSlider.value = _currentEnergy;
        _firstAbilitySliderFillAnimator.SetBool("isFull", _currentEnergy >= _firstAbilityCostSlider.maxValue ? true : false);
    }

    void OnPlayerDamaged(float damageAmount, PlayerHealthManager playerHealthManager)
    {
        if(_playerHealthManager == null)
        {
            _playerHealthManager = playerHealthManager;
        }
        /*
       _backHealthSlider.image.sprite = _backHealthDamageSprite;

       StopAllCoroutines();

       SetHealthBar(playerHealthManager.GetHealth(), playerHealthManager.GetMaxHealth());

       if (_timeLeftBeforeUpdatingHealth <= 0)
       {
           _backHealthSlider.value = playerHealthManager.GetHealth() + damageAmount;
       }

       _timeLeftBeforeUpdatingHealth = _maxTimeBeforeUpdatingHealth;
        */

        if (playerHealthManager.GetHealth() <= playerHealthManager.GetMaxHealth())
        {
            SetHealthBar(playerHealthManager.GetHealth(), playerHealthManager.GetMaxHealth());
            SetOverflowHealthBar(0, playerHealthManager.GetMaxHealth());
        }
        else
        {
            SetHealthBar(playerHealthManager.GetMaxHealth(), playerHealthManager.GetMaxHealth());
            SetOverflowHealthBar(playerHealthManager.GetHealth() - playerHealthManager.GetMaxHealth(), playerHealthManager.GetMaxHealth());
        }
    }

    void OnPlayerHealed(float healAmount, PlayerHealthManager playerHealthManager)
    {
        if (_playerHealthManager == null)
        {
            _playerHealthManager = playerHealthManager;
        }

        if(playerHealthManager.GetHealth() <= playerHealthManager.GetMaxHealth())
        {
            SetHealthBar(playerHealthManager.GetHealth(), playerHealthManager.GetMaxHealth());
            SetOverflowHealthBar(0, playerHealthManager.GetMaxOverflowHealth());
        }
        else
        {
            SetHealthBar(playerHealthManager.GetMaxHealth(), playerHealthManager.GetMaxHealth());
            SetOverflowHealthBar(playerHealthManager.GetHealth() - playerHealthManager.GetMaxHealth(), playerHealthManager.GetMaxOverflowHealth());
        }

        /*
        _backHealthSlider.image.sprite = _backHealthHealSprite;

        StopAllCoroutines();

        SetBackHealthBar(playerHealthManager.GetHealth(), playerHealthManager.GetMaxHealth());

        _timeLeftBeforeUpdatingHealth = _maxTimeBeforeUpdatingHealth;
        */
    }

    void OnPlayerGetArmor(float armorAmount, PlayerHealthManager playerHealthManager)
    {
        if (_playerHealthManager == null)
        {
            _playerHealthManager = playerHealthManager;
        }

        StopAllCoroutines();

        SetArmorBar(_playerHealthManager.GetArmor(), _playerHealthManager.GetMaxArmor());
    }

    void OnPlayerGetEnergy(float energyAmount, PlayerAbilityManager abilityManager)
    {
        SetEnergyBar(abilityManager.CurrentEnergy, abilityManager.maxEnergy);
        _currentEnergy = abilityManager.CurrentEnergy;
    }

    void OnPlayerConsumeEnergy(float energyAmount, PlayerAbilityManager abilityManager)
    {
        SetEnergyBar(abilityManager.CurrentEnergy, abilityManager.maxEnergy);
        _currentEnergy = abilityManager.CurrentEnergy;
    }

    void SetHealthBar(float health, float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _backHealthSlider.maxValue = maxHealth;
        _healthSlider.value = health;
    }

    void SetOverflowHealthBar(float health, float maxHealth)
    {
        _overflowHealthSlider.maxValue = maxHealth;
        _overflowHealthSlider.value = health;
    }

    void SetArmorBar(float armor, float maxArmor)
    {
        _armorSlider.maxValue = maxArmor;
        _backArmorSlider.maxValue = maxArmor;
        _armorSlider.value = armor;
    }

    void SetEnergyBar(float energy, float maxEnergy)
    {
        _energySlider.maxValue = maxEnergy;
        _energySlider.value = energy;
    }

    void SetBackHealthBar(float health, float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _backHealthSlider.maxValue = maxHealth;
        _backHealthSlider.value = health;
    }

    IEnumerator CRT_SetBar(float valueToReach, float speed)
    {
        float originalValue = _backHealthSlider.value;
        float originalValue2 = _healthSlider.value;
        float delta = 0;

        while(delta < 1)
        {
            _backHealthSlider.value = Mathf.Lerp(originalValue, valueToReach, delta);
            _healthSlider.value = Mathf.Lerp(originalValue2, valueToReach, delta);

            delta += Time.deltaTime * speed;
            yield return null;
        }

        yield break;
    }

    void OnPlayerRoll(PlatformController controller)
    {
        _controller = (StarterCharacterMovement)controller;
        _isRollOnCooldown = true;
    }

    void OnWaveStarted(int waveIndex)
    {
        _currentRoundText.text = waveIndex.ToString();
    }

    void OnWaveCleared(int waveIndex)
    {
        _currentRoundText.text = "Awaiting Altar interaction...";
    }

    public void SetImplantPanel(bool active)
    {
        if (active)
        {
            if (implantPanel != null)
            {
                implantPanel.SetActive(true);
            }
        }
        else
        {
            if (implantPanel != null)
            {
                ImplantButton[] buttons = implantPanel.GetComponentsInChildren<ImplantButton>();

                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].DisableButton();
                }

                Animator animator = implantPanel.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("disable");
                    Invoke("DisableUpgradePanel", 1f);
                }
                else
                {
                    DisableImplantPanel();
                }
            }
        }
    }

    public void SetAbility(int slot, WeaponInformations informations)
    {
        if(slot == 0)
        {
            _firstAbilityCooldownData = informations.gameObject.GetComponent<CooldownManager>();
            _firstAbility = informations.gameObject.GetComponent<PlayerAbility>();


            float size = (_firstAbility.GetAbilityCost() / 20) * 8;
            _firstAbilityCostSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 5);
            _firstAbilityCostSlider.maxValue = _firstAbility.GetAbilityCost();
        }
    }

    void DisableImplantPanel()
    {
        implantPanel.SetActive(false);
    }
}
