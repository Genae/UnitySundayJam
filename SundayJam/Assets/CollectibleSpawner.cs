using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

public class CollectibleSpawner : NetworkBehaviour
{
    public float Cooldown = 10;
    public float CurrentCooldown;
    public Collectable[] AllCollectables;

    void Start()
    {
        AllCollectables = Resources.LoadAll<Collectable>("Collectable");
        CurrentCooldown = Cooldown;
    }

	// Update is called once per frame
	void Update ()
	{
	    if (!isServer)
	        return;
	    if (CurrentCooldown > 0)
	    {
	        CurrentCooldown -= Time.deltaTime;
	        return;
	    }
        

	    CurrentCooldown = Cooldown;
	    CmdSpawnRandomBooster();
	}

    [Command]
    private void CmdSpawnRandomBooster()
    {
        Debug.Log("Spawn");
        var collectable = AllCollectables[Random.Range(0, AllCollectables.Length)].gameObject;
        var obj = Instantiate(collectable, new Vector3(Random.Range(-40f, 40f), 1, Random.Range(-20f, 20f)), Quaternion.identity);
        NetworkServer.Spawn(obj);
    }
}
