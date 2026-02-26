using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float _fireCooldown = 0.3f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ushort _poolSize = 10;
        [SerializeField] private GameObject _bulletsContainer;
        [SerializeField] private Bullet _bullet;

        private int _current;
        private List<Bullet> _bulletsPool;
        private WaitForSeconds _cooldownRoutine;

        public bool IsReady { get; private set; }

        private void Awake()
        {
            _current = 0;
            _bulletsPool = new List<Bullet>(_poolSize);
            _cooldownRoutine = new WaitForSeconds(_fireCooldown);

            for (int i = 0; i < _poolSize; i++)
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
            Bullet bullet = _bulletsPool[_current++];
            bullet.gameObject.SetActive(false);

            if (_current == _poolSize)
            {
                _current = 0;
            }

            return bullet;
        }

        private IEnumerator CooldownRoutine()
        {
            yield return _cooldownRoutine;

            IsReady = true;
        }
    }
}
