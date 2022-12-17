using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool _pauseMenu;
    public GameObject pauseGameMenu;

    public Player player;

    private bool _deathMenu;
    public GameObject deathGameMenu;

    public GameObject settingsGameMenu;
    private bool _settingsMenu;

    public AudioMixer musicMixer;
    public float volumeMusic = 0.5f;
    private bool _musicOff;
    
    public AudioMixer soundMixer;
    public float volumeSound = 0.5f;
    private bool _soundOff = true;
    private List<AudioSource> listAudioSource = new List<AudioSource>();
    private AudioSource _bossAudio;
    private AudioSource _endAudio;

    public AudioMixerSnapshot pauseSnapshot;
    public AudioMixerSnapshot unpauseSnapshot;
    public AudioSource music;

    //public bool mainMenu;
    private int _count;
    public Slider sliderVolumeMusic;
    public Slider sliderVolumeSound;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Cursor.visible = false;

        var listEnemyGameObject = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in listEnemyGameObject)
        {
            listAudioSource.Add(enemy.GetComponent<AudioSource>());
        }
        listAudioSource.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>());
        _bossAudio = GameObject.FindWithTag("Boss").GetComponent<AudioSource>();
        _endAudio = GameObject.FindWithTag("Point").GetComponent<AudioSource>();
        music.Stop();
        LoadSettings();
        AcceptSettings();
    }

    private void Update()
    {
        sliderVolumeMusic.value = volumeMusic;
        sliderVolumeSound.value = volumeSound;
        
        SaveSettings();
        //if (mainMenu) return;
        
        if (Input.GetKeyDown(KeyCode.Escape) && !_deathMenu)
        {
            if (_pauseMenu)
            {
                if (_settingsMenu) Settings();
                else Resume();
            }
            else Pause();
        }

        if (player.death)
        {
            player.SetMenu();
            deathGameMenu.SetActive(true);
            _count++;
            if (_count == 1) VolumeInMenu();
            Time.timeScale = 0;
            _deathMenu = true;
            Cursor.visible = true;
        }
        LowPass();
        
    }

    public void Resume()
    {
        player.SetMenu();
        pauseGameMenu.SetActive(false);
        //if (!mainMenu) VolumeOutMenu();
        Time.timeScale = 1f;
        _pauseMenu = false;
        Cursor.visible = false;
    }

    private void Pause()
    {
        player.SetMenu();
        pauseGameMenu.SetActive(true);
        //if (!mainMenu) VolumeInMenu();
        Time.timeScale = 0;
        _pauseMenu = true;
        Cursor.visible = true;
    }

    public void Settings()
    {
        if (!_settingsMenu)
        {
            pauseGameMenu.SetActive(false);
            settingsGameMenu.SetActive(true);
            _settingsMenu = true;
        }
        else
        {
            settingsGameMenu.SetActive(false);
            pauseGameMenu.SetActive(true);
            _settingsMenu = false;
        }
    }
    
    public void InMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartLevel()
    {
        player.SetMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
        player.SetNoDeath();
        Time.timeScale = 1f;
        _deathMenu = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void VolumeInMenu()
    {
        if (volumeMusic > 0.0001f) volumeMusic /= 1.5f;
        musicMixer.SetFloat("volume", Mathf.Log10(volumeMusic) * 20);
        foreach (var soundInList in listAudioSource)
        {
            soundInList.Stop();
        }
        _bossAudio.Stop();
        _endAudio.Stop();
    }

    private void VolumeOutMenu()
    {
        if (volumeMusic < 0.6f) volumeMusic *= 1.5f;
        musicMixer.SetFloat("volume", Mathf.Log10(volumeMusic) * 20);
        foreach (var soundInList in listAudioSource)
        {
            soundInList.Play();
        }
        _bossAudio.Play();
        _endAudio.Play();
    }

    public void SetVolume(float volume)
    {
        volumeMusic = volume;
        musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void SoundMusic()
    {
        if (_musicOff) music.Stop();
        else music.Play();
        _musicOff = !_musicOff;
    }
    
    public void SoundSound()
    {
        if (_soundOff)
        {
            foreach (var soundInList in listAudioSource)
            {
                soundInList.mute = true;
            }

            _bossAudio.mute = true;
            _endAudio.mute = true;
        }
        else
        {
            foreach (var soundInList in listAudioSource)
            {
                soundInList.mute = false;
            }

            _bossAudio.mute = false;
            _endAudio.mute = false;
        }
        _soundOff = !_soundOff;
    }
    
    public void SetVolumeSound(float volume)
    {
        volumeSound = volume;
        soundMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    private void LowPass()
    {
        if (Time.timeScale == 0) pauseSnapshot.TransitionTo(.01f);
        else unpauseSnapshot.TransitionTo(.01f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("VolumeSound", volumeSound);
        PlayerPrefs.SetFloat("VolumeMusic", volumeMusic);
        
        var offMusic = 0;
        if (_musicOff) offMusic = 1;
        PlayerPrefs.SetInt("OffMusic", offMusic);
        
        var offSound = 0;
        if (_soundOff) offSound = 1;
        PlayerPrefs.SetInt("OffSound", offSound);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("VolumeSound")) volumeSound = PlayerPrefs.GetFloat("VolumeSound");
        else volumeSound = 0.5f;
        
        if (PlayerPrefs.HasKey("VolumeMusic")) volumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        else volumeMusic = 0.5f;

        /*var offMusic = 1;
        if (PlayerPrefs.HasKey("OffMusic")) offMusic = PlayerPrefs.GetInt("OffMusic");
        if (offMusic == 1) _musicOff = true;
        else _musicOff = false;

        var offSound = 0;
        if (PlayerPrefs.HasKey("OffSound")) offSound = PlayerPrefs.GetInt("OffSound");
        if (offSound == 1) _soundOff = true;
        else _soundOff = false;*/
    }

    private void AcceptSettings()
    {
        /*if (_soundOff)
        {
            foreach (var soundInList in listAudioSource)
            {
                soundInList.mute = true;
            }
        }
        else
        {
            foreach (var soundInList in listAudioSource)
            {
                soundInList.mute = false;
            }
        }
        
        if (_musicOff) music.Stop();
        else music.Play();*/
        
        SetVolumeSound(volumeSound);
        SetVolume(volumeMusic);
    }
}
