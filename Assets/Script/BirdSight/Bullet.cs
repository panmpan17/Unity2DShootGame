using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdSight {
    public class Bullet : MonoBehaviour
    {
        static private Bullet bulletPrefab;
        static List<Bullet> bulletsPool;
        static public Bullet Instantiate() {
            if (bulletsPool == null) bulletsPool = new List<Bullet>();

            if (bulletsPool.Count > 0) {
                Bullet bullet = bulletsPool[0];
                bulletsPool.RemoveAt(0);
                bullet.gameObject.SetActive(true);
                return bullet;
            }
            else {
                if (bulletPrefab == null) bulletPrefab = Resources.Load<Bullet>("Bullet");
                return Instantiate(bulletPrefab);
            }
        }

        static private void Put(Bullet bullet) {
            bullet.gameObject.SetActive(false);
            bulletsPool.Add(bullet);
        }

        [SerializeField]
        int damage;
        float lifeCount;

        public void Setup(Vector3 position, Vector2 velocity) {
            transform.position = position;
            GetComponent<Rigidbody2D>().velocity = velocity;
            lifeCount = 0;
        }

        private void FixedUpdate() {
            lifeCount += Time.fixedDeltaTime;
            if (lifeCount > 1.5f) {
                Put(this);
            }
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.collider.CompareTag("Enemy")) {
                Enemy enemy = other.collider.GetComponent<Enemy>();
                enemy.OnDamage(transform, damage);
            }

            GameMgr.InstantiateEffect(other.contacts[0].point);

            Put(this);
        }
    }
}