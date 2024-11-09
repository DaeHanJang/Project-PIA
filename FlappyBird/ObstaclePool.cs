using UnityEngine;

//장애물 spawner
public class ObstaclePool : MonoBehaviour {
    private GameObject prefColumn; //장애물 prefab
    private GameObject[] columns; //장애물 objects
    private int colPoolSize = 5; //장애물 갯수
    private int currentColIndex = 0; //현재 장애물 번호
    private float colSpawnRate = 3f; //장애물 spawn 주기
    private float spawnXPosition = 10f; //spawn될 장애물의 x좌표
    private float colYPositionMax = 3f; //spawn될 장애물의 y축 최대 좌표
    private float colYPositionMin = -0.5f; //spanw될 장애물의 y축 최소 좌표

    private void Awake() { prefColumn = Resources.Load("FlappyBird/Columns") as GameObject; }

    //장애물 data 초기화
    public void InitColumnCreate() {
        columns = new GameObject[colPoolSize];
        for (int i = 0; i < columns.Length; i++) columns[i] = Instantiate(prefColumn, new Vector2(-15, -25), Quaternion.identity);
        InvokeRepeating("Spawn", 0f, colSpawnRate);
    }

    //장애물 생성
    private void Spawn() {
        if (FlappyManager.Inst.isGameOver) return;

        float _y_position = Random.Range(colYPositionMin, colYPositionMax);
        columns[currentColIndex].transform.position = new Vector2(spawnXPosition, _y_position);
        currentColIndex = (currentColIndex + 1) % colPoolSize;
    }
}
