using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>(); // Senjata yang dimiliki
    public Transform weaponHolder; // Tempat senjata dipegang (buat empty GameObject di tangan player)
    private GameObject currentWeapon; // Senjata yang sedang aktif

    // Tambah senjata ke inventory
    public bool AddWeapon(Weapon newWeapon)
    {
        // Cek apakah sudah punya senjata ini
        foreach (Weapon w in weapons)
        {
            if (w.weaponName == newWeapon.weaponName)
                return false; // Sudah ada, tidak diambil
        }

        weapons.Add(newWeapon);

        // Jika ini senjata pertama, langsung equip
        if (weapons.Count == 1)
            EquipWeapon(0);

        return true;
    }

    // Ganti senjata
    public void EquipWeapon(int index)
    {
        if (currentWeapon != null) Destroy(currentWeapon);
        if (index >= 0 && index < weapons.Count)
        {
            currentWeapon = Instantiate(weapons[index].weaponPrefab, weaponHolder);
        }

        // Beritahu PlayerFire untuk update state
        GetComponent<PlayerFire>().UpdateWeaponState();
    }

    void Update()
    {
        // Ganti senjata dengan tombol 1, 2, 3, dst.
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                EquipWeapon(i);
        }
    }

    public int GetCurrentWeaponDamage()
    {
        if (weapons.Count == 0) return 0;

        int currentIndex = weapons.FindIndex(w => w.weaponPrefab.name == currentWeapon?.name.Replace("(Clone)", "").Trim());

        if (currentIndex >= 0 && currentIndex < weapons.Count)
            return weapons[currentIndex].damage;

        return 0;
    }

}