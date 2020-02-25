using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEditorInternal;
// #endif

namespace BirdSight {
    public class GameMgr : MonoBehaviour
    {
        private const float MinDistance = 10, MaxDistance = 23;

        static private ParticleSystem shootblastPrefab;
        static List<ParticleSystem> shootBlastPool;
        static List<ParticleSystem> shootBlastAlives;
        static public void InstantiateEffect(Vector3 position)
        {
            if (shootBlastPool == null) shootBlastPool = new List<ParticleSystem>();
            if (shootBlastAlives == null) shootBlastAlives = new List<ParticleSystem>();

            ParticleSystem effect;
            if (shootBlastPool.Count > 0)
            {
                effect = shootBlastPool[0];
                shootBlastPool.RemoveAt(0);
                effect.gameObject.SetActive(true);
            }
            else
            {
                if (shootblastPrefab == null) shootblastPrefab = Resources.Load<ParticleSystem>("ShootBlast");
                effect = Instantiate(shootblastPrefab);
            }

            effect.transform.position = position;
            effect.Play();
            shootBlastAlives.Add(effect);
        }

        static private void PutEffect(ParticleSystem effect) {
            effect.Stop();
            effect.Clear();
            effect.gameObject.SetActive(false);
            shootBlastPool.Add(effect);
        }

        static public GameMgr mgr;

        [SerializeField]
        GameObject editorGlobalLight;

    #region Enemy spawning
        EnemySpawnPoint[] spawnPoints;
        Timer enemySpawnTimer;
        int enemySpawnNum = 1, enemySpawnCount = 0, enemyKillCount = 0;
        public int EnemyKillCount {
            get { return enemyKillCount; }
            set { enemyKillCount = value; enemyKillCountText.text = value.ToString(); } }
        public float SpeedMultiplier { get { return Mathf.Lerp(1, 3, enemySpawnCount / 100); } }
    #endregion

        public PlayerController player;
        Vector3 playerOriginPosition;

    #region Camera
        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        CinemachineBasicMultiChannelPerlin cameraNoise;
    #endregion

    #region UI control
        [SerializeField]
        RectTransform healthFill;
        float fillFullWidth;
        [SerializeField]
        TextMeshProUGUI enemyKillCountText;
        [SerializeField]
        Menu.PauseMenu pauseMenu;
        [SerializeField]
        Menu.MainMenu mainMenu;
    #endregion

        private void Awake() {
            mgr = this;
            shootBlastAlives = new List<ParticleSystem>();

            fillFullWidth = healthFill.sizeDelta.x;

            enemySpawnTimer = new Timer(2);
            spawnPoints = FindObjectsOfType<EnemySpawnPoint>();
            Destroy(editorGlobalLight);

            cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            playerOriginPosition = player.transform.position;
        }

        private void Start() {
            EnemyKillCount = 0;
            player.enabled = enabled = false;
            healthFill.parent.gameObject.SetActive(false);
            enemyKillCountText.gameObject.SetActive(false);
        }

        public void GameRestart() {
            enemySpawnNum = 1;
            enemySpawnCount = 0;
            EnemyKillCount = 0;
            player.transform.position = playerOriginPosition;

            healthFill.parent.gameObject.SetActive(true);
            enemyKillCountText.gameObject.SetActive(true);

            player.Reset();

            Enemy[] enemies = FindObjectsOfType<Enemy>();
            for (int i = 0; i < enemies.Length; i++) Destroy(enemies[i].gameObject);

            ResumeGame();
        }

        public void PauseGame() {
            Time.timeScale = 0;
            enabled = player.enabled = false;
            pauseMenu.Activate();
        }

        public void ResumeGame() {
            Time.timeScale = 1;
            enabled = player.enabled = true;
            pauseMenu.Deactivate();
        }

        private void FixedUpdate() {
            for (int i = 0; i < shootBlastAlives.Count; i++) {
                if (!shootBlastAlives[i].IsAlive()) {
                    PutEffect(shootBlastAlives[i]);
                    shootBlastAlives.RemoveAt(i);
                }
            }
 
            if (enemySpawnTimer.FixedUpdateEnd) {
                enemySpawnTimer = new Timer(Random.Range(
                    Mathf.Lerp(8, 2, 1 - (enemySpawnCount / 100)),
                    Mathf.Lerp(15, 5, 1 - (enemySpawnCount / 100))
                ));

                List<EnemySpawnPoint> inRangePoints = new List<EnemySpawnPoint>();
                for (int i = 0; i < spawnPoints.Length; i++) {
                    float dis = (spawnPoints[i].transform.position - player.transform.position).sqrMagnitude;
                    if (dis > (MinDistance * MinDistance) && dis < (MaxDistance * MaxDistance)) {
                        inRangePoints.Add(spawnPoints[i]);
                    }
                }

                if (inRangePoints.Count > 0) inRangePoints.Add(spawnPoints[0]);

                for (int i = 0; i < enemySpawnNum; i++) inRangePoints[Random.Range(0, inRangePoints.Count)].SpawnEnemy();

                enemySpawnCount++;
                if (enemySpawnCount % 15 == 0) enemySpawnNum++;
            }
        }

        public void GameOver() {
            if (!enabled) return;

            player.enabled = enabled = false;

            mainMenu.Canvas.enabled = true;
            mainMenu.FadeIn();
        }

        public void BackToMainMenu() {
            player.enabled = enabled = false;

            healthFill.parent.gameObject.SetActive(false);
            enemyKillCountText.gameObject.SetActive(false);

            player.transform.position = playerOriginPosition;

            Enemy[] enemies = FindObjectsOfType<Enemy>();
            for (int i = 0; i < enemies.Length; i++) Destroy(enemies[i].gameObject);

            if (pauseMenu.enabled) {
                Time.timeScale = 1;
                pauseMenu.Deactivate();
            }

            mainMenu.Canvas.enabled = true;
            mainMenu.FadeIn();
        }

        public IEnumerator ShakeCamera(float shakeIntensity = 5f, float shakeTiming = 0.5f)
        {
            Noise(1, shakeIntensity);
            yield return new WaitForSeconds(shakeTiming);
            Noise(0, 0);
        }

        public void Noise(float amplitudeGain, float frequencyGain)
        {
            cameraNoise.m_AmplitudeGain = amplitudeGain;
            cameraNoise.m_FrequencyGain = frequencyGain;

        }

        public void UpdateHealthBar(float percentage) {
            healthFill.sizeDelta = new Vector2(percentage * fillFullWidth, healthFill.sizeDelta.y);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(player.transform.position, MinDistance);
            Gizmos.DrawWireSphere(player.transform.position, MaxDistance);
        }

    // #if UNITY_EDITOR
    //     [CustomEditor(typeof(GameMgr))]
    //     public class _Editor : Editor {
    //         GameMgr mgr;
    //         ReorderableList spawnPoints;
    //         bool showHandle;

    //         GUIStyle buttonPressed;

    //         private void SetupStyle() {
    //             if (buttonPressed != null) return;

    //             buttonPressed = new GUIStyle("Button");
    //             buttonPressed.padding = new RectOffset(0, 0, 5, 5);
    //             buttonPressed.margin = new RectOffset(0, 0, 0, 0);
    //         }

    //         private void OnEnable() {
    //             mgr = (GameMgr) target;

    //             spawnPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("spawnPoints"));
    //             spawnPoints.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Enemy Spawn Points");
    //             spawnPoints.drawElementCallback = (rect, index, _a, _b) => {
    //                 EditorGUI.PropertyField(rect,
    //                     spawnPoints.serializedProperty.GetArrayElementAtIndex(index),
    //                     GUIContent.none,
    //                     false);
    //             };
    //         }

    //         public override void OnInspectorGUI() {
    //             SetupStyle();

    //             serializedObject.Update();

    //             EditorGUILayout.PropertyField(serializedObject.FindProperty("editorGlobalLight"));
    //             spawnPoints.DoLayoutList();

    //             showHandle = GUILayout.Toggle(showHandle, new GUIContent("Show Handle"), buttonPressed);

    //             serializedObject.ApplyModifiedProperties();
    //         }
    //     }
    // #endif
    }
}