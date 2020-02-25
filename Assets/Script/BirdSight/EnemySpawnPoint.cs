using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdSight {
    public class EnemySpawnPoint : MonoBehaviour
    {
        private Timer timer;

        public void SpawnEnemy() {
            Enemy enemy = Enemy.Instantiate();
            enemy.Setup(transform.position);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}