using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _weaponItemPrefab;

    //float _aimAngle = 0;

    Camera _mainCamera;

    Gun _selectedWeapon;

    Gun[] _gunsInInventory = new Gun[3];

    List<Transform> _weaponSlots = new List<Transform>();
    Transform _weaponHolder;

    int _currentWeaponSlot = 0;
    int _lastWeaponSlot = 0;

    PlayerAnimationManager animationManager;

    private void Awake()
    {
        animationManager = GetComponent<PlayerAnimationManager>();

        _weaponHolder = transform.Find("WeaponHolder");
        _selectedWeapon = _weaponHolder.GetChild(0).GetChild(0).GetComponent<Gun>(); //Placeholder way to find the current gun
        _gunsInInventory[0] = _selectedWeapon;

        for (int i = 0; i < _weaponHolder.transform.childCount; i++)
        {
            Transform child = _weaponHolder.transform.GetChild(i);

            if (child.name.Contains("Slot"))
            {
                _weaponSlots.Add(_weaponHolder.transform.GetChild(i));
            }
        }
    }

    void Start()
    {
        _mainCamera = Camera.main;

        SwitchGun(0);
    }

    void SwitchGun(int slotIndex)
    {
        if (slotIndex > _weaponSlots.Count - 1 || slotIndex < 0)
        {
            return;
        }

        if(_gunsInInventory[slotIndex] == null)
        {
            return;
        }

        for(int i = 0; i < _gunsInInventory.Length; i++)
        {
            if (_gunsInInventory[i] != null)
            {
                if (i == slotIndex)
                {
                    Gun weapon = _gunsInInventory[i];

                    if (weapon != null)
                    {
                        _gunsInInventory[i].gameObject.SetActive(true);
                        _selectedWeapon = weapon;

                        weapon.OnSwitch(true);
                    }
                }
                else
                {
                    _gunsInInventory[i].OnSwitch(false);
                }
            }
        }
    }

    private void Update()
    {
        RotateGun();

        GetSwitchInput();

        GetGunInput();
    }

    void GetSwitchInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeaponSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeaponSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeaponSlot(2);
        }

        if (Input.mouseScrollDelta.y < -.1f)
        {
            _lastWeaponSlot = _currentWeaponSlot;
            _currentWeaponSlot--;

            if (_currentWeaponSlot < 0)
            {
                _currentWeaponSlot = _weaponSlots.Count - 1;
            }

            SwitchGun(_currentWeaponSlot);
        }
        else if (Input.mouseScrollDelta.y > .1f)
        {
            _lastWeaponSlot = _currentWeaponSlot;
            _currentWeaponSlot++;

            if (_currentWeaponSlot > _weaponSlots.Count - 1)
            {
                _currentWeaponSlot = 0;
            }

            SwitchGun(_currentWeaponSlot);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeWeaponSlot(_lastWeaponSlot);
        }
    }

    void ChangeWeaponSlot(int weaponSlot)
    {
        if (_currentWeaponSlot != weaponSlot)
        {
            _lastWeaponSlot = _currentWeaponSlot;
            _currentWeaponSlot = weaponSlot;
            SwitchGun(_currentWeaponSlot);
        }
    }

    void GetGunInput()
    {
        if(_selectedWeapon.gameObject.activeInHierarchy == false)
        {
            Debug.Log("NO GUN");
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _selectedWeapon.OnPrimaryPressed();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            _selectedWeapon.OnPrimaryReleased();
        }
        if (Input.GetButton("Fire1"))
        {
            _selectedWeapon.OnPrimary();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _selectedWeapon.OnSecondaryPressed();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            _selectedWeapon.OnSecondaryReleased();
        }
        if (Input.GetButton("Fire2"))
        {
            _selectedWeapon.OnSecondary();
        }
    }

    public void AddGunToInventory(GameObject weaponToAdd)
    {
        int weaponSlot = GetSlotToPutWeaponIn();

        if (GetWeaponInSlot(weaponSlot).Item2 == null)
        {
            Debug.Log("No gun found in this slot, adding the new gun");

            _currentWeaponSlot = weaponSlot;
            _selectedWeapon = InstantiateNewGun(weaponToAdd, weaponSlot);
            SwitchGun(weaponSlot);
        }
        else
        {
            //Creates a weapon item holding the gun to replace
            CreateWeaponItem(_selectedWeapon.gameObject);

            //Destroys the gun in inventory since the item has been created
            //Destroy(GetWeaponInSlot(weaponSlot).Item1);
            RemoveGunFromInventory(weaponSlot);

            //Adds the new gun to the inventory
            _currentWeaponSlot = weaponSlot;
            _selectedWeapon = InstantiateNewGun(weaponToAdd, weaponSlot);
            SwitchGun(weaponSlot);
        }
    }

    Gun InstantiateNewGun(GameObject weaponObject, int weaponSlot)
    {
        Gun gun = null;

        Transform slot = _weaponHolder.GetChild(weaponSlot);

        if(slot != null)
        {
            weaponObject.transform.position = slot.transform.position;
            weaponObject.transform.SetParent(slot.transform);

            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            //Get the angle between the points
            float angle = Helper.AngleBetweenTwoPoints(transform.position, mousePos);
            weaponObject.transform.rotation = Quaternion.Euler(0, 0, angle + 180);

            weaponObject.SetActive(false);
            weaponObject.SetActive(true);

            gun = weaponObject.GetComponent<Gun>();

            _gunsInInventory[weaponSlot] = gun;

            Debug.Log(string.Format("Added new gun ({0}) to inventory", weaponObject.name));
        }
        else
        {
            Debug.LogError("Error: Couldn't add a gun to the current slot because slot " + weaponSlot + " does not exist");
        }

        return gun;
    }

    void RemoveGunFromInventory(int slot)
    {
        _gunsInInventory[slot].gameObject.SetActive(false);
        _gunsInInventory[slot] = null;
    }


    void CreateWeaponItem(GameObject weaponObject)
    {
        GameObject weaponItem = Instantiate(_weaponItemPrefab, transform.position, Quaternion.identity);
        weaponItem.GetComponent<WeaponItem>().InitializeGunItem(weaponObject);
    }

    int GetSlotToPutWeaponIn()
    {
        //Check if there is a free slot available
        for(int i = 0; i < _weaponSlots.Count; i++)
        {
            if(_weaponSlots[i].childCount < 1)
            {
                _lastWeaponSlot = _currentWeaponSlot;
                Debug.Log("Free slot here");
                return i;
            }
        }

        //If there isn't, return the current weapon slot
        return _currentWeaponSlot;
    }


    //Return a gun Object and its Gun component
    (GameObject, Gun) GetWeaponInSlot(int slotIndex)
    {
        if (_gunsInInventory[slotIndex] != null)
        {
            return (_gunsInInventory[slotIndex].gameObject, _gunsInInventory[slotIndex]);
        }
        else
        {
            return (null, null);
        }
    }

    void RotateGun()
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = Helper.AngleBetweenTwoPoints(transform.position, mousePos);

        _weaponHolder.transform.rotation = Quaternion.Euler(0, 0, angle + 180);


        if (_selectedWeapon != null)
        {
            _selectedWeapon.Rotate(angle);
        }

        if (animationManager != null)
        {
            animationManager.Rotate(angle);
        }
    }

    public List<Transform> GetWeaponSlots()
    {
        return _weaponSlots;
    }
}
