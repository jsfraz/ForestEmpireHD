using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Fight : MonoBehaviour
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
    public TextAsset mercySuccessText;
    public TextAsset mercyFailText;
    public TextAsset mercyWinText;
    public TextAsset runText;
    public TextAsset runSuccessText;
    public TextAsset runFailText;
    public TextAsset runWinText;
    public TextAsset enemyActionText;
    public TextAsset winText;
    public TextAsset deathText;

    public TextAsset centaurNames;
    public TextAsset dragonNames;
    public TextAsset ogreNames;
    public TextAsset skeletonNames;
    public TextAsset wolfNames;

    public Enemy enemy;
    public Player player = new Player("Tomáš", 0, 0, 0, 0, 0, 0, 0, null);      //vytvoření objektu hráče

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    // Start is called before the first frame update
    void Start()
    {
        //vygenerování enemy stats
        enemy = new Enemy(GetEnemyType(), "", GetEnemyLevel(), 0, 0, 0, 0);     //vytvoření objektu enemy
        enemy.name = GetEnemyName(enemy.type);      //jméno
        enemy.maxhp = GetEnemyMaxHp(enemy.type, enemy.level);       //maxhp
        enemy.hp = enemy.maxhp;     //hp
        enemy.attack = GetEnemyAttack(enemy.type, enemy.level);     //attack
        enemy.defense = GetEnemyDefense(enemy.type, enemy.level);       //defense

        //vypsání enemy stats
        StartCoroutine(HpStart(enemy.maxhp, enemy.hp, enemyHealthBar));      //náběh enemy hp
        enemyName.text = enemy.name + " LVL " + enemy.level;

        StartCoroutine(GetPlayerStats());

        PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Hudba/fight/" + UnityEngine.Random.Range(1, 5), 0.3F, true);
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

    //UnityEngine.Random typ enemy z metody
    string GetEnemyType()
    {
        //string[] array = { "dragon", "ogre", "centaur", "skeleton", "wolf" };

        int line = UnityEngine.Random.Range(1, 100);       //UnityEngine.Random


        string type = "";
        //drak
        if (line <= 20)
            type = "dragon";
        //ogr
        if (line > 20 && line <= 40)
            type = "ogre";
        //kentaur
        if (line > 40 && line <= 60)
            type = "centaur";
        //skeleton
        if (line > 60 && line <= 80)
            type = "skeleton";
        //vlk
        if (line > 80 && line <= 100)
            type = "wolf";

        Debug.Log("Loading enemy type " + line);
        return type;        //typ enemy
    }

    //jméno enemy podle typu z assetu
    string GetEnemyName(string type)
    {
        string name = "";

        if (type == "dragon")
        {
            string[] array = dragonNames.text.Split("\n"[0]);
            int index = UnityEngine.Random.Range(0, array.Length - 1);
            name = array[index];

            if (index <= 9)
                enemy_obj.GetComponent<Animator>().Play("dragonMale");
            else
                enemy_obj.GetComponent<Animator>().Play("dragonFemale");
        }
        if (type == "ogre")
        {
            string[] array = ogreNames.text.Split("\n"[0]);
            name = array[UnityEngine.Random.Range(0, array.Length - 1)];

            enemy_obj.GetComponent<Animator>().Play("ogre");
        }
        if (type == "centaur")
        {
            string[] array = centaurNames.text.Split("\n"[0]);
            int index = UnityEngine.Random.Range(0, array.Length - 1);
            name = array[index];

            if (index <= 9)
                enemy_obj.GetComponent<Animator>().Play("centaurMale");
            else
                enemy_obj.GetComponent<Animator>().Play("centaurFemale");
        }
        if (type == "skeleton")
        {
            name = skeletonNames.text;
            enemy_obj.GetComponent<Animator>().Play("skeleton");
        }
        if (type == "wolf")
        {
            name = wolfNames.text;
            enemy_obj.GetComponent<Animator>().Play("wolf");
        }

        Debug.Log("Loading enemy name " + name);
        return name;        //typ enemy
    }

    //generování enemy lvl
    int GetEnemyLevel()
    {
        int lvl = UnityEngine.Random.Range(1, 30);     //UnityEngine.Random

        int level = 0;
        //1
        if (lvl <= 10)
            level = 1;
        if (lvl > 10 && lvl <= 20)
            level = 2;
        if (lvl > 20 && lvl <= 30)
            level = 3;

        Debug.Log("Loading enemy level " + level);

        return level;
    }

    //generuje maximální enemy hp podle typu a levelu
    int GetEnemyMaxHp(string type, int level)        //ke každému typu enemy existuje pole s mezí hodnot podle levelu
    {
        int hp = 0;

        //drak
        if (type == "dragon")
        {
            int[] hp_levels = { 25, UnityEngine.Random.Range(26, 30), UnityEngine.Random.Range(30, 35) };

            hp = hp_levels[level - 1];
        }
        //ogr
        if (type == "ogre")
        {
            int[] hp_levels = { 22, UnityEngine.Random.Range(23, 25), UnityEngine.Random.Range(26, 30) };

            hp = hp_levels[level - 1];
        }
        //kentaur
        if (type == "centaur")
        {
            int[] hp_levels = { 24, UnityEngine.Random.Range(23, 26), UnityEngine.Random.Range(26, 29) };

            hp = hp_levels[level - 1];
        }
        //kostlivec
        if (type == "skeleton")
        {
            int[] hp_levels = { 20, UnityEngine.Random.Range(21, 22), UnityEngine.Random.Range(23, 25) };

            hp = hp_levels[level - 1];
        }
        //vlk
        if (type == "wolf")
        {
            int[] hp_levels = { 15, UnityEngine.Random.Range(16, 19), UnityEngine.Random.Range(20, 22) };

            hp = hp_levels[level - 1];
        }

        Debug.Log("Setting enemy max hp " + hp);

        return hp;
    }

    //generuje attack enemy podle typu a levelu
    int GetEnemyAttack(string type, int level)      //ke každému typu enemy existuje pole s mezí hodnot podle levelu
    {
        int attack = 0;

        //drak
        if (type == "dragon")
        {
            int[] attack_levels = { 10, UnityEngine.Random.Range(11, 13), UnityEngine.Random.Range(14, 15) };

            attack = attack_levels[level - 1];
        }
        //ogr
        if (type == "ogre")
        {
            int[] attack_levels = { 10, UnityEngine.Random.Range(10, 11), UnityEngine.Random.Range(11, 12) };

            attack = attack_levels[level - 1];
        }
        //kentaur
        if (type == "centaur")
        {
            int[] attack_levels = { 10, UnityEngine.Random.Range(11, 12), UnityEngine.Random.Range(12, 13) };

            attack = attack_levels[level - 1];
        }
        //kostlivec
        if (type == "skeleton")
        {
            int[] attack_levels = { 9, UnityEngine.Random.Range(9, 11), UnityEngine.Random.Range(12, 13) };

            attack = attack_levels[level - 1];
        }
        //vlk
        if (type == "wolf")
        {
            int[] attack_levels = { 8, UnityEngine.Random.Range(9, 10), UnityEngine.Random.Range(11, 12) };

            attack = attack_levels[level - 1];
        }

        Debug.Log("Setting enemy attack " + attack);

        return attack;
    }

    //generuje defense enemy podle typu a levelu
    int GetEnemyDefense(string type, int level)     //ke každému typu enemy existuje pole s mezí hodnot podle levelu
    {
        int defense = 0;

        //drak
        if (type == "dragon")
        {
            int[] defense_levels = { 5, UnityEngine.Random.Range(5, 6), UnityEngine.Random.Range(7, 8) };

            defense = defense_levels[level - 1];
        }
        //ogr
        if (type == "ogre")
        {
            int[] defense_levels = { 4, UnityEngine.Random.Range(4, 5), UnityEngine.Random.Range(5, 6) };

            defense = defense_levels[level - 1];
        }
        //kentaur
        if (type == "centaur")
        {
            int[] defense_levels = { 4, UnityEngine.Random.Range(4, 5), UnityEngine.Random.Range(5, 6) };

            defense = defense_levels[level - 1];
        }
        //kostlivec
        if (type == "skeleton")
        {
            int[] defense_levels = { 2, UnityEngine.Random.Range(2, 3), UnityEngine.Random.Range(3, 4) };

            defense = defense_levels[level - 1];
        }
        //vlk
        if (type == "wolf")
        {
            int[] defense_levels = { 3, UnityEngine.Random.Range(4, 5), UnityEngine.Random.Range(5, 6) };

            defense = defense_levels[level - 1];
        }

        Debug.Log("Setting enemy defense " + defense);

        return defense;
    }

    void ButtonActions()
    {
        if (fightPressed)       //fight button
        {
            fightPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            string[] fight_array = fightText.text.Split("\n"[0]);        //text na řádky
            StartCoroutine(AnimateText(0.3F, message, enemy.name + fight_array[UnityEngine.Random.Range(0, 5)]));

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
        }

        if (runPressed)     //run button
        {
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
        }
    }

    void EnemyAction()      //akce enemy po tahu hráče
    {
        if (enemyAction)
        {
            enemyAction = false;

            string[] enemy_action_array = enemyActionText.text.Split("\n"[0]);        //text na řádky
            StartCoroutine(AnimateText(message, enemy.name + enemy_action_array[UnityEngine.Random.Range(0, 5)]));

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

        yield return new WaitForSeconds(2F);
        winButtonObject.SetActive(true);

        string text = "";
        if (type == "normal")
            text = winText.text;
        if (type == "mercy")
            text = mercyWinText.text;
        if (type == "run")
            text = runWinText.text;

        StartCoroutine(AnimateText(0.2F, winMessage, text));
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

        //počet budov
        string[] buildings_count = new string[4];
        buildings_count = File.ReadAllLines(saveFolder + "/buildings_count");
        int population = int.Parse(buildings_count[0]);

        /*jeden dům ubytuje čtyři lidi
         * Čtyři lidi jsou potřeba pro obsluhu jedné budovy
         * => pokud je víc budov než domů, nemá tam kdo pracovat.
         * Proto musí být stejně domů jako funkčních budov.
         * Počítá se to od nejvýdělečnější budovy
         * => lidi automaticky pracujou tam, kde z toho je víc peněz.
         * Půjdou spíše na pilu, než na pole a spíš na pec než na pilu
         * => proto je počítací pořadí níže pec, pila, pole.
        */

        //kalkulace resources
        if (int.Parse(buildings_count[1]) != 0 && population != 0)      //pece
        {
            player.money += 30 * int.Parse(buildings_count[1]);
            player.material += 25 * int.Parse(buildings_count[1]);
            population -= int.Parse(buildings_count[1]) * 4;
        }
        if (int.Parse(buildings_count[2]) != 0 && population != 0)      //pily
        {
            player.money += 25 * int.Parse(buildings_count[2]);
            player.material += 20 * int.Parse(buildings_count[2]);
            population -= int.Parse(buildings_count[2]) * 4;
        }
        if (int.Parse(buildings_count[3]) != 0 && population != 0)      //pole
        {
            player.money += 20 * int.Parse(buildings_count[3]);
            player.material += 15 * int.Parse(buildings_count[3]);
            population -= int.Parse(buildings_count[3]) * 4;
        }

        Debug.Log(population + " unemployed.");

        //uložení
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