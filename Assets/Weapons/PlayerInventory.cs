using System.Collections.Generic;
using UnityEngine;


public class WeaponInstance
{
    public Weapon weaponData;
    public int currentAmmo;

    public WeaponInstance(Weapon weapon)
    {
        weaponData = weapon;
        currentAmmo = weapon.maxAmmo;
    }
}


public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> weaponDataList = new List<Weapon>(); // Data senjata (ScriptableObjects)
    public List<WeaponInstance> weapons = new List<WeaponInstance>(); // WeaponInstance runtime
    public Transform weaponHolder; // Tempat senjata di tangan player
    private GameObject currentWeapon; // GameObject senjata yang sedang aktif
    private int currentWeaponIndex = 0;

    // Tambah senjata baru dari Weapon data
    public bool AddWeapon(Weapon newWeapon)
    {
        foreach (WeaponInstance w in weapons)
        {
            if (w.weaponData.weaponName == newWeapon.weaponName)
                return false; // Sudah ada
        }

        weapons.Add(new WeaponInstance(newWeapon));

        // Kalau ini senjata pertama langsung equip
        if (weapons.Count == 1)
            EquipWeapon(0);

        return true;
    }

    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        currentWeaponIndex = -1; // Tidak ada senjata aktif

        // Update animasi dan state senjata
        GetComponent<PlayerFire>().UpdateWeaponState();
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return;

        currentWeaponIndex = index;

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(weapons[index].weaponData.weaponPrefab, weaponHolder);

        GetComponent<PlayerFire>().UpdateWeaponState();
    }


    void Update()
    {
        // Tekan 0 untuk sembunyikan semua senjata
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UnequipWeapon();
        }

        // Ganti senjata dengan tombol 1, 2, 3, dst.
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
            }
        }
    }


    public WeaponInstance GetCurrentWeaponInstance()
    {
        if (weapons.Count == 0)
            return null;

        if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Count)
            return null;

        return weapons[currentWeaponIndex];
    }


    public int GetCurrentWeaponDamage()
    {
        WeaponInstance weapon = GetCurrentWeaponInstance();
        if (weapon == null)
            return 0;
        return weapon.weaponData.damage;
    }
}
