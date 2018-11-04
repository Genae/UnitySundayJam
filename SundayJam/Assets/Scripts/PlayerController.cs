using System.Linq;
using System.Text;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public Material LocalPlayerMaterial;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public WeaponBase WeaponToEquip;
    private WeaponBase _equipedWeapon;
    private AudioClip _fireSound;

    public float PlayerSpeed = 6;

    [SyncVar(hook = "OnChangeInvisible")]
    public bool IsInvisible = true;

    private float _timer;

    public string PlayerName = "Dummy";
    public int Kills = 0;
    public int Deaths = 0;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //visibility
        if (!IsInvisible)
        {
            _timer += Time.deltaTime;
        }
        if(_timer > 2)
        {
            IsInvisible = true;
            _timer = 0;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed;
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
        _fireSound = weaponBase.fireSound;
    }

    public void OnChangeInvisible(bool invisible)
    {
        IsInvisible = invisible;
        if (!isLocalPlayer)
        {
            Debug.Log(invisible + ", " + IsInvisible);
            if (IsInvisible)
            {
                Renderer[] rs = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rs)
                    r.enabled = false;
            }
            else
            {
                Renderer[] rs = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rs)
                    r.enabled = true;
            }

        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        EquipWeapon(Resources.LoadAll<WeaponBase>("Items").First(w => !w.Automatic));
    }

    void Start()
    {
        OnChangeInvisible(true);
    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    public void CmdFire()
    {
        _timer = 0;
        IsInvisible = false;
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject) Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 30;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        AudioSource.PlayClipAtPoint(_fireSound,this.transform.position);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}