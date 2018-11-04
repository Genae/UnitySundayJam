using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    class CollectableWeapon : Collectable
    {
        public WeaponBase Weapon;
        
        public override void CollectedBy(PlayerController player)
        {
            player.EquipWeapon(Weapon);
        }
    }
}
