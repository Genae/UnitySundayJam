using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "SO/Weapon", order = 1)]
    public class WeaponBase : ScriptableObject
    {
        public bool Automatic;
        public float Cooldown;
        public int MaxAmmunition;
        public int CurrentAmmunition;

        public AudioClip[] FireSounds;

        public GameObject bulletPrefab;
        public PlayerController player;

        protected bool _shooting;
        protected float _currentCooldown;

        public void ButtonDown()
        {
            if (Automatic)
            {
                _shooting = true;
            }
            else
                DoShooting();
        }

        public void ButtonUp()
        {
            _shooting = false;
        }

        public void RunUpdate(float deltaTime)
        {
            DoAutomaticShooting(deltaTime);
        }

        private void DoAutomaticShooting(float deltaTime)
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= deltaTime;
                return;
            }

            if (CurrentAmmunition == 0)
            {
                return;
            }

            if (!_shooting)
                return;

            _currentCooldown = Cooldown;
            player.CmdFire();
        }

        private void DoShooting()
        {
            player.CmdFire();
        }

        
    }
}
