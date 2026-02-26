using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float _fireCooldown = 0.5f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ushort _initialPoolSize = 10;
        [SerializeField] private GameObject _bulletsContainer;
        [SerializeField] private Bullet _bullet;

        private int _current;
        private List<Bullet> _bulletsPool;
        private WaitForSeconds _cooldownRoutine;

        public bool IsReady { get; private set; }

        private void Awake()
        {
            _current = 0;
            _bulletsPool = new List<Bullet>(_initialPoolSize);
            _cooldownRoutine = new WaitForSeconds(_fireCooldown);

            for (int i = 0; i < _initialPoolSize; i++)
            {
                Bullet bullet = Instantiate(_bullet, _bulletsContainer.transform, true);
                bullet.gameObject.SetActive(false);

                _bulletsPool.Add(bullet);
            }

            IsReady = true;
        }

        public void Fire()
        {
            if (!IsReady)
            {
                return;
            }

            Bullet bullet = GetBullet();
            bullet.transform.position = _firePoint.position;
            bullet.transform.rotation = _firePoint.rotation;
            bullet.SetDirection(_firePoint.root.localScale.x > 0 ? Vector2.right : Vector2.left);
            bullet.gameObject.SetActive(true);

            StartCoroutine(CooldownRoutine());

            IsReady = false;
        }

        private Bullet GetBullet()
        {
            int initialCurrent = _current;

            Bullet bullet = _bulletsPool[_current];
            _current = ++_current % _bulletsPool.Count;

            if (bullet.IsFlying)
            {
                while (initialCurrent != _current)
                {
                    bullet = _bulletsPool[_current].IsFlying ? null : _bulletsPool[_current];
                    _current = ++_current % _bulletsPool.Count;
                }
            }

            if (!bullet || bullet.IsFlying)
            {
                bullet = Instantiate(_bullet, _bulletsContainer.transform, true);
                _bulletsPool.Add(bullet);
            }

            bullet.gameObject.SetActive(false);

            return bullet;
        }

        private IEnumerator CooldownRoutine()
        {
            yield return _cooldownRoutine;

            IsReady = true;
        }
    }
}
