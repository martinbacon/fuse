/*
Fuse - Fuse is an open source action game created in Unity3D

Copyright (C) 2016  Martin Slanina

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Security.Cryptography;

public class Menu : MonoBehaviour {

    AudioSource soundPlayer;
    public InputField nickName;
    public InputField IpAddress;
    //public InputField motdTitle;
    //public InputField motdText;
    //public Toggle motdToggle;
    public GameObject player;
    public Animator anim;
    private bool left;
    private bool right;
    private bool loading;
    private float nextTime;
    private float timeout;
    GameObject last;
    MeshRenderer gmMesh;
    bool first = true;
    RaycastHit hit;
    public GameObject colliders;
    public GameObject backCustomize;
    public GameObject rightPanel;
    public Image uiSprite;
    GameObject[] heads;
    GameObject[] bodies;
    GameObject[] pants;
    GameObject[] boots;
    bool[] isEquiped = new bool[100];
    bool[] isEquipedBody = new bool[200];
    bool[] isEquipedPants = new bool[200];
    bool[] isEquipedBoots = new bool[200];
    public InputField timelimitInput;

    void Start ()
    {
        Button host = GameObject.Find("Host Button").GetComponent<Button>();
        host.onClick.AddListener(GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().StartupHost);
        Button join = GameObject.Find("Connect Button").GetComponent<Button>();
        join.onClick.AddListener(GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().JoinGame);

        soundPlayer = GetComponent<AudioSource>();
        soundPlayer.PlayOneShot(Resources.Load("Sounds/transition") as AudioClip);
#if UNITY_STANDALONE
        try
        {
            if (File.Exists("playerInformation.txt") && File.Exists("MD5"))
            {
                byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes("playerInformation.txt"));
                string playerInformationMD5 = System.BitConverter.ToString(hash).Replace("-", "");
                string MD5inFile = File.ReadAllText("MD5");
                Debug.Log(playerInformationMD5 + " " + MD5inFile);
                if (playerInformationMD5 != MD5inFile)
                {
                    Debug.Log("MD5 nesouhlasí!");
                    return;
                }
                Debug.Log("Načítám playerInformation.txt");
                string isEquipedString;
                string isEquipedBodyString;
                string isEquipedPantsString;
                string isEquipedBootsString;
                PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
                FileStream fs = new FileStream("playerInformation.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                nickName.text = sr.ReadLine();
                IpAddress.text = sr.ReadLine();
                for (int i = 0; i < 15; i++)
                {
                    plyInfo.equipment[i] = sr.ReadLine();
                }
                isEquipedString = sr.ReadLine();
                isEquipedBodyString = sr.ReadLine();
                isEquipedPantsString = sr.ReadLine();
                isEquipedBootsString = sr.ReadLine();
                timelimitInput.text = sr.ReadLine();
                sr.Close();
                fs.Close();
                plyInfo.nickName = nickName.text;
                plyInfo.time = float.Parse(timelimitInput.text);
                if (plyInfo.equipment[0] != "empty")
                {
                    GameObject head = Instantiate(Resources.Load(plyInfo.equipment[0])) as GameObject;
                    head.transform.parent = GameObject.Find("Neck").transform;
                    head.transform.localPosition = Vector3.zero;
                    head.transform.localEulerAngles = Vector3.zero;
                    head.transform.localScale = Vector3.one;
                }
                if (plyInfo.equipment[1] != "empty")
                {
                    GameObject body = Instantiate(Resources.Load(plyInfo.equipment[1])) as GameObject;
                    GameObject bodyLelbow = Instantiate(Resources.Load(plyInfo.equipment[2])) as GameObject;
                    GameObject bodyPelbow = Instantiate(Resources.Load(plyInfo.equipment[3])) as GameObject;
                    GameObject bodyLshoulder = Instantiate(Resources.Load(plyInfo.equipment[4])) as GameObject;
                    GameObject bodyPshoulder = Instantiate(Resources.Load(plyInfo.equipment[5])) as GameObject;

                    body.transform.parent = GameObject.Find("Lower_Spine").transform;
                    body.transform.localPosition = Vector3.zero;
                    body.transform.localEulerAngles = Vector3.zero;

                    bodyLelbow.transform.parent = GameObject.Find("L_Elbow").transform;
                    bodyLelbow.transform.localPosition = Vector3.zero;
                    bodyLelbow.transform.localEulerAngles = Vector3.zero;

                    bodyPelbow.transform.parent = GameObject.Find("R_Elbow").transform;
                    bodyPelbow.transform.localPosition = Vector3.zero;
                    bodyPelbow.transform.localEulerAngles = Vector3.zero;

                    bodyLshoulder.transform.parent = GameObject.Find("L_Shoulder").transform;
                    bodyLshoulder.transform.localPosition = Vector3.zero;
                    bodyLshoulder.transform.localEulerAngles = Vector3.zero;

                    bodyPshoulder.transform.parent = GameObject.Find("R_Shoulder").transform;
                    bodyPshoulder.transform.localPosition = Vector3.zero;
                    bodyPshoulder.transform.localEulerAngles = Vector3.zero;
                }
                if (plyInfo.equipment[6] != "empty")
                {
                    GameObject pantsBody = Instantiate(Resources.Load(plyInfo.equipment[6])) as GameObject;
                    GameObject pantsLknee = Instantiate(Resources.Load(plyInfo.equipment[7])) as GameObject;
                    GameObject pantsPknee = Instantiate(Resources.Load(plyInfo.equipment[8])) as GameObject;
                    GameObject pantsLthigh = Instantiate(Resources.Load(plyInfo.equipment[9])) as GameObject;
                    GameObject pantsPthigh = Instantiate(Resources.Load(plyInfo.equipment[10])) as GameObject;

                    pantsBody.transform.parent = GameObject.Find("Lower_Spine").transform;
                    pantsBody.transform.localPosition = Vector3.zero;
                    pantsBody.transform.localEulerAngles = Vector3.zero;
                    pantsBody.transform.localScale = Vector3.one;

                    pantsLknee.transform.parent = GameObject.Find("L_Knee").transform;
                    pantsLknee.transform.localPosition = Vector3.zero;
                    pantsLknee.transform.localEulerAngles = Vector3.zero;

                    pantsPknee.transform.parent = GameObject.Find("R_Knee").transform;
                    pantsPknee.transform.localPosition = Vector3.zero;
                    pantsPknee.transform.localEulerAngles = Vector3.zero;

                    pantsLthigh.transform.parent = GameObject.Find("L_Thigh").transform;
                    pantsLthigh.transform.localPosition = Vector3.zero;
                    pantsLthigh.transform.localEulerAngles = Vector3.zero;

                    pantsPthigh.transform.parent = GameObject.Find("R_Thigh").transform;
                    pantsPthigh.transform.localPosition = Vector3.zero;
                    pantsPthigh.transform.localEulerAngles = Vector3.zero;
                }
                if (plyInfo.equipment[11] != "empty")
                {
                    GameObject bootsLankle = Instantiate(Resources.Load(plyInfo.equipment[11])) as GameObject;
                    GameObject bootsRankle = Instantiate(Resources.Load(plyInfo.equipment[12])) as GameObject;
                    GameObject bootsLknee = Instantiate(Resources.Load(plyInfo.equipment[13])) as GameObject;
                    GameObject bootsRknee = Instantiate(Resources.Load(plyInfo.equipment[14])) as GameObject;

                    bootsLankle.transform.parent = GameObject.Find("L_Ankle").transform;
                    bootsLankle.transform.localPosition = Vector3.zero;
                    bootsLankle.transform.localEulerAngles = Vector3.zero;
                    bootsLankle.transform.localScale = Vector3.one;

                    bootsRankle.transform.parent = GameObject.Find("R_Ankle").transform;
                    bootsRankle.transform.localPosition = Vector3.zero;
                    bootsRankle.transform.localEulerAngles = Vector3.zero;

                    bootsLknee.transform.parent = GameObject.Find("L_Knee").transform;
                    bootsLknee.transform.localPosition = Vector3.zero;
                    bootsLknee.transform.localEulerAngles = Vector3.zero;

                    bootsRknee.transform.parent = GameObject.Find("R_Knee").transform;
                    bootsRknee.transform.localPosition = Vector3.zero;
                    bootsRknee.transform.localEulerAngles = Vector3.zero;


                }
                int j = 0;
                foreach (char c in isEquipedString)
                {
                    if (c == '1')
                    {
                        isEquiped[j] = true;
                    }
                    else
                    {
                        isEquiped[j] = false;
                    }
                    j++;
                }
                j = 0;
                foreach (char c in isEquipedBodyString)
                {
                    if (c == '1')
                    {
                        isEquipedBody[j] = true;
                    }
                    else
                    {
                        isEquipedBody[j] = false;
                    }
                    j++;
                }
                j = 0;
                foreach (char c in isEquipedPantsString)
                {
                    if (c == '1')
                    {
                        isEquipedPants[j] = true;
                    }
                    else
                    {
                        isEquipedPants[j] = false;
                    }
                    j++;
                }
                j = 0;
                foreach (char c in isEquipedBootsString)
                {
                    if (c == '1')
                    {
                        isEquipedBoots[j] = true;
                    }
                    else
                    {
                        isEquipedBoots[j] = false;
                    }
                    j++;
                }
            }
            //if (File.Exists("motd.txt"))
            //{
            //    FileStream fs2 = new FileStream("motd.txt", FileMode.Open);
            //    StreamReader sr2 = new StreamReader(fs2);
            //    motdToggle.isOn = bool.Parse(sr2.ReadLine());
            //    motdTitle.text = sr2.ReadLine();
            //    motdText.text = sr2.ReadToEnd();
            //    sr2.Close();
            //    fs2.Close();
            //}
        }
        catch
        {

        } 
#endif

    }

    // Update is called once per frame
    void Update ()
    {
        if (left)
        {
            player.transform.Rotate(0, -100 * Time.deltaTime, 0);
        }
        if (right)
        {
            player.transform.Rotate(0, 100 * Time.deltaTime, 0);
        }
        if (loading)
        {
            if (Time.time>nextTime)
            {
                nextTime = Time.time + 1;
                timeout++;
            }
            if (timeout>10)
            {
                anim.ResetTrigger("Loading");
                timeout = 0;
                nextTime = 0;
                loading = false;
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            anim.ResetTrigger("Loading");
            timeout = 0;
            nextTime = 0;
            loading = false;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag != "Background")
            {
                if (!first)
                {
                    gmMesh.material = Resources.Load("Materials/Transparent/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
                    last.GetComponent<MeshRenderer>().material.color = new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 0.1F);
                }
                gmMesh = hit.transform.gameObject.GetComponent<MeshRenderer>();
                last = hit.transform.gameObject;
                gmMesh.material = Resources.Load("Materials/Transparent/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
                gmMesh.material.color = new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 0.4F);
                first = false;
            }
            else
            {
                if (!first)
                {
                    gmMesh.material = Resources.Load("Materials/Transparent/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
                    last.GetComponent<MeshRenderer>().material.color = new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 0.1F);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.name == "Head")
            {
                Camera.main.GetComponent<Animator>().SetBool("Head", true);
                colliders.SetActive(false);
                backCustomize.SetActive(true);

                heads = Resources.LoadAll<GameObject>("Prefabs/Equipment/Head/");
                for (int i = 0; i < heads.Length; i++)
                {
                    string setName = heads[i].transform.name.Substring(0, heads[i].transform.name.IndexOf('_'));
                    GameObject buttonGO = new GameObject();
                    RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
                    buttonRT.SetParent(rightPanel.GetComponent<RectTransform>());
                    buttonRT.sizeDelta = new Vector2(50F, 50F);
                    buttonRT.anchoredPosition = new Vector2((52 * (i + 1) - 48), -4);
                    buttonRT.anchorMin = new Vector2(0, 1);
                    buttonRT.anchorMax = new Vector2(0, 1);
                    buttonRT.pivot = new Vector2(0, 1);
                    Button buttonBU = buttonGO.AddComponent<Button>();
                    Image buttonIM = buttonGO.AddComponent<Image>();
                    buttonIM.sprite = uiSprite.sprite;
                    buttonIM.type = Image.Type.Sliced;
                    buttonBU.targetGraphic = buttonIM;

                    GameObject textGO = new GameObject();
                    textGO.transform.parent = buttonGO.transform;
                    RectTransform textRT = textGO.AddComponent<RectTransform>();
                    textRT.sizeDelta = new Vector2(0F, 0F);
                    textRT.anchorMin = new Vector2(0, 0);
                    textRT.anchorMax = new Vector2(1, 1);
                    textRT.pivot = new Vector2(0.5F, 0.5F);
                    textRT.anchoredPosition = new Vector2(0, 0);
                    Text textTX = textGO.AddComponent<Text>();
                    Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    textTX.font = ArialFont;
                    textTX.color = Color.black;
                    textTX.alignment = TextAnchor.MiddleCenter;
                    textTX.text = setName;

                    AddListenerHead(buttonBU, i);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.name == "Body")
            {
                Camera.main.GetComponent<Animator>().SetBool("Body", true);
                colliders.SetActive(false);
                backCustomize.SetActive(true);

                int k = 0;
                bodies = Resources.LoadAll<GameObject>("Prefabs/Equipment/Body/");
                for (int i = 0; i < bodies.Length; i++)
                {
                    string[] bodyParts = new string[5];
                    string setName = bodies[i].transform.name.Substring(0, bodies[i].transform.name.IndexOf('_'));
                    bodyParts[0] = "Prefabs/Equipment/Body/" + setName + "_body_body";
                    bodyParts[1] = "Prefabs/Equipment/Body/" + setName + "_body_L_elbow";
                    bodyParts[2] = "Prefabs/Equipment/Body/" + setName + "_body_R_elbow";
                    bodyParts[3] = "Prefabs/Equipment/Body/" + setName + "_body_L_shoulder";
                    bodyParts[4] = "Prefabs/Equipment/Body/" + setName + "_body_R_shoulder";
                    GameObject buttonGO = new GameObject();
                    RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
                    buttonRT.SetParent(rightPanel.GetComponent<RectTransform>());
                    buttonRT.sizeDelta = new Vector2(50F, 50F);
                    buttonRT.anchoredPosition = new Vector2((52 * (k + 1) - 48), -4);
                    buttonRT.anchorMin = new Vector2(0, 1);
                    buttonRT.anchorMax = new Vector2(0, 1);
                    buttonRT.pivot = new Vector2(0, 1);
                    Button buttonBU = buttonGO.AddComponent<Button>();
                    Image buttonIM = buttonGO.AddComponent<Image>();
                    buttonIM.sprite = uiSprite.sprite;
                    buttonIM.type = Image.Type.Sliced;
                    buttonBU.targetGraphic = buttonIM;

                    GameObject textGO = new GameObject();
                    textGO.transform.parent = buttonGO.transform;
                    RectTransform textRT = textGO.AddComponent<RectTransform>();
                    textRT.sizeDelta = new Vector2(0F, 0F);
                    textRT.anchorMin = new Vector2(0, 0);
                    textRT.anchorMax = new Vector2(1, 1);
                    textRT.pivot = new Vector2(0.5F, 0.5F);
                    textRT.anchoredPosition = new Vector2(0, 0);
                    Text textTX = textGO.AddComponent<Text>();
                    Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    textTX.font = ArialFont;
                    textTX.color = Color.black;
                    textTX.alignment = TextAnchor.MiddleCenter;
                    textTX.text = setName;

                    AddListenerBody(buttonBU, i, bodyParts);
                    k++;
                    i += 5;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.name == "Pants")
            {
                Camera.main.GetComponent<Animator>().SetBool("Pants", true);
                colliders.SetActive(false);
                backCustomize.SetActive(true);

                int k = 0;
                pants = Resources.LoadAll<GameObject>("Prefabs/Equipment/Pants/");
                for (int i = 0; i < pants.Length; i++)
                {
                    string[] pantsParts = new string[5];
                    string setName = pants[i].transform.name.Substring(0, pants[i].transform.name.IndexOf('_'));
                    pantsParts[0] = "Prefabs/Equipment/Pants/" + setName + "_pants_body";
                    pantsParts[1] = "Prefabs/Equipment/Pants/" + setName + "_pants_L_knee";
                    pantsParts[2] = "Prefabs/Equipment/Pants/" + setName + "_pants_R_knee";
                    pantsParts[3] = "Prefabs/Equipment/Pants/" + setName + "_pants_L_thigh";
                    pantsParts[4] = "Prefabs/Equipment/Pants/" + setName + "_pants_R_thigh";
                    GameObject buttonGO = new GameObject();
                    RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
                    buttonRT.SetParent(rightPanel.GetComponent<RectTransform>());
                    buttonRT.sizeDelta = new Vector2(50F, 50F);
                    buttonRT.anchoredPosition = new Vector2((52 * (k + 1) - 48), -4);
                    buttonRT.anchorMin = new Vector2(0, 1);
                    buttonRT.anchorMax = new Vector2(0, 1);
                    buttonRT.pivot = new Vector2(0, 1);
                    Button buttonBU = buttonGO.AddComponent<Button>();
                    Image buttonIM = buttonGO.AddComponent<Image>();
                    buttonIM.sprite = uiSprite.sprite;
                    buttonIM.type = Image.Type.Sliced;
                    buttonBU.targetGraphic = buttonIM;

                    GameObject textGO = new GameObject();
                    textGO.transform.parent = buttonGO.transform;
                    RectTransform textRT = textGO.AddComponent<RectTransform>();
                    textRT.sizeDelta = new Vector2(0F, 0F);
                    textRT.anchorMin = new Vector2(0, 0);
                    textRT.anchorMax = new Vector2(1, 1);
                    textRT.pivot = new Vector2(0.5F, 0.5F);
                    textRT.anchoredPosition = new Vector2(0, 0);
                    Text textTX = textGO.AddComponent<Text>();
                    Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    textTX.font = ArialFont;
                    textTX.color = Color.black;
                    textTX.alignment = TextAnchor.MiddleCenter;
                    textTX.text = setName;

                    AddListenerPants(buttonBU, i, pantsParts);
                    k++;
                    i += 5;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.name == "Boots")
            {
                Camera.main.GetComponent<Animator>().SetBool("Pants", true);
                colliders.SetActive(false);
                backCustomize.SetActive(true);

                int k = 0;
                boots = Resources.LoadAll<GameObject>("Prefabs/Equipment/Boots/");
                for (int i = 0; i < boots.Length; i++)
                {
                    string[] bootsParts = new string[4];
                    string setName = boots[i].transform.name.Substring(0, boots[i].transform.name.IndexOf('_'));
                    bootsParts[0] = "Prefabs/Equipment/Boots/" + setName + "_boots_L_ankle";
                    bootsParts[1] = "Prefabs/Equipment/Boots/" + setName + "_boots_R_ankle";
                    bootsParts[2] = "Prefabs/Equipment/Boots/" + setName + "_boots_L_knee";
                    bootsParts[3] = "Prefabs/Equipment/Boots/" + setName + "_boots_R_knee";
                    GameObject buttonGO = new GameObject();
                    RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
                    buttonRT.SetParent(rightPanel.GetComponent<RectTransform>());
                    buttonRT.sizeDelta = new Vector2(50F, 50F);
                    buttonRT.anchoredPosition = new Vector2((52 * (k + 1) - 48), -4);
                    buttonRT.anchorMin = new Vector2(0, 1);
                    buttonRT.anchorMax = new Vector2(0, 1);
                    buttonRT.pivot = new Vector2(0, 1);
                    Button buttonBU = buttonGO.AddComponent<Button>();
                    Image buttonIM = buttonGO.AddComponent<Image>();
                    buttonIM.sprite = uiSprite.sprite;
                    buttonIM.type = Image.Type.Sliced;
                    buttonBU.targetGraphic = buttonIM;
                    GameObject textGO = new GameObject();
                    textGO.transform.parent = buttonGO.transform;
                    RectTransform textRT = textGO.AddComponent<RectTransform>();
                    textRT.sizeDelta = new Vector2(0F, 0F);
                    textRT.anchorMin = new Vector2(0, 0);
                    textRT.anchorMax = new Vector2(1, 1);
                    textRT.pivot = new Vector2(0.5F, 0.5F);
                    textRT.anchoredPosition = new Vector2(0, 0);
                    Text textTX = textGO.AddComponent<Text>();
                    Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    textTX.font = ArialFont;
                    textTX.color = Color.black;
                    textTX.alignment = TextAnchor.MiddleCenter;
                    textTX.text = setName;

                    AddListenerBoots(buttonBU, i, bootsParts);
                    k++;
                    i += 4;
                }
            }
        }
    }

    void AddListenerHead(Button b, int id)
    {
        b.onClick.AddListener(() => headButton(id));
    }

    void AddListenerBody(Button b, int id, string[] bodyParts)
    {
        b.onClick.AddListener(() => bodyButton(id,bodyParts));
    }

    void AddListenerPants(Button b, int id, string[] pantsParts)
    {
        b.onClick.AddListener(() => pantsButton(id, pantsParts));
    }

    void AddListenerBoots(Button b, int id, string[] bootsParts)
    {
        b.onClick.AddListener(() => bootsButton(id, bootsParts));
    }

    void headButton(int id)
    {
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        if (!isEquiped[id])
        {
            for (int i = 0; i < heads.Length; i++)
            {
                Destroy(GameObject.Find(heads[i].transform.name + "(Clone)"));
                isEquiped[i] = false;
            }
            GameObject head = Instantiate(heads[id]);
            head.transform.parent = GameObject.Find("Neck").transform;
            head.transform.localPosition = Vector3.zero;
            head.transform.localEulerAngles = Vector3.zero;
            head.transform.localScale = Vector3.one;
            isEquiped[id] = true;
            plyInfo.equipment[0] = "Prefabs/Equipment/Head/"+heads[id].transform.name;
        }
        else
        {
            Destroy(GameObject.Find(heads[id].transform.name + "(Clone)"));
            isEquiped[id] = false;
            plyInfo.equipment[0] = "empty";
        }
    }

    void bodyButton(int id, string[] bodyParts)
    {
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        GameObject[] allBody = Resources.LoadAll<GameObject>("Prefabs/Equipment/Body/");
        if (!isEquipedBody[id])
        {
            for (int i = 0; i < allBody.Length; i++)
            {
                Destroy(GameObject.Find(allBody[i].transform.name + "(Clone)"));
                isEquipedBody = new bool[200];
            }
            GameObject body = Instantiate(Resources.Load(bodyParts[0])) as GameObject;
            GameObject bodyLelbow = Instantiate(Resources.Load(bodyParts[1])) as GameObject;
            GameObject bodyPelbow = Instantiate(Resources.Load(bodyParts[2])) as GameObject;
            GameObject bodyLshoulder = Instantiate(Resources.Load(bodyParts[3])) as GameObject;
            GameObject bodyPshoulder = Instantiate(Resources.Load(bodyParts[4])) as GameObject;

            body.transform.parent = GameObject.Find("Lower_Spine").transform;
            body.transform.localPosition = Vector3.zero;
            body.transform.localEulerAngles = Vector3.zero;

            bodyLelbow.transform.parent = GameObject.Find("L_Elbow").transform;
            bodyLelbow.transform.localPosition = Vector3.zero;
            bodyLelbow.transform.localEulerAngles = Vector3.zero;

            bodyPelbow.transform.parent = GameObject.Find("R_Elbow").transform;
            bodyPelbow.transform.localPosition = Vector3.zero;
            bodyPelbow.transform.localEulerAngles = Vector3.zero;

            bodyLshoulder.transform.parent = GameObject.Find("L_Shoulder").transform;
            bodyLshoulder.transform.localPosition = Vector3.zero;
            bodyLshoulder.transform.localEulerAngles = Vector3.zero;

            bodyPshoulder.transform.parent = GameObject.Find("R_Shoulder").transform;
            bodyPshoulder.transform.localPosition = Vector3.zero;
            bodyPshoulder.transform.localEulerAngles = Vector3.zero;

            isEquipedBody[id] = true;
            for (int i = 0; i < 5; i++)
            {
                plyInfo.equipment[i + 1] = bodyParts[i];
            }
        }
        else
        {
            for (int i = 0; i < bodyParts.Length; i++)
            {
                Destroy(GameObject.Find(bodyParts[i].Substring(23, bodyParts[i].Length - 23) + "(Clone)"));
            }
            isEquipedBody[id] = false;
            for (int i = 0; i < 5; i++)
            {
                plyInfo.equipment[i + 1] = "empty";
            }
        }
    }

    void pantsButton(int id, string[] pantsParts)
    {
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        GameObject[] allPants = Resources.LoadAll<GameObject>("Prefabs/Equipment/Pants/");
        if (!isEquipedPants[id])
        {
            for (int i = 0; i < allPants.Length; i++)
            {
                Destroy(GameObject.Find(allPants[i].transform.name + "(Clone)"));
                isEquipedPants = new bool[200];
            }
            GameObject pantsBody = Instantiate(Resources.Load(pantsParts[0])) as GameObject;
            GameObject pantsLknee = Instantiate(Resources.Load(pantsParts[1])) as GameObject;
            GameObject pantsPknee = Instantiate(Resources.Load(pantsParts[2])) as GameObject;
            GameObject pantsLthigh = Instantiate(Resources.Load(pantsParts[3])) as GameObject;
            GameObject pantsPthigh = Instantiate(Resources.Load(pantsParts[4])) as GameObject;

            pantsBody.transform.parent = GameObject.Find("Lower_Spine").transform;
            pantsBody.transform.localPosition = Vector3.zero;
            pantsBody.transform.localEulerAngles = Vector3.zero;
            pantsBody.transform.localScale = Vector3.one;

            pantsLknee.transform.parent = GameObject.Find("L_Knee").transform;
            pantsLknee.transform.localPosition = Vector3.zero;
            pantsLknee.transform.localEulerAngles = Vector3.zero;

            pantsPknee.transform.parent = GameObject.Find("R_Knee").transform;
            pantsPknee.transform.localPosition = Vector3.zero;
            pantsPknee.transform.localEulerAngles = Vector3.zero;

            pantsLthigh.transform.parent = GameObject.Find("L_Thigh").transform;
            pantsLthigh.transform.localPosition = Vector3.zero;
            pantsLthigh.transform.localEulerAngles = Vector3.zero;

            pantsPthigh.transform.parent = GameObject.Find("R_Thigh").transform;
            pantsPthigh.transform.localPosition = Vector3.zero;
            pantsPthigh.transform.localEulerAngles = Vector3.zero;

            isEquipedPants[id] = true;
            for (int i = 0; i < 5; i++)
            {
                plyInfo.equipment[i + 6] = pantsParts[i];
            }
        }
        else
        {
            for (int i = 0; i < pantsParts.Length; i++)
            {
                Destroy(GameObject.Find(pantsParts[i].Substring(24, pantsParts[i].Length - 24) + "(Clone)"));
            }
            isEquipedPants[id] = false;
            for (int i = 0; i < 5; i++)
            {
                plyInfo.equipment[i + 6] = "empty";
            }
        }
    }

    void bootsButton(int id, string[] bootsParts)
    {
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        GameObject[] allBoots = Resources.LoadAll<GameObject>("Prefabs/Equipment/Boots/");
        if (!isEquipedBoots[id])
        {
            for (int i = 0; i < allBoots.Length; i++)
            {
                Destroy(GameObject.Find(allBoots[i].transform.name + "(Clone)"));
                isEquipedBoots = new bool[200];
            }
            GameObject bootsLankle = Instantiate(Resources.Load(bootsParts[0])) as GameObject;
            GameObject bootsRankle = Instantiate(Resources.Load(bootsParts[1])) as GameObject;
            GameObject bootsLknee = Instantiate(Resources.Load(bootsParts[2])) as GameObject;
            GameObject bootsRknee = Instantiate(Resources.Load(bootsParts[3])) as GameObject;

            bootsLankle.transform.parent = GameObject.Find("L_Ankle").transform;
            bootsLankle.transform.localPosition = Vector3.zero;
            bootsLankle.transform.localEulerAngles = Vector3.zero;
            bootsLankle.transform.localScale = Vector3.one;

            bootsRankle.transform.parent = GameObject.Find("R_Ankle").transform;
            bootsRankle.transform.localPosition = Vector3.zero;
            bootsRankle.transform.localEulerAngles = Vector3.zero;

            bootsLknee.transform.parent = GameObject.Find("L_Knee").transform;
            bootsLknee.transform.localPosition = Vector3.zero;
            bootsLknee.transform.localEulerAngles = Vector3.zero;

            bootsRknee.transform.parent = GameObject.Find("R_Knee").transform;
            bootsRknee.transform.localPosition = Vector3.zero;
            bootsRknee.transform.localEulerAngles = Vector3.zero;

            isEquipedBoots[id] = true;
            for (int i = 0; i < 4; i++)
            {
                plyInfo.equipment[i + 11] = bootsParts[i];
            }
        }
        else
        {
            for (int i = 0; i < bootsParts.Length; i++)
            {
                Destroy(GameObject.Find(bootsParts[i].Substring(24, bootsParts[i].Length - 24) + "(Clone)"));
            }
            isEquipedBoots[id] = false;
            for (int i = 0; i < 4; i++)
            {
                plyInfo.equipment[i + 11] = "empty";
            }
        }
    }

    public void PlaySound(string soundName)
    {
        soundPlayer.PlayOneShot(Resources.Load("Sounds/"+soundName) as AudioClip);
    }

    public void Save()
    {
#if UNITY_STANDALONE
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        plyInfo.nickName = nickName.text;
        float result;
        if (float.TryParse(timelimitInput.text, out result) && result <= 1000 && result >= 180)
        {
            plyInfo.time = result;
        }
        else
        {
            plyInfo.time = 180F;
        }
        FileStream fs = new FileStream("playerInformation.txt", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        sw.WriteLine(nickName.text);
        sw.WriteLine(IpAddress.text);
        for (int i = 0; i < 15; i++)
        {
            sw.WriteLine(plyInfo.equipment[i]);
        }
        for (int i = 0; i < isEquiped.Length; i++)
        {
            if (isEquiped[i])
            {
                sw.Write("1");
            }
            else
            {
                sw.Write("0");
            }
        }
        sw.WriteLine();
        for (int i = 0; i < isEquipedBody.Length; i++)
        {
            if (isEquipedBody[i])
            {
                sw.Write("1");
            }
            else
            {
                sw.Write("0");
            }
        }
        sw.WriteLine();
        for (int i = 0; i < isEquipedPants.Length; i++)
        {
            if (isEquipedPants[i])
            {
                sw.Write("1");
            }
            else
            {
                sw.Write("0");
            }
        }
        sw.WriteLine();
        for (int i = 0; i < isEquipedBoots.Length; i++)
        {
            if (isEquipedBoots[i])
            {
                sw.Write("1");
            }
            else
            {
                sw.Write("0");
            }
        }
        sw.WriteLine();
        sw.WriteLine(plyInfo.time);
        sw.Close();
        fs.Close();
        byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes("playerInformation.txt"));
        string md5 = System.BitConverter.ToString(hash).Replace("-", "");
        File.WriteAllText("MD5", md5);
        //FileStream fs2 = new FileStream("motd.txt", FileMode.Create);
        //StreamWriter sw2 = new StreamWriter(fs2, System.Text.Encoding.UTF8);
        //sw2.WriteLine(motdToggle.isOn.ToString());
        //sw2.WriteLine(motdTitle.text);
        //sw2.WriteLine(motdText.text);
        //sw2.Close();
        //fs2.Close();  
#endif
    }
    public void CustomizeEnter()
    {
        colliders.SetActive(true);
    }

    public void CustomizeLeave()
    {
        colliders.SetActive(false);
        Camera.main.GetComponent<Animator>().ResetTrigger("Head");
        Camera.main.GetComponent<Animator>().ResetTrigger("Body");
        Camera.main.GetComponent<Animator>().ResetTrigger("Pants");
        backCustomize.SetActive(false);
        foreach (Transform child in rightPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BodyPartsLeave()
    {
        Camera.main.GetComponent<Animator>().ResetTrigger("Head");
        Camera.main.GetComponent<Animator>().ResetTrigger("Body");
        Camera.main.GetComponent<Animator>().ResetTrigger("Pants");
        colliders.SetActive(true);
        backCustomize.SetActive(false);
        foreach (Transform child in rightPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Show(string name)
    {
        GameObject.Find(name).GetComponent<Canvas>().enabled = true;
    }

    public void Hide(string name)
    {
        GameObject.Find(name).GetComponent<Canvas>().enabled = false;
    }

    public void RotateLeftDown()
    {
        left = true;
    }
    public void RotateLeftUp()
    {
        left = false;
    }
    public void RotateRightDown()
    {
        right = true;
    }
    public void RotateRightUp()
    {
        right = false;
    }
    public void Loading()
    {
        nextTime = Time.time + 1;
        loading = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
