using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class FirstFight : MonoBehaviour
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

    public Canvas buttonCanvas;     //canvas s fight buttons
    public Button fightButton;      //fight button
    public Button inventoryButton;      //inventory button
    public Button mercyButton;      //mercy button
    public Button runButton;        //run button

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
    public TextAsset inventoryText;
    public TextAsset mercyText;
    public TextAsset runText;
    public TextAsset enemyActionText;
    public TextAsset deathText;

    public Enemy enemy = new Enemy("", "Čarodějnice Harilda", 0, 100, 100, 10, 3);     //vytvoření objektu enemy
    public Player player = new Player("Tomáš", 1, 20, 20, 10, 5, 0, 0, null);      //vytvoření objektu hráče

    // Start is called before the first frame update
    void Start()
    {
        //vypsání enemy stats
        StartCoroutine(HpStart(enemy.maxhp, enemyHealthBar));      //náběh enemy hp
        enemyName.text = enemy.name;

        //vypsání player stats
        StartCoroutine(HpStart(player.maxhp, playerHealthBar));      //náběh player hp
        playerName.text = player.name;

        Debug.Log(enemyName.text + " VS " + playerName.text);

        PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Hudba/firstFight", 0.3F, true);
    }

    void Update()
    {
        //aktivace messages na začátku po naběhnutí hp
        if (hpLoaded == false && enemyHealthBar.GetComponent<Slider>().value == enemy.maxhp && playerHealthBar.GetComponent<Slider>().value == player.maxhp)
        {
            hpLoaded = true;
            StartCoroutine(ButtonsFadeIn());
        }

        ButtonActions();        //akce tlačítka
        EnemyAction();        //útok enemy

        //CHEAT
        if (Input.GetKeyDown(KeyCode.F1))
        {
            int beforeHp = player.hp;
            player.hp = 0;
            StartCoroutine(HpCounter(beforeHp, player.hp, player.maxhp, playerHealthBar));
            PlaySound(player_obj.GetComponent<AudioSource>(), "Zvuky/sfx/player_damage", 0.5F, false);      //zvuk
            cmr.GetComponent<Shake>().TriggerShake(shake_magnitude);        //otřes
            player_obj.GetComponent<Flash>().TriggerFlash();
        }
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
            {
                //nenastane
            }
            else
                StartCoroutine(EnemyActionDelay(3.3F));
        }

        if (inventoryPressed)        //inventory button
        {
            inventoryPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            PlaySound(inventoryButton.GetComponent<AudioSource>(), "Zvuky/sfx/inventory", 0.5F, false);

            StartCoroutine(AnimateText(0.3F, message, inventoryText.text));

            StartCoroutine(EnemyActionDelay(3.3F));
        }

        if (mercyPressed)       //mercy button
        {
            mercyPressed = false;

            StartCoroutine(ButtonsFadeOut());
            StartCoroutine(MessageFadeIn(0.2F));

            StartCoroutine(AnimateText(0.3F, message, enemy.name + mercyText.text));

            StartCoroutine(EnemyActionDelay(3.3F));
        }

        if (runPressed)     //run button
        {
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
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3F);
        StartCoroutine(Black());
        StartCoroutine(MessageFadeOut());
        StartCoroutine(StopSound(cmr.GetComponent<AudioSource>()));

        yield return new WaitForSeconds(3F);
        resetButtonObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        GameObject.Find(resetButtonObject.name + "Text").GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        StartCoroutine(AnimateTextTorture(0.03F, gameoverMessage, deathText.text, resetButton));
        //PlaySound(cmr.GetComponent<AudioSource>(), "Zvuky/Torture/1", 0.5F, false);      //zvuk

        //vytvoření původní village
        FirstSave.CreateSave();
    }

    IEnumerator Black()
    {
        Debug.Log("Fading black");

        gameoverCanvas.enabled = true;
        //zatmavení obrazovky
        for (byte i = 0; i < 255; i += 5)
        {
            gameoverCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return new WaitForSeconds(0.04F);
        }

        gameoverCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
    }

    IEnumerator HpCounter(int beforeHp, int currentHp, int maxHP, GameObject healthbar)        //animace healthbarů
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

        //pokud jsou player hp nulové
        if (player.hp <= 0)
            StartCoroutine(GameOver());
    }

    IEnumerator HpStart(int maxHp, GameObject healthbar)        //náběh hp
    {
        Debug.Log("Healthbar animation.");

        //inicializace
        healthbar.GetComponent<Slider>().maxValue = maxHp;

        //naběhnutí
        for (int i = 0; i <= maxHp; i++)
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
            Debug.Log(text.Length);

            while (i < text.Length)
            {
                messageObject.text += text[i++];
                yield return null;
            }

            writing = false;
        }
    }


    IEnumerator AnimateText(float delay, Text messageObject, string text)        //animování textu delay
    {
        int i = 0;
        messageObject.text = "";

        yield return new WaitForSeconds(delay);
        while (i < text.Length)
        {
            messageObject.text += text[i++];
            yield return null;
        }
    }

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
                PlaySound(gameoverMessage.GetComponent<AudioSource>(), "Zvuky/sfx/letter" + Random.Range(1, 5).ToString(), 0.2F, false);
            else
                PlaySound(gameoverMessage.GetComponent<AudioSource>(), "Zvuky/sfx/space", 0.2F, false);

            yield return new WaitForSeconds(delay);
        }

        btn.enabled = true;
    }

    IEnumerator EnemyActionDelay(float delay)
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

    IEnumerator MessageFadeIn(float delay)     //fade-in message delay
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

    IEnumerator ButtonsFadeIn()     //fade-in tlačítek
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

    void PlaySound(AudioSource audio, string resource, float volume, bool loop)
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
}
