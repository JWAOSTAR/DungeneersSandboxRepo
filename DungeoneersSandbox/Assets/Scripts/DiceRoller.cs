using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public enum RollTypes
{
    D4 = 0,
    D6,
    D8,
    D10,
    D100,
    D12,
    D20,
    Adv,
    D_Adv,
}

public class DiceRoller : MonoBehaviour
{
    const int camTransformSize = 2;
    const int rollingTransformsSize = 6;
    const int diceListSize = 7;
    int diceCap = 20; 
    [SerializeField]
    Transform gameCamera;
    [SerializeField]
    Transform[] cameraPositions = new Transform[camTransformSize];
    [Space]
    [SerializeField]
    Transform[] initialRollingTransforms = new Transform[rollingTransformsSize];
    //[Space]
    [SerializeField]
    Transform[] dice = new Transform[diceListSize];
    List<GameObject> instantiatedDice = new List<GameObject>();
    List<int> diceValues = new List<int>();
    int currentCamPos = 0;
    List<RollTypes> diceQueue = new List<RollTypes>();
    [SerializeField]
    Text[] diceAmounts = new Text[diceListSize];
    [SerializeField]
    RectTransform m_chatDisplay;
    [SerializeField]
    Sprite[] m_diceSprites = new Sprite[diceListSize];

    void OnValidate()
    {
        if (initialRollingTransforms.Length != rollingTransformsSize)
        {
            Debug.LogWarning("Don't change the 'rollingTransformsSize' field's array size!");
            Array.Resize(ref initialRollingTransforms, rollingTransformsSize);
        }
        if(dice.Length != diceListSize)
        {
            Debug.LogWarning("Don't change the 'dice' field's array size!");
            Array.Resize(ref dice, diceListSize);
        }
        if (diceAmounts.Length != diceListSize)
        {
            Debug.LogWarning("Don't change the 'diceAmounts' field's array size!");
            Array.Resize(ref diceAmounts, diceListSize);
        }
        if (m_diceSprites.Length != diceListSize)
        {
            Debug.LogWarning("Don't change the 'diceSprites' field's array size!");
            Array.Resize(ref m_diceSprites, diceListSize);
        }
        if (cameraPositions.Length != camTransformSize)
        {
            Debug.LogWarning("Don't change the 'cameraPositions' field's array size!");
            Array.Resize(ref cameraPositions, camTransformSize);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameCamera.position = cameraPositions[currentCamPos].position;
        gameCamera.rotation = cameraPositions[currentCamPos].rotation;
        //RollDice((int)DiceType.D20);
        if (File.Exists("./player_profile/dice/active_dice_set.dss"))
        {
            BinaryReader file = new BinaryReader(File.Open("./player_profile/dice/active_dice_set.dss", FileMode.Open));
            for (int i = 0; i < dice.Length; i++)
            {
                if (!file.ReadBoolean())
                {
                    int dieType = file.ReadInt32();
                    dice[dieType].GetChild(1).GetComponent<MeshRenderer>().materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                    dice[dieType].GetChild(1).GetComponent<MeshRenderer>().materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                }
                else
                {
                    int dieType = file.ReadInt32();
                    int byteSize = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(byteSize));
                    dice[dieType].GetChild(1).GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", newTex);
                    dice[dieType].GetChild(1).GetComponent<MeshRenderer>().materials[1].SetTexture("_MainTex", newTex);
                }
            }
            //if (!file.ReadBoolean())
            //{
            //    Color numbers = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
            //    Color die = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
            //    for (int i = 0; i < diceListSize; i++)
            //    {

            //        //for (int j = 0; j < 2; j++)
            //        //{
            //            dice[i].GetChild(0).GetComponent<MeshRenderer>().materials[0].color = numbers;
            //            dice[i].GetChild(0).GetComponent<MeshRenderer>().materials[1].color = die;
            //        //}

            //    }
            //}
        }
        else
        {
            for (int i = 0; i < dice.Length; i++)
            {
                //int dieType = file.ReadInt32();
                dice[i].GetChild(1).GetComponent<MeshRenderer>().materials[0].color = Color.red;
                dice[i].GetChild(1).GetComponent<MeshRenderer>().materials[1].color = Color.black;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.V))
        {
            UpdateDiceValues();
        }

        if(Input.GetKeyUp(KeyCode.G))
        {
            GenerateNewChatLine();
        }

        if(Input.GetKeyUp(KeyCode.C))
        {
            ClearChatDisplay();        }
    }

    public void SwitchCameraView()
    {
        if (currentCamPos + 1 < cameraPositions.Length)
        {
            currentCamPos++;
        }
        else
        {
            currentCamPos = 0;
        }
        gameCamera.position = cameraPositions[currentCamPos].position;
        gameCamera.rotation = cameraPositions[currentCamPos].rotation;
    }

    public void AddDice(int die)
    {
        if (diceQueue.Where(d => d == (RollTypes)die).Count() < diceCap) {
            diceQueue.Add((RollTypes)die);
            diceAmounts[die].text = diceQueue.Where(d => d == (RollTypes)die).Count().ToString();
        }
    }

    public void RemoveDice(int die)
    {
        diceQueue.Remove((RollTypes)die);
        diceAmounts[die].text = diceQueue.Where(d => d == (RollTypes)die).Count().ToString();
    }

    public void RollDiceInQueue()
    {
        foreach (RollTypes d in diceQueue)
        {
            RollDice((int)d);
        }
    }

    void RollDice(int die)
    {
        if (die == (int)RollTypes.D100)
        {
            Transform temp = initialRollingTransforms[Random.Range(0, rollingTransformsSize)];
            instantiatedDice.Add(Instantiate(dice[die - 1].gameObject, new Vector3(temp.position.x, temp.position.y, temp.position.z), new Quaternion(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), temp.rotation.w)));
            instantiatedDice.Add(Instantiate(dice[die].gameObject, new Vector3(temp.position.x, temp.position.y, temp.position.z + 0.25f), new Quaternion(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), temp.rotation.w)));
            instantiatedDice[instantiatedDice.Count - 2].GetComponent<Rigidbody>().AddForce(temp.forward * 250);
            instantiatedDice[instantiatedDice.Count - 1].GetComponent<Rigidbody>().AddForce(temp.forward * 250);
        }
        else if(die == (int)RollTypes.Adv || die == (int)RollTypes.D_Adv)
        {
            Transform temp = initialRollingTransforms[Random.Range(0, rollingTransformsSize)];
            instantiatedDice.Add(Instantiate(dice[6].gameObject, new Vector3(temp.position.x, temp.position.y, temp.position.z), new Quaternion(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), temp.rotation.w)));
            instantiatedDice.Add(Instantiate(dice[6].gameObject, new Vector3(temp.position.x, temp.position.y, temp.position.z + 0.3f), new Quaternion(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), temp.rotation.w)));
            if(die == (int)RollTypes.Adv)
            {
                instantiatedDice[instantiatedDice.Count - 2].GetComponent<Dice>().AdvDAdv = 6;
                instantiatedDice[instantiatedDice.Count - 1].GetComponent<Dice>().AdvDAdv = 7;
            }
            else if(die == (int)RollTypes.D_Adv)
            {
                instantiatedDice[instantiatedDice.Count - 2].GetComponent<Dice>().AdvDAdv = 4;
                instantiatedDice[instantiatedDice.Count - 1].GetComponent<Dice>().AdvDAdv = 5;
            }
            instantiatedDice[instantiatedDice.Count - 2].GetComponent<Rigidbody>().AddForce(temp.forward * 250);
            instantiatedDice[instantiatedDice.Count - 1].GetComponent<Rigidbody>().AddForce(temp.forward * 250);
        }
        else
        {
            Transform temp = initialRollingTransforms[Random.Range(0, rollingTransformsSize)];
            instantiatedDice.Add(Instantiate(dice[die].gameObject, temp.position, new Quaternion(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), temp.rotation.w)));
            instantiatedDice[instantiatedDice.Count - 1].GetComponent<Rigidbody>().AddForce(temp.forward * 250);
        }

    }

    public void RollAdvDAdv(bool disadvantage)
    {
        int AdvDAdv = (disadvantage) ? 2 : 1;
        RollDice((int)RollTypes.D20 + AdvDAdv);
    }

    public void ClearDice()
    {
        foreach(GameObject c in instantiatedDice)
        {
            Destroy(c);
        }
        instantiatedDice.Clear();
        diceValues.Clear();
    }

    public void ClearDiceQueue()
    {
        diceQueue.Clear();
        foreach(Text t in diceAmounts)
        {
            t.text = "0";
        }
    }

    public void UpdateDiceValues()
    {
        diceValues.Clear();
        //Transform topNum = null;
        bool onAdv = false;
        bool nullError = false;
        foreach(GameObject d in instantiatedDice)
        {
            if(d == null)
            {
                UpdateInstanceList();
                nullError = true;
                break;
            }
            //foreach(Transform n in d.GetComponentsInChildren<Transform>())
            //{
            //    if(topNum == null)
            //    {
            //        topNum = n;
            //        continue;
            //    }
            //    if(n.position.y > topNum.position.y)
            //    {
            //        topNum = n;
            //    }
            //}
        
            diceValues.Add(d.GetComponent<Dice>().currentValue);
            if ((d.GetComponent<Dice>().diceType == Dice.DiceType.D10)||(d.GetComponent<Dice>().diceType == Dice.DiceType.D10_10s))
            {

                if (diceValues[diceValues.Count - 1] == 0)
                {
                    diceValues[diceValues.Count - 1] = 10;
                }

                if ((diceValues.Count > 1) && diceValues[diceValues.Count - 1] >= 10)
                {
                    if (diceValues[diceValues.Count - 2] == 10)
                    {
                        diceValues[diceValues.Count - 2] = 0;
                    }

                    diceValues[diceValues.Count - 2] += diceValues[diceValues.Count - 1];
                    diceValues.RemoveAt(diceValues.Count - 1);

                    if (diceValues[diceValues.Count - 1] > 100)
                    {
                        diceValues[diceValues.Count - 1] -= 100;
                    }
                }
            }

            if((d.GetComponent<Dice>().AdvDAdv & 4) == 4)
            {
                if((d.GetComponent<Dice>().AdvDAdv & 2) == 2)
                {
                    if ((d.GetComponent<Dice>().AdvDAdv & 1) == 1)
                    {
                        diceValues[diceValues.Count - 2] = (diceValues[diceValues.Count - 1] > diceValues[diceValues.Count - 2]) ? diceValues[diceValues.Count - 1] : diceValues[diceValues.Count - 2];
                        diceValues.RemoveAt(diceValues.Count - 1);
                    }
                }
                else
                {
                    if ((d.GetComponent<Dice>().AdvDAdv & 1) == 1)
                    {
                        diceValues[diceValues.Count - 2] = (diceValues[diceValues.Count - 1] < diceValues[diceValues.Count - 2]) ? diceValues[diceValues.Count - 1] : diceValues[diceValues.Count - 2];
                        diceValues.RemoveAt(diceValues.Count - 1);
                    }
                }
            }
            //topNum = null;
        }
        if (nullError)
        {
            UpdateDiceValues();
        }
        else
        {
            Debug.Log("Current Value: " + diceValues.Take(diceValues.Count).Sum());
        }

        //return diceValues.Take(diceValues.Count).Sum();
    }

    public void UpdateChatDisplay()
    {
        if (instantiatedDice.Count > 0)
        {
            ClearChatDisplay();
            bool[] active_die_in_line = { false, false, false };
            int currentLine = 0;
            if (m_chatDisplay.childCount < ((instantiatedDice.Count + 1) / 3) + ((((instantiatedDice.Count + 1) % 3) == 0) ? 0 : 1))
            {
                for (int i = (((instantiatedDice.Count + 1) / 3) + ((((instantiatedDice.Count + 1) % 3) == 0) ? 0 : 1) - m_chatDisplay.childCount); i > 0; i--)
                {
                    GenerateNewChatLine();
                }
            }
            for (int j = 0; j < instantiatedDice.Count; j++)
            {
                for (int k = 0; k < active_die_in_line.Length; k++)
                {
                    if (!active_die_in_line[k])
                    {
                        String dieSprite = "<sprite=";
                        switch (instantiatedDice[j].GetComponent<Dice>().diceType)
                        {
                            case Dice.DiceType.D4:
                                {
                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue - 1).ToString() + ">";
                                }
                                break;
                            case Dice.DiceType.D6:
                                {
                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue + 3).ToString() + ">";
                                }
                                break;
                            case Dice.DiceType.D8:
                                {
                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue + 9).ToString() + ">";
                                }
                                break;
                            case Dice.DiceType.D10:
                                {
                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue + 18).ToString() + ">";
                                    active_die_in_line[k] = true;
                                    k++;
                                    if ((j + 1) != instantiatedDice.Count && instantiatedDice[j + 1].GetComponent<Dice>().diceType == Dice.DiceType.D10_10s)
                                    {
                                        int trueVal = (((instantiatedDice[j + 1].GetComponent<Dice>().currentValue / 10) % 10) == 0) ? 0 : ((instantiatedDice[j + 1].GetComponent<Dice>().currentValue / 10) % 10);
                                        dieSprite += "+<sprite=" + ((trueVal) + 27).ToString() + ">";
                                        j++;

                                    }
                                }
                                break;
                            case Dice.DiceType.D12:
                                {
                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue + 37).ToString() + ">";
                                }
                                break;
                            case Dice.DiceType.D20:
                                {

                                    dieSprite += (instantiatedDice[j].GetComponent<Dice>().currentValue + 49).ToString() + ">";
                                }
                                break;
                        }
                        m_chatDisplay.GetChild(currentLine).GetChild(1).GetComponent<TextMeshProUGUI>().text += dieSprite;
        
                        active_die_in_line[k] = true;
                        if ((j + 1) != instantiatedDice.Count)
                        {
                            m_chatDisplay.GetChild(currentLine).GetChild(1).GetComponent<TextMeshProUGUI>().text += "+";
                        }
                        else
                        {
                            m_chatDisplay.GetChild(currentLine).GetChild(1).GetComponent<TextMeshProUGUI>().text += "=";
                            if (active_die_in_line[2])
                            {
                                currentLine++;
                                active_die_in_line[0] = false;
                                active_die_in_line[1] = false;
                                active_die_in_line[2] = false;
                                k = 0;
                            }
                            m_chatDisplay.GetChild(currentLine).GetChild(1).GetComponent<TextMeshProUGUI>().text += diceValues.Take(diceValues.Count).Sum().ToString();
                        }
                        break;
                    }
                    else if(k == (active_die_in_line.Length - 1))
                    {
                        currentLine++;
                        active_die_in_line[0] = false;
                        active_die_in_line[1] = false;
                        active_die_in_line[2] = false;
                        k = 0;
                    }
                }
            }
        }
    }

    void GenerateNewChatLine()
    {
        m_chatDisplay.sizeDelta = new Vector2(m_chatDisplay.sizeDelta.x, m_chatDisplay.GetChild(0).GetComponent<RectTransform>().rect.height* m_chatDisplay.childCount);
        Instantiate(m_chatDisplay.GetChild(m_chatDisplay.childCount - 1), m_chatDisplay);
        for (int i = 0; i < (m_chatDisplay.childCount-1); i++)
        {
            m_chatDisplay.GetChild(i).localPosition = new Vector3(m_chatDisplay.GetChild(i).localPosition.x, m_chatDisplay.GetChild(i).localPosition.y + m_chatDisplay.GetChild(0).GetComponent<RectTransform>().rect.height + 5.0f, 0.0f);
            if (i + 1 < m_chatDisplay.childCount - 1)
            {
                m_chatDisplay.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = m_chatDisplay.GetChild(i + 1).GetChild(1).GetComponent<TextMeshProUGUI>().text;
            }
        }
        m_chatDisplay.GetChild(m_chatDisplay.childCount - 1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ClearChatDisplay()
    {
        m_chatDisplay.GetChild(0).localPosition = m_chatDisplay.GetChild(m_chatDisplay.childCount - 1).localPosition;
        for (int i = m_chatDisplay.childCount - 1; i > 0; i--)
        {
            DestroyImmediate(m_chatDisplay.GetChild(i).gameObject);
        }
        m_chatDisplay.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }

    public void UpdateInstanceList()
    {
        for(int i = (instantiatedDice.Count - 1); i >= 0; i--)
        {
            if(instantiatedDice[i] == null)
            {
                instantiatedDice.RemoveAt(i);
            }
        }
    }
}
