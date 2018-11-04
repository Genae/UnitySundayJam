using System.Linq;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public WeaponBase WeaponToEquip;
    private WeaponBase _equipedWeapon;

    private float _playerSpeed = 6;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * _playerSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * _playerSpeed;
        if (WeaponToEquip != null)
        {
            EquipWeapon(WeaponToEquip);
            WeaponToEquip = null;
        }



        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30 * Time.deltaTime);
        }

        transform.Translate(x, 0, z, Space.World);

        if (Input.GetMouseButtonDown(0))
        {
            _equipedWeapon.ButtonDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _equipedWeapon.ButtonUp();
        }

        _equipedWeapon.RunUpdate(Time.deltaTime);
    }

    public void EquipWeapon(WeaponBase weaponBase)
    {
        _equipedWeapon = weaponBase;
        _equipedWeapon.player = this;
        this.bulletPrefab = weaponBase.bulletPrefab;
    }


    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        EquipWeapon(Resources.FindObjectsOfTypeAll<WeaponBase>().First());
    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    public void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject) Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}