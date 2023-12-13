using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class LastFight : MonoBehaviour
{
    public class Entity     //šablona pro ostatní třídy
    {
        public string name;
        public int level;
        public int maxhp;
        public int hp;
        public int attack;
        public int defense;

        public Entity(string name, int level, int maxhp, int hp, int attack, int defense)
        {
            this.name = name;
            this.level = level;
            this.maxhp = maxhp;
            this.hp = hp;
            this.attack = attack;
            this.defense = defense;
        }
    }

    public class Enemy : Entity     //třída enemy
    {
        public string type;

        public Enemy(string type, string name, int level, int maxhp, int hp, int attack, int defense) :
            base(name, level, maxhp, hp, attack, defense)
        {
            this.type = type;       //typ enemy (drak, ogr, kentaur, skeleton)
            this.name = name;       //jméno z https://www.fantasynamegenerators.com/
            this.level = level;     //level enemy (definuje stats)
            this.maxhp = maxhp;     //maximální enemy hp
            this.hp = hp;       //enemy hp
            this.attack = attack;       //enemy attack
            this.defense = defense;     //enemy damage
        }
    }

    public class Player : Entity     //třída hráče
    {
        public int money;
        public int material;
        public string[] inventory;

        public Player(string name, int level, int maxhp, int hp, int attack, int defense, int money, int material, string[] inventory) :
            base(name, level, maxhp, hp, attack, defense)
        {
            this.name = name;       //jméno hrdiny
            this.level = level;     //level
            this.maxhp = maxhp;     //maximální hp
            this.hp = hp;       //hp
            this.attack = attack;       //attack
            this.defense = defense;     //damage
            this.money = money;
            this.material = material;
            this.inventory = inventory;
        }
    }

    public Camera cmr;      //objekt kamery
    public GameObject enemy_obj;
    public GameObject player_obj;

    public Canvas healthbarCanvas;      //canvas s healthbary
    public Text enemyName;      //jméno enemy
    public GameObject enemyHealthBar;       //healthbar enemy
    public Text enemyHealthBarHp;       //enemy hp ukazatel
    public Text playerName;     //jméno hráče
    public GameObject playerHealthBar;      //healthbar hráče
    public Text playerHealthBarHp;       //player hp ukazatel

    public Canvas messageCanvas;        //canvas s message oknem
    public Text message;        //text zprávy

    public Canvas gameoverCanvas;
    public Text gameoverMessage;
    public Button resetButton;
    public GameObject resetButtonObject;

    public Canvas winCanvas;
    public Text winMessage;
    public GameObject winButtonObject;

    public Canvas buttonCanvas;     //canvas s fight buttons
    public Button fightButton;      //fight button
    public Button inventoryButton;      //inventory button
    public Button mercyButton;      //mercy button
    public Button runButton;        //run button

    public GameObject inventoryCanvas;

    public bool writing = false;
    public bool fightPressed = false;       //bylo stlačeno fight tlačítko
    public bool inventoryPressed = false;       //bylo stlačeno inventory tlačítko
    public bool mercyPressed = false;       //bylo stlačeno mercy tlačítko
    public bool runPressed = false;     //bylo stlačeno run tlačítko
    private bool enemyAction = false;       //probíhá enemy akce
    private bool hpLoaded = false;
    public float shake_magnitude;       //síla otřesu

    //textové assety
    public TextAsset fightText;
    public TextAsset emptyInventoryText;
    public TextAsset mercyText;
    public TextAsset runText;
    public TextAsset enemyActionText;
    public TextAsset winText;
    public TextAsset deathText;

    public Enemy enemy = new Enemy("", "Čarodějnice Harilda", 0, 100, 100, 10, 3);     //vytvoření objektu enemy
    public Player player = new Player("Tomáš", 0, 0, 0, 0, 0, 0, 0, null);      //vytvoření objektu hráče

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";
    string[] lastfight;

    // Start is called before the first frame update
    void Start()
    {
        //vypsání enemy stats
        StartCoroutine(HpStart(enemy.maxhp, enemy.hp, enemyHealthBar));      //náběh enemy hp
        enemyName.text = enemy.name;

        StartCoroutine(GetPlayerStats());

        PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Hudba/lastfight", 0.3F, true);

        lastfight = File.ReadAllLines(saveFolder + "/lastfight");
    }

    void Update()
    {
        //aktivace messages na začátku po naběhnutí hp
        if (hpLoaded == false && enemyHealthBar.GetComponent<Slider>().value == enemy.hp && playerHealthBar.GetComponent<Slider>().value == player.hp)
        {
            hpLoaded = true;
            StartCoroutine(ButtonsFadeIn());
        }

        ButtonActions();        //akce tlačítka
        EnemyAction();        //útok enemy

        //CHEAT
        if (Input.GetKeyDown(KeyCode.F1))
        {
            int beforeHp = enemy.hp;
            enemy.hp = 0;
            StartCoroutine(HpCounter(beforeHp, enemy.hp, enemy.maxhp, enemyHealthBar));
            PlaySound(enemy_obj.GetComponent<AudioSource>(), "Zvuky/sfx/sword_hit", 0.5F, false);      //zvuk
            cmr.GetComponent<Shake>().TriggerShake(shake_magnitude);        //otřes
            enemy_obj.GetComponent<Flash>().TriggerFlash();
            StartCoroutine(Win("normal", 1F));
        }
    }

    IEnumerator GetPlayerStats()
    {
        yield return new WaitForEndOfFrame();

        //načte stats save
        string[] stats = File.ReadAllLines(saveFolder + "/stats");

        player.level = int.Parse(stats[0]);     //level
        player.maxhp = int.Parse(stats[1]);        //maxhp
        player.hp = int.Parse(stats[2]);        //hp
        player.attack = int.Parse(stats[3]);     //attack
        player.defense = int.Parse(stats[4]);       //defense
        player.money = int.Parse(stats[5]);     //money
        player.material = int.Parse(stats[6]);      //material
        //inventář
        string[] inventory = File.ReadAllLines(saveFolder + "/inventory");
        player.inventory = inventory;

        //vypsání player stats
        StartCoroutine(HpStart(player.maxhp, player.hp, playerHealthBar));      //náběh player hp
        playerName.text = player.name + " LVL " + player.level;
    }

    void ButtonActions()
    {
        if (fightPressed)       //fight button
        {
            fightPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            StartCoroutine(AnimateText(0.3F, message, enemy.name + fightText.text));

            int beforeHp = enemy.hp;
            enemy.hp = enemy.hp - player.attack + enemy.defense;
            if (enemy.hp < 0)
                enemy.hp = 0;
            StartCoroutine(HpCounter(beforeHp, enemy.hp, enemy.maxhp, enemyHealthBar));

            PlaySound(enemy_obj.GetComponent<AudioSource>(), "Zvuky/sfx/sword_hit", 0.5F, false);      //zvuk
            cmr.GetComponent<Shake>().TriggerShake(shake_magnitude);        //otřes
            enemy_obj.GetComponent<Flash>().TriggerFlash();

            if (enemy.hp <= 0)
                StartCoroutine(Win("normal", 1F));
            else
                StartCoroutine(EnemyActionDelay(3.3F));
        }

        if (inventoryPressed)        //inventory button
        {
            inventoryPressed = false;

            StartCoroutine(ButtonsFadeOut());
            PlaySound(inventoryButton.GetComponent<AudioSource>(), "Zvuky/sfx/inventory", 0.5F, false);

            bool empty = true;
            for (int i = 0; i < player.inventory.Length; i++)
                if (player.inventory[i] != "")
                    empty = false;

            //prázdný inventář
            if (empty)
            {
                StartCoroutine(MessageFadeIn(0.2F));
                StartCoroutine(AnimateText(0.3F, message, emptyInventoryText.text));
                StartCoroutine(EnemyActionDelay(3.3F));
            }
            else
            {
                inventoryCanvas.GetComponent<Canvas>().enabled = true;
                inventoryCanvas.GetComponent<BattleInventory>().StartInventory(player.inventory);
            }
        }

        if (mercyPressed)       //mercy button
        {
            /*
            mercyPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));
            
            //šance
            int chance = UnityEngine.Random.Range(1, 100);

            //mercy zpráva
            string[] mercy_array = mercyText.text.Split("\n"[0]);        //text na řádky
            StartCoroutine(AnimateText(0.3F, message, mercy_array[UnityEngine.Random.Range(0, 6)]));

            //success
            if (chance <= 33)
            {
                string[] mercy_success_array = mercySuccessText.text.Split("\n"[0]);        //text na řádky
                StartCoroutine(AnimateText(6F, message, enemy.name + mercy_success_array[UnityEngine.Random.Range(0, 3)]));
                StartCoroutine(Win("mercy", 10F));
            }

            //fail
            if (chance > 33)
            {
                string[] mercy_fail_array = mercyFailText.text.Split("\n"[0]);        //text na řádky
                StartCoroutine(AnimateText(6F, message, enemy.name + mercy_fail_array[UnityEngine.Random.Range(0, 3)]));

                StartCoroutine(EnemyActionDelay(11F));
            }
            */

            mercyPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            StartCoroutine(AnimateText(0.3F, message, enemy.name + mercyText.text));

            StartCoroutine(EnemyActionDelay(3.3F));
        }

        if (runPressed)     //run button
        {
            /*
            runPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));
            
            //šance
            int chance = UnityEngine.Random.Range(1, 100);

            //run zpráva
            string[] run_array = runText.text.Split("\n"[0]);        //text na řádky
            StartCoroutine(AnimateText(0.3F, message, run_array[UnityEngine.Random.Range(0, 6)]));

            //success
            if (chance <= 33)
            {
                string[] run_success_array = runSuccessText.text.Split("\n"[0]);        //text na řádky
                StartCoroutine(AnimateText(6F, message, run_success_array[UnityEngine.Random.Range(0, 3)]));
                StartCoroutine(Win("run", 10F));
            }

            //fail
            if (chance > 33)
            {
                string[] run_fail_array = runFailText.text.Split("\n"[0]);        //text na řádky
                StartCoroutine(AnimateText(6F, message, run_fail_array[UnityEngine.Random.Range(0, 4)]));

                StartCoroutine(EnemyActionDelay(11F));
            }
            */

            runPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            StartCoroutine(AnimateText(0.3F, message, runText.text));

            StartCoroutine(EnemyActionDelay(3.3F));
        }
    }

    void EnemyAction()      //akce enemy po tahu hráče
    {
        if (enemyAction)
        {
            enemyAction = false;

            StartCoroutine(AnimateText(message, enemy.name + enemyActionText.text));

            int beforeHp = player.hp;
            player.hp = player.hp - enemy.attack + player.defense;
            if (player.hp < 0)
                player.hp = 0;
            StartCoroutine(HpCounter(beforeHp, player.hp, player.maxhp, playerHealthBar));

            PlaySound(player_obj.GetComponent<AudioSource>(), "Zvuky/sfx/player_damage", 0.5F, false);      //zvuk
            cmr.GetComponent<Shake>().TriggerShake(shake_magnitude);        //otřes
            player_obj.GetComponent<Flash>().TriggerFlash();

            if (player.hp > 0)
            {
                StartCoroutine(MessageFadeOut(3F));
                StartCoroutine(ButtonsFadeIn(3.2F));
            }
            if (player.hp <= 0)
                StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1F);
        StartCoroutine(GameObjectFadeOut(player_obj));
        StartCoroutine(Black(gameoverCanvas));
        StartCoroutine(MessageFadeOut());
        PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Hudba/lose", 0.3F, false);

        yield return new WaitForSeconds(2F);
        resetButtonObject.SetActive(true);

        StartCoroutine(AnimateText(0.2F, gameoverMessage, deathText.text));
    }

    IEnumerator Win(string type, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(GameObjectFadeOut(enemy_obj));
        StartCoroutine(Black(winCanvas));
        StartCoroutine(MessageFadeOut());
        //StartCoroutine(StopSound(cmr.GetComponent<AudioSource>()));
        PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Hudba/win", 0.3F, false);
        StartCoroutine(SaveInventory(player.inventory));
        StartCoroutine(SaveStats());

        yield return new WaitForSeconds(3F);
        winButtonObject.SetActive(true);

        lastfight[0] = "true";
        File.WriteAllLines(saveFolder + "/lastfight", lastfight);

        /*
        string text = "";
        if (type == "normal")
            text = winText.text;
        if (type == "mercy")
            text = mercyWinText.text;
        if (type == "run")
            text = runWinText.text;
        */

        StartCoroutine(AnimateText(0.2F, winMessage, winText.text));
    }

    IEnumerator Black(Canvas cnvs)
    {
        Debug.Log("Fading black");

        cnvs.enabled = true;
        //zatmavení obrazovky
        for (byte i = 0; i < 255; i += 5)
        {
            cnvs.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return new WaitForSeconds(0.02F);
        }

        cnvs.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
    }

    public IEnumerator HpCounter(int beforeHp, int currentHp, int maxHP, GameObject healthbar)        //animace healthbarů
    {
        Slider healthbarSlider = healthbar.GetComponent<Slider>();
        Text healthbarText = GameObject.Find(healthbar.name + "_hp").GetComponent<Text>();

        if (currentHp < beforeHp)       //odčítání
        {
            for (int i = beforeHp; i >= currentHp; i--)
            {
                healthbarSlider.value = i;
                healthbarText.text = i + "/" + maxHP;
                PlaySound(healthbar.GetComponent<AudioSource>(), "Zvuky/sfx/health_decrease", 0.3F, false);

                yield return null;
            }
        }

        if (currentHp > beforeHp)       //přičítání
        {
            for (int i = beforeHp; i <= currentHp; i++)
            {
                healthbarSlider.value = i;
                healthbarText.text = i + "/" + maxHP;
                PlaySound(healthbar.GetComponent<AudioSource>(), "Zvuky/sfx/health_increase", 0.3F, false);

                yield return null;
            }
        }
    }

    IEnumerator HpStart(int maxHp, int Hp, GameObject healthbar)        //náběh hp
    {
        Debug.Log("Healthbar animation.");

        //inicializace
        healthbar.GetComponent<Slider>().maxValue = maxHp;

        //naběhnutí
        for (int i = 0; i <= Hp; i++)
        {
            healthbar.GetComponent<Slider>().value = i;
            //PlaySound(healthbar.GetComponent<AudioSource>(), "Zvuky/sfx/health_increase", 0.2F, false);
            GameObject.Find(healthbar.name + "_hp").GetComponent<Text>().text = i + "/" + maxHp;
            yield return null;
        }
    }

    IEnumerator AnimateText(Text messageObject, string text)        //animování textu
    {
        if (writing == false)
        {
            writing = true;

            int i = 0;
            messageObject.text = "";

            while (i < text.Length)
            {
                messageObject.text += text[i++];
                yield return null;
            }

            writing = false;
        }
    }


    public IEnumerator AnimateText(float delay, Text messageObject, string text)        //animování textu delay
    {
        yield return new WaitForSeconds(delay);

        int i = 0;
        messageObject.text = "";

        while (i < text.Length)
        {
            messageObject.text += text[i++];
            yield return null;
        }
    }

    /*
    IEnumerator AnimateTextTorture(float delay, Text messageObject, string text, Button btn)        //animování textu letter delay
    {
        int i = 0;
        messageObject.text = "";
        Debug.Log(text.Length);

        while (i < text.Length)
        {
            char letter = text[i++];
            messageObject.text += letter;

            if (letter != ' ')
                PlaySound(gameoverMessage.GetComponent<AudioSource>(), "Zvuky/sfx/letter" + UnityEngine.Random.Range(1, 5).ToString(), 0.01F, false);
            else
                PlaySound(gameoverMessage.GetComponent<AudioSource>(), "Zvuky/sfx/space", 0.01F, false);

            yield return new WaitForSeconds(delay);
        }

        btn.enabled = true;
    }
    */

    public IEnumerator EnemyActionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyAction = true;
    }

    IEnumerator MessageFadeIn()     //fade-in message
    {
        Debug.Log("Message fading in.");

        messageCanvas.GetComponent<Canvas>().enabled = true;

        for (float i = 0F; i < 1.1F; i += 0.1F)
        {
            message.GetComponent<Text>().color = new Color(0, 0, 0, i);
            GameObject.Find(message.name + "_box").GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    public IEnumerator MessageFadeIn(float delay)     //fade-in message delay
    {
        Debug.Log("Message fading in with delay " + delay);

        yield return new WaitForSeconds(delay);
        messageCanvas.GetComponent<Canvas>().enabled = true;

        for (float i = 0F; i < 1.1F; i += 0.1F)
        {
            message.GetComponent<Text>().color = new Color(0, 0, 0, i);
            GameObject.Find(message.name + "_box").GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    IEnumerator MessageFadeOut()        //fade-out message
    {
        Debug.Log("Message fading out.");

        for (float i = 1F; i > -0.1F; i -= 0.1F)
        {
            message.GetComponent<Text>().color = new Color(0, 0, 0, i);
            GameObject.Find(message.name + "_box").GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        messageCanvas.GetComponent<Canvas>().enabled = false;        //zakázání message canvasu
    }

    IEnumerator MessageFadeOut(float delay)        //fade-out message delay
    {
        Debug.Log("Message fading out with delay " + delay);

        yield return new WaitForSeconds(delay);
        for (float i = 1F; i > -0.1F; i -= 0.1F)
        {
            message.GetComponent<Text>().color = new Color(0, 0, 0, i);
            GameObject.Find(message.name + "_box").GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        messageCanvas.GetComponent<Canvas>().enabled = false;        //zakázání message canvasu
    }

    public IEnumerator ButtonsFadeIn()     //fade-in tlačítek
    {
        Debug.Log("Buttons fading in.");

        buttonCanvas.GetComponent<Canvas>().enabled = true;
        fightButton.GetComponent<Button>().enabled = true;
        inventoryButton.GetComponent<Button>().enabled = true;
        mercyButton.GetComponent<Button>().enabled = true;
        runButton.GetComponent<Button>().enabled = true;

        for (float i = 0F; i < 1.1F; i += 0.1F)
        {
            fightButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(fightButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            inventoryButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(inventoryButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            mercyButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(mercyButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            runButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(runButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    IEnumerator ButtonsFadeIn(float delay)     //fade-in tlačítek delay
    {
        Debug.Log("Buttons fading in with delay " + delay);

        buttonCanvas.GetComponent<Canvas>().enabled = true;
        fightButton.GetComponent<Button>().enabled = true;
        inventoryButton.GetComponent<Button>().enabled = true;
        mercyButton.GetComponent<Button>().enabled = true;
        runButton.GetComponent<Button>().enabled = true;

        yield return new WaitForSeconds(delay);
        for (float i = 0F; i < 1.1F; i += 0.1F)
        {
            fightButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(fightButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            inventoryButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(inventoryButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            mercyButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(mercyButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            runButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(runButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    IEnumerator ButtonsFadeOut()        //fade-out tlačítek
    {
        Debug.Log("Buttons fading out.");

        fightButton.GetComponent<Button>().enabled = false;
        inventoryButton.GetComponent<Button>().enabled = false;
        mercyButton.GetComponent<Button>().enabled = false;
        runButton.GetComponent<Button>().enabled = false;

        for (float i = 1F; i > -0.1F; i -= 0.1F)
        {
            fightButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(fightButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            inventoryButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(inventoryButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            mercyButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(mercyButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            runButton.GetComponent<Image>().color = new Color(1, 1, 1, i);
            GameObject.Find(runButton.name + "Text").GetComponent<Text>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        buttonCanvas.GetComponent<Canvas>().enabled = false;
    }

    private void PlaySound(AudioSource audio, string resource, float volume, bool loop)
    {
        audio.volume = volume;
        audio.clip = Resources.Load(resource) as AudioClip;     //inicializuje zvukový klip

        //nastavení loopování
        if (loop)
            audio.loop = true;
        else
            audio.loop = false;

        //přehrání
        audio.Play();
    }

    IEnumerator StopSound(AudioSource audio)
    {
        for (float i = audio.volume; i > 0; i -= 0.01F)
        {
            audio.volume = i;
            yield return new WaitForSeconds(0.05F);
        }

        audio.Stop();
    }

    IEnumerator SaveInventory(string[] inventory)
    {
        yield return new WaitForEndOfFrame();

        File.WriteAllLines(saveFolder + "/inventory", inventory);
    }

    IEnumerator SaveStats()
    {
        yield return new WaitForEndOfFrame();

        /*
        player.money += 50;
        player.material += 35;
        */

        string[] stats = File.ReadAllLines(saveFolder + "/stats");      //načte stats save
        stats[2] = player.hp.ToString();        //hp
        stats[5] = player.money.ToString();     //money
        stats[6] = player.material.ToString();      //material
        File.WriteAllLines(saveFolder + "/stats", stats);       //uloží stats
    }

    IEnumerator GameObjectFadeOut(GameObject obj)
    {
        for (float i = 1F; i > -0.1F; i -= 0.1F)
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.1F);
        }
    }
}