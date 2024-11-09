using UnityEngine;

//player shooting
public class PlayerShooting : MonoBehaviour {
    private LineRenderer gunLine;
    private AudioSource gunAudio;
    private ParticleSystem gunParticles;
    private Light gunLight;
    private Ray shootRay = new Ray();
    private RaycastHit shootHit;
    private float timer;
    private float effectsDisplayTime = 0.2f; //��� effect ���� �ð�
    private int shootableMask; //��� ������ object index
    private int damagePerShot = 20;

    public float timeBetweenBullets = 0.15f; //��� ���� �ð�
    public float range = 100f; //�ִ� ��Ÿ�

    private void Awake() {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }

    private void Update() {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets) Shoot();

        if (timer >= timeBetweenBullets * effectsDisplayTime) DisableEffects();
    }
    
    //shoot
    void Shoot() {
        timer = 0f;

        gunAudio.Play();
        gunLine.enabled = true;
        gunParticles.Stop();
        gunParticles.Play();
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        //��� ���� object�� �¾��� ��
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            gunLine.SetPosition(1, shootHit.point);
        }
        else gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
    }

    //�Ѿ� ȿ�� ����
    public void DisableEffects() {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }
}
