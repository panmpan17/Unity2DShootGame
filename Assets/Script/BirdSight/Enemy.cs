using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdSight {
    public class Enemy : MonoBehaviour
    {
        static private Enemy Prefab;
        static List<Enemy> Pool;
        static public Enemy Instantiate()
        {
            if (Pool == null) Pool = new List<Enemy>();

            if (Pool.Count > 0)
            {
                Enemy enemy = Pool[0];
                Pool.RemoveAt(0);
                enemy.gameObject.SetActive(true);
                return enemy;
            }
            else
            {
                if (Prefab == null) Prefab = Resources.Load<Enemy>("Enemy");
                return Instantiate(Prefab);
            }
        }

        static private void Put(Enemy bullet) {
            bullet.gameObject.SetActive(false);
            Pool.Add(bullet);
        }

        [SerializeField]
        int health;
        int healthDamaged;
        [SerializeField]
        int damage;
        [SerializeField]
        float movingSpeed = 1;

        [SerializeField]
        float knockBackForce;
        [SerializeField]
        Timer knockBackTimer;
        bool knockback;

        new Rigidbody2D rigidbody2D;

        private void Awake() {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Setup(Vector3 pos) {
            transform.position = pos;
            healthDamaged = 0;
        }

        private void FixedUpdate() {
            if (knockback)
            {
                if (knockBackTimer.UpdateEnd) knockback = false;
                return;
            }

            Vector3 dis = GameMgr.mgr.player.transform.position - transform.position;
            rigidbody2D.velocity = dis.normalized * movingSpeed * GameMgr.mgr.SpeedMultiplier;
        }

        public void OnDamage(Transform other, int amount) {
            healthDamaged += amount;
            if (healthDamaged >= health) {
                Put(this);
                GameMgr.mgr.EnemyKillCount++;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                PlayerController playerCtl = other.collider.GetComponent<PlayerController>();

                if (playerCtl.Dashing) {
                    knockback = true;

                    Vector2 vec = (Vector2)transform.position - other.contacts[0].point;
                    rigidbody2D.velocity = vec.normalized * knockBackForce;
                    knockBackTimer.Reset();

                    OnDamage(playerCtl.transform, 2);
                }
                else playerCtl.OnDamage(transform, damage);
            }
        }

    }
}