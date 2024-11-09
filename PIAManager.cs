using System.Collections.Generic;
using Management;
using UnityEngine;

/*
    build index
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

//data manager
public class PIAManager : GameManager<PIAManager> {
    public class Rating {
        string name;
        int win, lose;

        public Rating() {
            name = "";
            win = 0;
            lose = 0;
        }
        public Rating(string _name, int _win, int _lose) {
            name = _name;
            win = _win;
            lose = _lose;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public int Win {
            get { return win; }
            set { win = value; }
        }
        public int Lose {
            get { return lose; }
            set { lose = value; }
        }
    }

    private string gameVersion = "";

    private List<string> users = new List<string>();
    private string _name;
    private int[] _bestOm = new int[2];
    private int[] _bestOd = new int[2];
    private int[] _bestPF = new int[2];
    private int _bestF, _bestA, _bestG, _bestN;
    private List<KeyValuePair<string, int>> _recordF = new List<KeyValuePair<string, int>>();
    private List<KeyValuePair<string, int>> _recordA = new List<KeyValuePair<string, int>>();
    private List<KeyValuePair<string, int>> _recordG = new List<KeyValuePair<string, int>>();
    private List<Rating> _recordOm = new List<Rating>();
    private List<Rating> _recordOd = new List<Rating>();
    private List<KeyValuePair<string, int>> _recordN = new List<KeyValuePair<string, int>>();
    private List<Rating> _recordPF = new List<Rating>();
    private string defaultRecord = "none/0\nnone/0\nnone/0\nnone/0\nnone/0\nnone/0\nnone/0\nnone/0\nnone/0\nnone/0";
    private string defaultRecord2 = "none/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0\nnone/0/0";
    private string defaultScore = "0/0/0/0/0/0/0/0/0/0";
    private char[] delimiterChars = { '/', '\n' };

    public string Name {
        get { return _name; }
        set { _name = value; }
    }
    public int BestF { 
        get { return _bestF; }
        set { _bestF = value; }
    }
    public int BestA {
        get { return _bestA; }
        set { _bestA = value; }
    }
    public int BestG {
        get { return _bestG; }
        set { _bestG = value; }
    }
    public int[] BestOm {
        get { return _bestOm; }
        set { _bestOm = value; }
    }
    public int[] BestOd {
        get { return _bestOd; }
        set { _bestOd = value; }
    }
    public int BestN {
        get { return _bestN; }
        set { _bestN = value; }
    }
    public int[] BestPF {
        get { return _bestPF; }
        set { _bestPF = value; }
    }
    public string GameVersion {
        get { return gameVersion; }
    }

    protected override void Awake() {
        base.Awake();
        Application.targetFrameRate = 60;
        SetResolution(1600, 900, false);
        LoadGameVersion();
        LoadUsers();
    }

    private void LoadGameVersion() { gameVersion = PlayerPrefs.GetString("GameVersion", "0.1"); }

    //record data load
    private void LoadUsers() {
        string[] tmp = PlayerPrefs.GetString("UsersList", null).Split("\n");
        for (int i = 0; i < tmp.Length; i++) users.Add(tmp[i]);
        tmp = PlayerPrefs.GetString("RecordF", defaultRecord).Split(delimiterChars);
        for (int i = 0; i < 20; i += 2) {
            if (i < tmp.Length) _recordF.Add(new KeyValuePair<string, int>(tmp[i], int.Parse(tmp[i + 1])));
            else _recordF.Add(new KeyValuePair<string, int>("none", 0));
        }
        tmp = PlayerPrefs.GetString("RecordA", defaultRecord).Split(delimiterChars);
        for (int i = 0; i < 20; i += 2) {
            if(i < tmp.Length) _recordA.Add(new KeyValuePair<string, int>(tmp[i], int.Parse(tmp[i + 1])));
            else _recordF.Add(new KeyValuePair<string, int>("none", 0));
        }
        tmp = PlayerPrefs.GetString("RecordG", defaultRecord).Split(delimiterChars);
        for (int i = 0; i < 20; i += 2) {
            if(i < tmp.Length) _recordG.Add(new KeyValuePair<string, int>(tmp[i], int.Parse(tmp[i + 1])));
            else _recordF.Add(new KeyValuePair<string, int>("none", 0));
        }
        tmp = PlayerPrefs.GetString("RecordOm", defaultRecord2).Split(delimiterChars);
        for (int i = 0; i < 30; i += 3) {
            if (i < tmp.Length) _recordOm.Add(new Rating(tmp[i], int.Parse(tmp[i + 1]), int.Parse(tmp[i + 2])));
            else _recordOm.Add(new Rating("none", 0, 0));
        }
        tmp = PlayerPrefs.GetString("RecordOd", defaultRecord2).Split(delimiterChars);
        for (int i = 0; i < 30; i += 3) {
            if (i < tmp.Length) _recordOd.Add(new Rating(tmp[i], int.Parse(tmp[i + 1]), int.Parse(tmp[i + 2])));
            else _recordOd.Add(new Rating("none", 0, 0));
        }
        tmp = PlayerPrefs.GetString("RecordN", defaultRecord).Split(delimiterChars);
        for (int i = 0; i < 20; i += 2) {
            if (i < tmp.Length) _recordN.Add(new KeyValuePair<string, int>(tmp[i], int.Parse(tmp[i + 1])));
            else _recordN.Add(new KeyValuePair<string, int>("none", 0));
        }
        tmp = PlayerPrefs.GetString("RecordPF", defaultRecord2).Split(delimiterChars);
        for (int i = 0; i < 30; i += 3) {
            if (i < tmp.Length) _recordPF.Add(new Rating(tmp[i], int.Parse(tmp[i + 1]), int.Parse(tmp[i + 2])));
            else _recordPF.Add(new Rating("none", 0, 0));
        }
    }

    //user data load
    private void Load(string _id) {
        _name = _id;
        string[] tmp = PlayerPrefs.GetString(_name + "B", defaultScore).Split('/');
        _bestF = int.Parse(tmp[0]);
        _bestA = int.Parse(tmp[1]);
        _bestG = int.Parse(tmp[2]);
        _bestOm[0] = int.Parse(tmp[3]);
        _bestOm[1] = int.Parse(tmp[4]);
        _bestOd[0] = int.Parse(tmp[5]);
        _bestOd[1] = int.Parse(tmp[6]);
        _bestN = int.Parse(tmp[7]);
        _bestPF[0] = int.Parse(tmp[8]);
        _bestPF[1] = int.Parse(tmp[9]);
    }

    //set record data
    public void SetData(int buildindex, int idx, string name, int score, int score2 = 0) {
        if (buildindex == 3) _recordF[idx] = new KeyValuePair<string, int>(name, score);
        else if (buildindex == 4) _recordA[idx] = new KeyValuePair<string, int>(name, score);
        else if (buildindex == 5) _recordG[idx] = new KeyValuePair<string, int>(name, score);
        else if (buildindex == 6) _recordOm[idx] = new Rating(name, score, score2);
        else if (buildindex == 7) _recordOd[idx] = new Rating(name, score, score2);
        else if (buildindex == 8) _recordN[idx] = new KeyValuePair<string, int>(name, score);
        else if (buildindex == 10) _recordPF[idx] = new Rating(name, score, score2);
    }

    //get data(score type)
    public void GetData(int buildindex, int idx, out string out_name, out int out_score) {
        if (buildindex == 3) {
            out_name = _recordF[idx].Key;
            out_score = _recordF[idx].Value;
        }
        else if (buildindex == 4) {
            out_name = _recordA[idx].Key;
            out_score = _recordA[idx].Value;
        }
        else if (buildindex == 5) {
            out_name = _recordG[idx].Key;
            out_score = _recordG[idx].Value;
        }
        else {
            out_name = _recordN[idx].Key;
            out_score = _recordN[idx].Value;
        }
    }

    //get data(win/lose type)
    public void GetData(int buildindex, int idx, out string out_name, out int out_score, out int out_score2) {
        if (buildindex == 6) {
            out_name = _recordOm[idx].Name;
            out_score = _recordOm[idx].Win;
            out_score2 = _recordOm[idx].Lose;
        }
        else if (buildindex == 7) {
            out_name = _recordOd[idx].Name;
            out_score = _recordOd[idx].Win;
            out_score2 = _recordOd[idx].Lose;
        }
        else {
            out_name = _recordPF[idx].Name;
            out_score = _recordPF[idx].Win;
            out_score2 = _recordPF[idx].Lose;
        }
    }

    //user list save
    private void SaveUsers() {
        string tmp = "";
        for (int i = 0; i < users.Count; i++) tmp += users[i] + "\n";
        PlayerPrefs.SetString("UsersList", tmp);
    }

    //user data save
    public void Save() {
        string tmp = $"{_bestF}/{_bestA}/{_bestG}/{_bestOm[0]}/{_bestOm[1]}/{_bestOd[0]}/{_bestOd[1]}/{_bestN}/{_bestPF[0]}/{_bestPF[1]}";
        PlayerPrefs.SetString(_name + "B", tmp);
    }

    //record data save
    public void SaveRecord(int buildindex) {
        string tmp = "";
        for (int i = 0; i < 10; i++) {
            if (buildindex == 3) {
                tmp += string.Format("{0}/{1:D}\n", _recordF[i].Key, _recordF[i].Value);
                PlayerPrefs.SetString("RecordF", tmp);
            }
            else if (buildindex == 4) {
                tmp += string.Format("{0}/{1:D}\n", _recordA[i].Key, _recordA[i].Value);
                PlayerPrefs.SetString("RecordA", tmp);
            }
            else if (buildindex == 5) {
                tmp += string.Format("{0}/{1:D}\n", _recordG[i].Key, _recordG[i].Value);
                PlayerPrefs.SetString("RecordG", tmp);
            }
            else if (buildindex == 6) {
                tmp += string.Format("{0}/{1:D}/{2:D}\n", _recordOm[i].Name, _recordOm[i].Win, _recordOm[i].Lose);
                PlayerPrefs.SetString("RecordOm", tmp);
            }
            else if (buildindex == 7) {
                tmp += string.Format("{0}/{1:D}/{2:D}\n", _recordOd[i].Name, _recordOd[i].Win, _recordOd[i].Lose);
                PlayerPrefs.SetString("RecordOd", tmp);
            }
            else if (buildindex == 8) {
                tmp += string.Format("{0}/{1:D}\n", _recordN[i].Key, _recordN[i].Value);
                PlayerPrefs.SetString("RecordN", tmp);
            }
            else if (buildindex == 10) {
                tmp += string.Format("{0}/{1:D}/{2:D}\n", _recordPF[i].Name, _recordPF[i].Win, _recordPF[i].Lose);
                PlayerPrefs.SetString("RecordPF", tmp);
            }
        }
    }

    //best score update(score type)
    public void UpdateBest(int buildindex, int _score) {
        if (buildindex == 3) {
            if (BestF < _score) BestF = _score;
        }
        else if (buildindex == 4) {
            if (BestA < _score) BestA = _score;
        }
        else if (buildindex == 5) {
            if (BestG < _score) BestG = _score;
        }
        else if (buildindex == 8) {
            if (BestN < _score) BestN = _score;
        }
        Save();
    }
    
    //best score update(win/lose type)
    public void UpdateBest(int buildindex, int _win, int _lose) {
        if (buildindex == 6) {
            if (BestOm[0] < _win) BestOm[0] = _win;
            if (BestOm[1] < _lose) BestOm[1] = _lose;
        }
        else if (buildindex == 7) {
            if (BestOd[0] < _win) BestOd[0] = _win;
            if (BestOd[1] < _lose) BestOd[1] = _lose;
        }
        else if (buildindex == 10) {
            if (BestPF[0] < _win) BestPF[0] = _win;
            if (BestPF[1] < _lose) BestPF[1] = _lose;
        }
        Save();
    }

    //get recored data to string
    public string GetRankString(int buildindex) {
        string res = "";
        for (int i = 0; i < 10; i++) {
            if (buildindex == 3) res += string.Format("{0:D2}. {1}({2:#,0})\n", i + 1, _recordF[i].Key, _recordF[i].Value);
            else if (buildindex == 4) res += string.Format("{0:D2}. {1}({2:#,0})\n", i + 1, _recordA[i].Key, _recordA[i].Value);
            else if (buildindex == 5) res += string.Format("{0:D2}. {1}({2:#,0})\n", i + 1, _recordG[i].Key, _recordG[i].Value);
            else if (buildindex == 6) res += string.Format("{0:D2}. {1}({2:#,0}/{3:#,0})\n", i + 1, _recordOm[i].Name, _recordOm[i].Win, _recordOm[i].Lose);
            else if (buildindex == 7) res += string.Format("{0:D2}. {1}({2:#,0}/{3:#,0})\n", i + 1, _recordOd[i].Name, _recordOd[i].Win, _recordOd[i].Lose);
            else if (buildindex == 8) res += string.Format("{0:D2}. {1}({2:#,0})\n", i + 1, _recordN[i].Key, _recordN[i].Value);
            else if (buildindex == 10) res += string.Format("{0:D2}. {1}({2:#,0}/{3:#,0})\n", i + 1, _recordPF[i].Name, _recordPF[i].Win, _recordPF[i].Lose);
        }
        return res;
    }

    //Create account
    public int CreateAccount(string _id, string _pwd) {
        if (_id == "" || _pwd == "") return 1; //id 혹은 pwd 입력 검사
        for (int i = 0; i < _id.Length; i++) { //id에 숫자, 영문자 외 문자 검사
            if (!('0' <= _id[i] && _id[i] <= '9') && !('A' <= _id[i] && _id[i] <= 'Z') && !('a' <= _id[i] && _id[i] <= 'z')) return 2;
        }
        for (int i = 0; i < _pwd.Length; i++) { //pwd 공백 검사
            if (_pwd[i] == ' ' && _pwd[i] == '\n') return 3;
        }
        if (users != null) { //id 중복 검사
            for (int i = 0; i < users.Count; i++) {
                if (users[i].CompareTo(_id) == 0) return 4;
            }
        }
        users.Add(_id);
        SaveUsers();
        PlayerPrefs.SetString(_id, _pwd);
        PlayerPrefs.SetString(_id + "B", defaultScore);
        return 0;
    }
    
    //Check account
    public int CheckAccount(string _id, string _pwd) {
        if (_id == "" || _pwd == "") return 1; //id 혹은 pwd 입력 검사
        if (users != null) {
            for (int i = 0; i < users.Count; i++) {
                if (users[i].CompareTo(_id) != 0) continue;
                if (users[i].CompareTo(_id) == 0) {
                    if (_pwd.CompareTo(PlayerPrefs.GetString(_id, "")) != 0) return 3; //pwd 검사
                    else {
                        Load(_id);
                        return 0;
                    }
                }
            }
        }
        return 2; //id 검사
    }

    public override void GameStart() { }

    public override void GameOver() { }
}
