using UnityEngine;
using UnityEngine.UI;

//player health point
public class PlayerHealth : MonoBehaviour {
    private PlayerMovement playMovement;
    private PlayerShooting playShooting;
    private Animator anim;
    private AudioSource pAudio;
    private Slider healthSlider; //ü�¹�
    private Image damageImage; //�ǰ� ȿ��
    private Color flashColor = new Color(1f, 0f, 0f, 0.1f); //�ǰ� �� ��½�̴� ȿ��
    private bool damaged = false;
    private float flashSpeed = 5f; //��½�̴� ȿ�� �ӵ�
    private int startingHealth = 100; //���� ü��
    private int currentHealth;

    public AudioClip deadClip;
    public bool isDead = false;

    private void Awake() {
        playMovement = GetComponent<PlayerMovement>();
        playShooting = GetComponentInChildren<PlayerShooting>();
        anim = GetComponent<Animator>();
        pAudio = GetComponent<AudioSource>();
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        damageImage = GameObject.Find("DamageEffect").GetComponent<Image>();
    }

    private void Start() {
        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;
        currentHealth = startingHealth;
    }

    private void Update() {
        if (damaged) damageImage.color = flashColor;
        else damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        damaged = false;
    }

    //�ǰ�
    public void TakeDamage(int amount) {
        damaged = true;
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        pAudio.Play();
        if (currentHealth <= 0 && !isDead) Death();
    }

    //die
    void Death() {
        isDead = true;
        playShooting.DisableEffects();
        anim.SetTrigger("Die");
        pAudio.clip = deadClip;
        pAudio.Play();
        playMovement.enabled = false;
        playShooting.enabled = false;
        NightmareManager.Inst.GameOver();
    }
}
