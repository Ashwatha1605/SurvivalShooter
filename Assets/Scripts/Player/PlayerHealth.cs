using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    [Range(0f, 1f)]
    public float armorReduction = 0f;


    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake()
    {
        //Mendapatkan refernce komponen
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();

        playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }


    void Update()
    {
        //Jika terkena damaage
        if (damaged)
        {
            //Merubah warna gambar menjadi value dari flashColour
            if (damageImage != null)
            {
                damageImage.color = flashColour;
            }
        }
        else
        {
            //Fade out damage image
            if (damageImage != null)
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
        }

        //Set damage to false
        damaged = false;
    }


    // Menghitung damage efektif setelah mitigasi armor
    public int CalculateEffectiveDamage(int rawDamage, float armor)
    {
        if (rawDamage <= 0)
            return 0;

        float clampedArmor = Mathf.Clamp(armor, 0f, 0.8f);
        return Mathf.RoundToInt(rawDamage * (1f - clampedArmor));
    }


    //fungsi untuk mendapatkan damage
    public void TakeDamage(int amount)
    {
        damaged = true;

        int effectiveDamage = CalculateEffectiveDamage(amount, armorReduction);

        //mengurangi health
        currentHealth -= effectiveDamage;

        //Merubah tampilan dari health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        //Memainkan suara ketika terkena damage
        if (playerAudio != null)
        {
            playerAudio.Play();
        }

        //Memanggil method Death() jika darahnya kurang dari sama dengan 0 dan belum mati
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    // fungsi untuk menambah nyawa
    public void Healing(int amount = 40)
    {
        currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

        //Merubah tampilan dari health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }


    void Death()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        //mentrigger animasi Die
        anim.SetTrigger("Die");

        //Memainkan suara ketika mati
        playerAudio.clip = deathClip;
        playerAudio.Play();

        //mematikan script player movement
        playerMovement.enabled = false;

        playerShooting.enabled = false;
    }

    public void RestartLevel()
    {
        //meload ulang scene dengan index 0 pada build setting
        SceneManager.LoadScene(0);
    }
}
