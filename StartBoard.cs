using UnityEngine;
using UnityEngine.SceneManagement;

//게임 시작 UI
public class StartBoard : MonoBehaviour {
    private int buildindex; //current scence index

    private void Awake() { buildindex = SceneManager.GetActiveScene().buildIndex; }

    public void SetTurn(int n) {
        if (buildindex == 6) OmokManager.Inst.SetTurn(n);
        else if (buildindex == 7) OthelloManager.Inst.SetTurn(n);
    }
}
