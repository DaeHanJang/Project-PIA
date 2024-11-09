using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//result board object
public class Result : MonoBehaviour {
    private GameObject btnReplay = null;
    private AudioSource _as = null;
    private Text tRank = null;
    /*
    3: flappy bird
    4: angry bird
    5: galaga
    6: omok
    7: othello
    8: nightmare
    10: pifight
    ==============================
    0: logo
    1: title
    2: menu
    9: pifight lobby
    */
    private int buildindex; //current scence index

    private void Awake() {
        btnReplay = GameObject.Find("bReplay");
        _as = GetComponent<AudioSource>();
        buildindex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start() {
        SetResult();
        PIAManager.Inst.Save(); //save user data
        if (buildindex == 10) btnReplay.SetActive(false);
        tRank = GameObject.Find("txtRank").GetComponent<Text>();
        tRank.text = PIAManager.Inst.GetRankString(buildindex);
    }

    //set result board
    private void SetResult() {
        if (buildindex == 3) PIAManager.Inst.UpdateBest(buildindex, FlappyManager.Inst.Score);
        else if (buildindex == 4) PIAManager.Inst.UpdateBest(buildindex, AngryManager.Inst.Score);
        else if (buildindex == 5) PIAManager.Inst.UpdateBest(buildindex, GalagaManager.Inst.Score);
        else if (buildindex == 6) PIAManager.Inst.UpdateBest(buildindex, OmokManager.Inst.win, OmokManager.Inst.lose);
        else if (buildindex == 7) PIAManager.Inst.UpdateBest(buildindex, OthelloManager.Inst.win, OthelloManager.Inst.lose);
        else if (buildindex == 8) PIAManager.Inst.UpdateBest(buildindex, NightmareManager.Inst.Score);
        else if (buildindex == 10) PIAManager.Inst.UpdateBest(buildindex, PIFightManager.Inst.win, PIFightManager.Inst.lose);

        var list_name = new ArrayList();
        var list_score = new ArrayList();
        var list_score2 = new ArrayList();
        string out_name;
        int out_score, out_score2;

        //get record
        if (buildindex == 3 || buildindex == 4 || buildindex == 5 || buildindex == 8) {
            for (int i = 0; i < 10; i++) {
                PIAManager.Inst.GetData(buildindex, i, out out_name, out out_score);
                list_name.Add(out_name);
                list_score.Add(out_score);
            }
        }
        else {
            for (int i = 0; i < 10; i++) {
                PIAManager.Inst.GetData(buildindex, i, out out_name, out out_score, out out_score2);
                list_name.Add(out_name);
                list_score.Add(out_score);
                list_score2.Add(out_score2);
            }
        }

        //update record
        for (int i = 0; i < 10; i++) {
            if (buildindex == 3) {
                if (FlappyManager.Inst.Score > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, FlappyManager.Inst.Score);
                    break;
                }
            }
            else if (buildindex == 4) {
                if (AngryManager.Inst.Score > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, AngryManager.Inst.Score);
                    break;
                }
            }
            else if (buildindex == 5) {
                if (GalagaManager.Inst.Score > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, GalagaManager.Inst.Score);
                    break;
                }
            }
            else if (buildindex == 6) {
                if (OmokManager.Inst.win > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, OmokManager.Inst.win);
                    list_score2.Insert(i, OmokManager.Inst.lose);
                    break;
                }
                else if (OmokManager.Inst.win == (int)list_score[i]) {
                    if (OmokManager.Inst.lose <= (int)list_score[i]) {
                        list_name.Insert(i, PIAManager.Inst.Name);
                        list_score.Insert(i, OmokManager.Inst.win);
                        list_score2.Insert(i, OmokManager.Inst.lose);
                        break;
                    }
                }
            }
            else if (buildindex == 7) {
                if (OthelloManager.Inst.win > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, OthelloManager.Inst.win);
                    list_score2.Insert(i, OthelloManager.Inst.lose);
                    break;
                }
                else if (OthelloManager.Inst.win == (int)list_score[i]) {
                    if (OthelloManager.Inst.lose <= (int)list_score[i]) {
                        list_name.Insert(i, PIAManager.Inst.Name);
                        list_score.Insert(i, OthelloManager.Inst.win);
                        list_score2.Insert(i, OthelloManager.Inst.lose);
                        break;
                    }
                }
            }
            else if (buildindex == 8) {
                if (NightmareManager.Inst.Score > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, NightmareManager.Inst.Score);
                    break;
                }
            }
            else if (buildindex == 10) {
                if (PIFightManager.Inst.win > (int)list_score[i]) {
                    list_name.Insert(i, PIAManager.Inst.Name);
                    list_score.Insert(i, PIFightManager.Inst.win);
                    list_score2.Insert(i, PIFightManager.Inst.lose);
                    break;
                }
                else if (PIFightManager.Inst.win == (int)list_score[i]) {
                    if (PIFightManager.Inst.lose <= (int)list_score[i]) {
                        list_name.Insert(i, PIAManager.Inst.Name);
                        list_score.Insert(i, PIFightManager.Inst.win);
                        list_score2.Insert(i, PIFightManager.Inst.lose);
                        break;
                    }
                }
            }
        }

        //update record data
        for (int i = 0; i < 10; i++) {
            if (buildindex == 3 || buildindex == 4 || buildindex == 5 || buildindex == 8) PIAManager.Inst.SetData(buildindex, i, (string)list_name[i], (int)list_score[i]);
            else PIAManager.Inst.SetData(buildindex, i, (string)list_name[i], (int)list_score[i], (int)list_score2[i]);
        }

        //save record
        PIAManager.Inst.SaveRecord(buildindex);
    }

    //result board button
    public void onClick(int next) {
        if (next == 0) next = buildindex;

        _as.Play();
        if (buildindex == 3) GameObject.Find("FlappyManager").SendMessage("LoadScene", next);
        else if (buildindex == 4) GameObject.Find("AngryManager").SendMessage("LoadScene", next);
        else if (buildindex == 5) GameObject.Find("GalagaManager").SendMessage("LoadScene", next);
        else if (buildindex == 6) GameObject.Find("OmokManager").SendMessage("LoadScene", next);
        else if (buildindex == 7) GameObject.Find("OthelloManager").SendMessage("LoadScene", next);
        else if (buildindex == 8) GameObject.Find("NightmareManager").SendMessage("LoadScene", next);
        else if (buildindex == 10) GameObject.Find("PIFightManager").SendMessage("BtnBack");
    }
}
