using Management;
using UnityEngine;
using UnityEngine.UI;

//Title scene manager
public class TitleManager : SceneManager<TitleManager> {
    //UI
    private GameObject btnSignIn = null, btnSignUp = null; //sign in, sign up button
    private GameObject txtID = null, txtPWD = null, ifID = null, ifPWD = null; //id, pwd input filed
    private InputField txtIfID = null, txtIfPWD = null;
    private GameObject btnCreate = null, btnCancel = null; //create, cancel button
    private Text txtBtnCreate = null, txtBtnCancel = null;
    private GameObject cvConfirmationWindow = null; //confirmation window
    private Text txtError = null;

    private AudioSource _as = null;
    private int titleState = 0; //0: Main, 1: Sign up, 2: Sign in

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        btnSignIn = GameObject.Find("btnSignIn");
        btnSignUp = GameObject.Find("btnSignUp");
        txtID = GameObject.Find("txtID");
        txtPWD = GameObject.Find("txtPWD");
        ifID = GameObject.Find("ifID");
        ifPWD = GameObject.Find("ifPWD");
        txtIfID = ifID.GetComponent<InputField>();
        txtIfPWD = ifPWD.GetComponent<InputField>();
        btnCreate = GameObject.Find("btnCreate");
        btnCancel = GameObject.Find("btnCancel");
        txtBtnCreate = GameObject.Find("txtBtnCreate").GetComponent<Text>();
        txtBtnCancel = GameObject.Find("txtBtnCancel").GetComponent<Text>();
        cvConfirmationWindow = GameObject.Find("cvConfirmationWindow");
        txtError = GameObject.Find("txtError").GetComponent<Text>();
        _as = GetComponent<AudioSource>();

        InitUI(true);
        cvConfirmationWindow.SetActive(false);
    }

    //UI initialization
    private void InitUI(bool b) {
        btnSignIn.SetActive(b);
        btnSignUp.SetActive(b);
        txtID.SetActive(!b);
        txtPWD.SetActive(!b);
        ifID.SetActive(!b);
        ifPWD.SetActive(!b);
        btnCreate.SetActive(!b);
        btnCancel.SetActive(!b);
    }

    //Sign up
    public void ClickSignUp() {
        titleState = 1;
        InitUI(false);
        txtBtnCreate.text = "Create";
        txtBtnCancel.text = "Cancel";
        _as.Play();
    }

    //Sign in
    public void ClickSignIn() {
        titleState = 2;
        InitUI(false);
        txtBtnCreate.text = "SignIn";
        txtBtnCancel.text = "Back";
        _as.Play();
    }

    //Confirm
    public void ClickCreate() {
        int error = 0;
        _as.Play();
        if (titleState == 1) {
            error = PIAManager.Inst.CreateAccount(txtIfID.text, txtIfPWD.text);
            if (error == 1) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "ID �Ǵ� Password�� �Էµ��� �ʾҽ��ϴ�.";
            }
            else if (error == 2) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "ID�� ����, ������� ���ڰ� �ֽ��ϴ�.";
            }
            else if (error == 3) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "Password�� ������ �ֽ��ϴ�.";
            }
            else if (error == 4) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "�ߺ��Ǵ� ID�� �ֽ��ϴ�.";
            }
            else {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "������ ���������� �����Ǿ����ϴ�.";
                txtIfID.text = "";
                txtIfPWD.text = "";
                ClickSignIn();
            }
        }
        else {
            error = PIAManager.Inst.CheckAccount(txtIfID.text, txtIfPWD.text);
            if (error == 1) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "ID �Ǵ� Password�� �Էµ��� �ʾҽ��ϴ�.";
            }
            else if (error == 2) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "ID�� �����ϴ�.";
            }
            else if (error == 3) {
                cvConfirmationWindow.SetActive(true);
                txtError.text = "Password�� �߸��Ǿ����ϴ�.";
            }
            else LoadScene(2);
        }
    }

    //Cancel
    public void ClickCancel() {
        titleState = 0;
        txtIfID.text = "";
        txtIfPWD.text = "";
        InitUI(true);
        _as.Play();
    }

    //Close confirmation window
    public void ClickBack() {
        cvConfirmationWindow.SetActive(false);
        _as.Play();
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }

    public override void GameStart() { }

    public override void GameOver() { }
}
