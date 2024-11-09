using UnityEngine;

//��ֹ� spawner
public class ObstaclePool : MonoBehaviour {
    private GameObject prefColumn; //��ֹ� prefab
    private GameObject[] columns; //��ֹ� objects
    private int colPoolSize = 5; //��ֹ� ����
    private int currentColIndex = 0; //���� ��ֹ� ��ȣ
    private float colSpawnRate = 3f; //��ֹ� spawn �ֱ�
    private float spawnXPosition = 10f; //spawn�� ��ֹ��� x��ǥ
    private float colYPositionMax = 3f; //spawn�� ��ֹ��� y�� �ִ� ��ǥ
    private float colYPositionMin = -0.5f; //spanw�� ��ֹ��� y�� �ּ� ��ǥ

    private void Awake() { prefColumn = Resources.Load("FlappyBird/Columns") as GameObject; }

    //��ֹ� data �ʱ�ȭ
    public void InitColumnCreate() {
        columns = new GameObject[colPoolSize];
        for (int i = 0; i < columns.Length; i++) columns[i] = Instantiate(prefColumn, new Vector2(-15, -25), Quaternion.identity);
        InvokeRepeating("Spawn", 0f, colSpawnRate);
    }

    //��ֹ� ����
    private void Spawn() {
        if (FlappyManager.Inst.isGameOver) return;

        float _y_position = Random.Range(colYPositionMin, colYPositionMax);
        columns[currentColIndex].transform.position = new Vector2(spawnXPosition, _y_position);
        currentColIndex = (currentColIndex + 1) % colPoolSize;
    }
}
