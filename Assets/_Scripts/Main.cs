using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static private Main S; // private singleton for Main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies; // array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[] {
                                            eWeaponType.blaster, eWeaponType.blaster,
                                            eWeaponType.spread, eWeaponType.shield };
    private BoundsCheck bndCheck;

    [Header("Scoring")]
    static public int currentScore = 0;
    static public int highScore = 0;

    const string HIGH_SCORE_KEY = "HighScore";
    
    void Awake() {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        currentScore = 0;
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() {
        if(!spawnEnemies) {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Set initial position for spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }

    void DelayedRestart() {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart() {
        SceneManager.LoadScene("SampleScene");
    }

    static public void HERO_DIED() {
        if(currentScore > highScore) {
            highScore = currentScore;

            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();

            Debug.Log("New High Score: " + highScore);
        }

        S.DelayedRestart();
    }

    static public void addScore(int points) {
        currentScore += points;

        Debug.Log("Current Score: " + currentScore);
    }

    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt) {
        if(WEAP_DICT.ContainsKey(wt)) {
            return(WEAP_DICT[wt]);
        }

        return(new WeaponDefinition());
    }

    static public void SHIP_DESTROYED(Enemy e) {
        if(Random.value <= e.powerUpDropChance) {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S. powerUpFrequency[ndx];

            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);

            pUp.transform.position = e.transform.position;
        }
    }
}
