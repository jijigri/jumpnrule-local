using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] private GameObject _gunObject = null;
    [SerializeField] private TextMeshProUGUI _nameTextMesh = null;
    //[SerializeField] private TextMeshProUGUI _descriptionTextMesh = null;
    [SerializeField] private Text _descriptionTextMesh = null;
    [SerializeField] private TextMeshProUGUI _pickupInputTextMesh = null;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private WorldUIPanel _worldUIPanel = null;

    private string _name;
    private string _description;
    private Sprite _sprite;

    GameObject _weapon;

    private bool _allowPickup = false;

    private WeaponInformations _weaponInformations;

    PlayerWeaponManager _playerGunHandler;

    private void Awake()
    {
        _weapon = Instantiate(_gunObject, transform.position, Quaternion.identity);
        _weapon.SetActive(false);
        InitializeGunItem(_weapon);
    }

    private void Start()
    {
        HideText();
    }

    public void InitializeGunItem(GameObject weaponObject)
    {
        if(weaponObject != null)
        {
            _weapon = weaponObject;

            weaponObject.transform.SetParent(this.transform);

            _weaponInformations = weaponObject.GetComponent<WeaponInformations>();
            SetGun();
        }
    }
    public void SetGun()
    {
        if (_weaponInformations != null)
        {
            _nameTextMesh.text = _name = _weaponInformations.name;
            _descriptionTextMesh.text = _description = _weaponInformations.description.ToLower();
            _spriteRenderer.sprite = _sprite = _weaponInformations.sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_playerGunHandler == null)
            {
                _playerGunHandler = collision.gameObject.GetComponent<PlayerWeaponManager>();
            }

            _allowPickup = true;
            DisplayText();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _allowPickup = false;
            HideText();
        }
    }

    private void Update()
    {
        if (_allowPickup)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("ADD GUN");
                if (_playerGunHandler != null)
                {
                    _playerGunHandler.AddGunToInventory(_weapon);
                }

                Destroy(gameObject);
            }
        }
    }

    void DisplayText()
    {
        /*
        _nameTextMesh.gameObject.SetActive(true);
        _descriptionTextMesh.gameObject.SetActive(true);
        _pickupInputTextMesh.gameObject.SetActive(true);
        */

        _worldUIPanel.gameObject.SetActive(false);
        _worldUIPanel.gameObject.SetActive(true);
    }

    void HideText()
    {
        /*
        _nameTextMesh.gameObject.SetActive(false);
        _descriptionTextMesh.gameObject.SetActive(false);
        _pickupInputTextMesh.gameObject.SetActive(false);
        */

        _worldUIPanel.ClosePanel();
    }
}
