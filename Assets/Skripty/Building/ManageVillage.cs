using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ManageVillage : MonoBehaviour
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

    public Camera cmr;

    public GameObject villageChunk;
    public GameObject villageHouse;
    public GameObject villageForge;
    public GameObject villageSawmill;
    public GameObject villageField;
    public GameObject villageWorkshop;
    public GameObject villageShop;

    public Sprite spriteNormal;

    public Text textPopulation;
    public Text textMaterial;
    public Text textHouses;
    public Text textForges;
    public Text textSawmills;
    public Text textFields;
    public Text textMoney;
    public Text textHP;
    public Text textAttack;
    public Text textDefense;
    public Text textLevel;
    public Text annouceText;

    public TextAsset notEnoughMoneyVillage;

    public List<GameObject> buildingList = new List<GameObject>();

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    public Player player = new Player("Tomáš", 0, 0, 0, 0, 0, 0, 0, null);      //hráč

    // Start is called before the first frame update
    void Start()
    {
        LoadVillage();      //nahraje vesnici
        StartCoroutine(CreateListAndCount());
        StartCoroutine(LoadGameInfo());     //zobrazí info ze save
    }

    private void Update()
    {
        //CHEAT
        if (Input.GetKeyDown(KeyCode.F2))
        {
            player.money += 1000;
            player.material += 1000;
            StartCoroutine(Save());
            StartCoroutine(LoadGameInfo());
        }
    }

    //nahraje vesnici ze souboru a postaví ji
    void LoadVillage()
    {
        RemoveBuildings();

        string[,] village = new string[10, 10];
        string[] save = File.ReadAllLines(saveFolder + "/village_buildings");

        int position = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                village[i, j] = save[position];
                position++;
            }
        }

        float x = transform.position.x;
        float y = transform.position.y;
        int count = 0;

        string chunkName = "villageChunk";

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string current_chunk = village[i, j];

                if (current_chunk == "")
                {
                    Instantiate(villageChunk, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageChunk.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "house")
                {
                    Instantiate(villageHouse, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageHouse.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "forge")
                {
                    Instantiate(villageForge, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageForge.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "sawmill")
                {
                    Instantiate(villageSawmill, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageSawmill.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "field")
                {
                    Instantiate(villageField, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageField.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "workshop")
                {
                    Instantiate(villageWorkshop, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageWorkshop.name + "(Clone)").name = chunkName + count;
                }
                if (current_chunk == "shop")
                {
                    Instantiate(villageShop, new Vector3(x, y, 0), Quaternion.identity, transform);
                    GameObject.Find(villageShop.name + "(Clone)").name = chunkName + count;
                }

                //buildingList.Add(GameObject.Find(chunkName + count));

                x += 11F;
                count++;
                if (j == 9)
                    x = transform.position.x;
            }
            y -= 11F;
        }
    }

    public IEnumerator Save()
    {
        yield return new WaitForEndOfFrame();

        string[] stats = File.ReadAllLines(saveFolder + "/stats");      //načte stats save
        stats[0] = player.level.ToString();     //level
        stats[1] = player.maxhp.ToString();     //maxhp
        stats[2] = player.hp.ToString();        //hp
        stats[3] = player.attack.ToString();        //attack
        stats[4] = player.defense.ToString();       //defense
        stats[5] = player.money.ToString();     //money
        stats[6] = player.material.ToString();      //material
        File.WriteAllLines(saveFolder + "/stats", stats);       //uloží stats

        string chunkName = "villageChunk";
        string[] save = File.ReadAllLines(saveFolder + "/village_buildings");       //načte buildings save
        for (int i = 0; i < 100; i++)
        {
            string currentSprite = GameObject.Find(chunkName + i).GetComponent<SpriteRenderer>().sprite.name;

            if (currentSprite == "village_empty" || currentSprite == "village_green" || currentSprite == "village_red")
                save[i] = "";

            if (currentSprite == "house")
                save[i] = "house";

            if (currentSprite == "tile003" || currentSprite == "tile004" || currentSprite == "tile005")
                save[i] = "forge";

            if (currentSprite == "sawmill")
                save[i] = "sawmill";

            if (currentSprite == "field")
                save[i] = "field";
        }
        File.WriteAllLines(saveFolder + "/village_buildings", save);       //uloží budovy

        string[] inventory = File.ReadAllLines(saveFolder + "/inventory");
        inventory[0] = player.inventory[0];
        inventory[1] = player.inventory[1];
        inventory[2] = player.inventory[2];
        inventory[3] = player.inventory[3];
        File.WriteAllLines(saveFolder + "/inventory", inventory);       //uloží inventář

        //pozice hráče
        string[] playerPosition = File.ReadAllLines(saveFolder + "/position");
        Vector3 positionInMap = GameObject.Find("Hero").GetComponent<Transform>().position;
        playerPosition[0] = positionInMap.x.ToString();     //X
        playerPosition[1] = positionInMap.y.ToString();        //Y
        playerPosition[2] = positionInMap.z.ToString();       //Z
        File.WriteAllLines(saveFolder + "/position", playerPosition);

        //čas
        SaveTime();
    }

    public void SaveTime()
    {
        DayNight light = GameObject.Find("GlobalLight2D").GetComponent<DayNight>();
        string[] timesave = { light.time.ToString(), light.intesity.ToString() };
        File.WriteAllLines(saveFolder + "/time", timesave);
    }

    //vypíše info
    public IEnumerator LoadGameInfo()
    {
        yield return new WaitForEndOfFrame();

        //načte stats save
        string[] stats = File.ReadAllLines(saveFolder + "/stats");

        //povolí první cutscénu
        if (stats[7] == "true")
        {
            GameObject.Find("Mayor").GetComponent<MayorCutscene>().enabled = true;
            stats[7] = "false";
            File.WriteAllLines(saveFolder + "/stats", stats);
        }


        //vypíše stats
        player.level = int.Parse(stats[0]);     //level
        textLevel.text = "Level: " + player.level;

        player.maxhp = int.Parse(stats[1]);        //maxhp

        player.hp = int.Parse(stats[2]);        //hp
        textHP.text = "HP: " + player.hp;
        if (int.Parse(stats[2]) == int.Parse(stats[1]))
            textHP.color = new Color32(0, 0, 0, 255);
        if (int.Parse(stats[2]) <= int.Parse(stats[1]) * 0.75)
            textHP.color = new Color32(255, 242, 0, 255);
        if (int.Parse(stats[2]) <= int.Parse(stats[1]) * 0.5)
            textHP.color = new Color32(255, 128, 64, 255);
        if (int.Parse(stats[2]) <= int.Parse(stats[1]) * 0.25)
            textHP.color = new Color32(255, 0, 0, 255);

        player.attack = int.Parse(stats[3]);     //attack
        textAttack.text = "Útok: " + player.attack;

        player.defense = int.Parse(stats[4]);       //defense
        textDefense.text = "Obrana: " + player.defense;

        player.money = int.Parse(stats[5]);     //money
        textMoney.text = "Peníze: " + player.money;

        player.material = int.Parse(stats[6]);      //material
        textMaterial.text = "Materiál: " + player.material;

        //inventář
        string[] inventory = File.ReadAllLines(saveFolder + "/inventory");
        player.inventory = inventory;

        //pozice hráče
        string[] playerPosition = File.ReadAllLines(saveFolder + "/position");
        GameObject.Find("Hero").GetComponent<Transform>().position = new Vector3(float.Parse(playerPosition[0]), float.Parse(playerPosition[1]), float.Parse(playerPosition[2]));
    }

    IEnumerator CreateListAndCount()
    {
        yield return new WaitForEndOfFrame();
        CreateList();
        CountBuildings();
    }

    //spočítá a vypíše budovy
    void CountBuildings()
    {
        int houses = 0;
        int forges = 0;
        int sawmills = 0;
        int fields = 0;
        for (int i = 0; i < 100; i++)
        {
            //domy
            if (buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "house")
                houses++;

            //kovárny
            if (buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "tile003" || buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "tile004" || buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "tile005")
                forges++;

            //pily
            if (buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "sawmill")
                sawmills++;

            //pole
            if (buildingList[i].GetComponent<SpriteRenderer>().sprite.name == "field")
                fields++;
        }

        textPopulation.text = "Obyvatelé: " + houses * 4;
        textHouses.text = "Domy: " + houses;
        textForges.text = "Kovárny: " + forges;
        textSawmills.text = "Pily: " + sawmills;
        textFields.text = "Pole: " + fields;

        //počet budov
        string[] buildings_count = new string[4];
        buildings_count[0] = (houses * 4).ToString();
        buildings_count[1] = forges.ToString();
        buildings_count[2] = sawmills.ToString();
        buildings_count[3] = fields.ToString();
        File.WriteAllLines(saveFolder + "/buildings_count", buildings_count);
    }

    void CreateList()
    {
        string chunkName = "villageChunk";

        buildingList.Clear();

        for (int i = 0; i < 100; i++)
            buildingList.Add(GameObject.Find(chunkName + i));

        Debug.Log(buildingList[60].GetComponent<SpriteRenderer>().sprite.name);

        CountBuildings();
    }

    public void BuildOnEmpty(string name, GameObject building, Vector3 position)
    {
        //cena
        int money = 0;
        int material = 0;
        string buildingType = building.GetComponent<SpriteRenderer>().sprite.name;
        if (buildingType == "house")
        {
            money = 10;
            material = 30;
        }
        if (buildingType == "tile003" || buildingType == "tile004" || buildingType == "tile005")
        {
            money = 30;
            material = 40;
        }
        if (buildingType == "sawmill")
        {
            money = 25;
            material = 35;
        }
        if (buildingType == "field")
        {
            money = 10;
            material = 10;
        }

        //mám peníze a materiál
        if (player.money >= money && player.material >= material)
        {
            int index = int.Parse(name.Replace("villageChunk", ""));

            Debug.Log("Deleting " + name + " from scene");
            Destroy(GameObject.Find(name));

            Debug.Log("Adding new " + name + " to scene");
            Instantiate(building, position, Quaternion.identity, transform);
            GameObject.Find(building.name + "(Clone)").name = name;

            StartCoroutine(CreateListAndCount());

            player.money -= money;
            player.material -= material;
            PlaySound(GameObject.Find("Village").GetComponent<AudioSource>(), "Zvuky/sfx/buy_build", 0.2F, false);

            /*
            textMoney.text = "Peníze: " + player.money;
            textMaterial.text = "Materiál: " + player.material;
            */

            StartCoroutine(Save());
            StartCoroutine(LoadGameInfo());
        }
        else
            StartCoroutine(AnnouceText(annouceText, notEnoughMoneyVillage.text, 1.5F));
    }

    //boření
    public void RemoveBuilding(string name, GameObject building, GameObject new_building, Vector3 position)
    {
        Destroy(GameObject.Find(name));
        PlaySound(GameObject.Find("Village").GetComponent<AudioSource>(), "Zvuky/sfx/destroy", 0.2F, false);

        int material = 0;
        if (building.GetComponent<SpriteRenderer>().sprite.name == "house")
            material = 6;
        if (building.GetComponent<SpriteRenderer>().sprite.name == "tile003" || building.GetComponent<SpriteRenderer>().sprite.name == "tile004" || building.GetComponent<SpriteRenderer>().sprite.name == "tile005")
            material = 8;
        if (building.GetComponent<SpriteRenderer>().sprite.name == "sawmill")
            material = 7;
        if (building.GetComponent<SpriteRenderer>().sprite.name == "field")
            material = 2;

        Instantiate(new_building, position, Quaternion.identity, transform);
        GameObject.Find(new_building.name + "(Clone)").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find(new_building.name + "(Clone)").GetComponent<BoxCollider2D>().enabled = true;
        GameObject.Find(new_building.name + "(Clone)").name = name;

        Debug.Log("Materiál: " + player.material);
        player.material = player.material + material;
        Debug.Log("Materiál: " + player.material);

        StartCoroutine(Save());
        StartCoroutine(CreateListAndCount());
        StartCoroutine(LoadGameInfo());
    }

    void RemoveBuildings()
    {
        //nesmysl, stačil by jeden cyklus, protože chunky mají číslo od 0 do 99
        buildingList.Clear();

        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Destroy(GameObject.Find(villageChunk.name + count));
                count++;
            }
        }
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

    private IEnumerator AnnouceText(Text textComponent, string text, float time)
    {
        textComponent.text = text;
        for (byte i = 0; i < 250; i += 25)
        {
            yield return new WaitForSeconds(0.03F);
            textComponent.color = new Color32(255, 0, 0, i);
        }
        textComponent.color = new Color32(255, 0, 0, 255);
        yield return new WaitForSeconds(time);
        for (byte i = 250; i > 0; i -= 25)
        {
            yield return new WaitForSeconds(0.03F);
            textComponent.color = new Color32(255, 0, 0, i);
        }
        textComponent.color = new Color32(255, 0, 0, 0);
    }
}