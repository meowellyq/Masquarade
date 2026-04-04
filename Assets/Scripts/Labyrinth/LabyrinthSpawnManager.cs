using UnityEngine;
using Core;

namespace Labyrinth
{
    public class LabyrinthSpawnManager : MonoBehaviour
    {
        [Header("Игрок")]
        public Transform player;

        [Header("Точки спавна")]
        [Tooltip("Стандартный вход — у фонтана")]
        public Transform spawnDefault;

        [Tooltip("После Зала Гнева — рядом с беседкой")]
        public Transform spawnAfterWrath;

        [Tooltip("После Scene16/18 — у входа в Зал Печали")]
        public Transform spawnAfterEcho;

        void Start()
        {
            ApplySpawnPoint();
        }

        void ApplySpawnPoint()
        {
            if (player == null)
            {
                Debug.LogWarning("[SpawnManager] Игрок не назначен!");
                return;
            }

            if (GameStateManager.Instance == null)
            {
                Debug.LogWarning("[SpawnManager] GameStateManager не найден, используем default.");
                MoveToSpawn(spawnDefault);
                return;
            }

            string id = GameStateManager.Instance.spawnPointId;
            Debug.Log($"[SpawnManager] Применяем точку спавна: {id}");

            switch (id)
            {
                case "after_wrath":
                    MoveToSpawn(spawnAfterWrath);
                    break;

                case "after_echo":
                    MoveToSpawn(spawnAfterEcho);
                    break;

                case "return_position":
                    // Возвращаем игрока точно туда откуда он ушёл в мини-игру
                    Vector3 returnPos = GameStateManager.Instance.labyrinthReturnPosition;
                    if (returnPos != Vector3.zero)
                    {
                        player.position = returnPos;
                        Debug.Log($"[SpawnManager] Игрок возвращён в сохранённую позицию: {returnPos}");
                    }
                    else
                    {
                        Debug.LogWarning("[SpawnManager] return_position равна zero, используем default.");
                        MoveToSpawn(spawnDefault);
                    }
                    break;

                default: // "default"
                    MoveToSpawn(spawnDefault);
                    break;
            }

            // Сбрасываем на default после применения
            GameStateManager.Instance.spawnPointId = "default";
        }

        void MoveToSpawn(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("[SpawnManager] Точка спавна не назначена в Inspector!");
                return;
            }
            player.position = target.position;
            Debug.Log($"[SpawnManager] Игрок перемещён в {target.name} ({target.position})");
        }
    }
}