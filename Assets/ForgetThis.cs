using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForgetThis : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public Light theLight;
    public KMBossModule BossModule;

    public MeshRenderer[] stageDisplay;
    public MeshRenderer inputDisplay;
    public MeshRenderer LED;
    public MeshRenderer cbmColor;

    public KMSelectable upButton;
    public KMSelectable downButton;
    public KMSelectable centerButton;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    private bool colorblindModeEnabled;
    public KMColorblindMode colorblindMode;

    public static readonly string[] listFDefault = @"Forget Enigma,Forget Everything,Forget Infinity,Forget Me Not,Forget Them All,Forget This,Four-Card Monte,Purgatory,Simon's Stages,Souvenir,Tallordered Keys,The Time Keeper,Timing is Everything,Turn The Key".Split(',');
                                                     

    int extraz = 0;
    int tix = 0;
    int modulesFun;
    int tpTicks = 0;

    private string[] listF;

    string[] theValues = new string[36]
    {
      "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
      "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };

    string[] lightColors = new string[6]
    {
        "Cyan", "Magenta", "Yellow", "Black", "White", "Green"
    };

    Color[] colory = { new Color(0.0f, 1f, 1f), new Color(1f, 0f, 1f), new Color(1f, 1f, 0f), new Color(0f, 0f, 0f), new Color(1f, 1f, 1f), new Color(0f, 1f, 0f) };

    List<int> stageNumbers = new List<int>();
    List<int> stageColors = new List<int>();


    bool canPress = true;
    bool autoSolved = false;
    bool isSolved = false;
    bool canSolve = false;
    bool doRotate = false;
    bool newIncrement = false;
    bool justAfterStrike = false;

    float timeX = 0;
    int curStageNum = 0;
    int numSolvables = 0;
    string coolString = "Stage roll call, in groups of five: ";
    int curAnswer = 0;
    int curRotation = 0;
    int theSolution = 0;

    int curSolved = -1;

    int[] finalStage = new int[5]
    {
        0, 0, 0, 0, 0
    };

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        colorblindModeEnabled = colorblindMode.ColorblindModeActive;
        listF = BossModule.GetIgnoredModules(Module, listFDefault);
        Init();
        canPress = true;
        float scalar = transform.lossyScale.x;
        theLight.range *= scalar;
        theLight.enabled = false;
    }

    void Init()
    {
        for (int i = 0; i < 5; i++)
        {
            stageDisplay[i].GetComponentInChildren<TextMesh>().text = "";
        }
        inputDisplay.GetComponentInChildren<TextMesh>().text = "";
        curSolved = 0;
        numSolvables = Bomb.GetSolvableModuleNames().Where(x => !listF.Contains(x)).Count() + extraz;
        //Debug.Log(Bomb.GetSolvableModuleNames().Where(x => !listF.Contains(x)).ToString());
        //Debug.Log(Bomb.GetSolvableModuleNames());
        //var xxxxx = String.Join(", ", Bomb.GetSolvableModuleNames());
        //Debug.Log("Solviez " + String.Join(", ", Bomb.GetSolvableModuleNames().ToArray()));


        //numSolvables = 57; //debug
        if (numSolvables < 2)
        {
            Debug.LogFormat("[Forget This #{0}] Auto-solving, not enough non-List F modules exist (need 2 or more)", _moduleId);
            autoSolved = true;
            isSolved = true;
        }
        else
        {
            Debug.LogFormat("[Forget This #{0}] {1} (Row C is the first letter of the stage's color, V is the stage's value)", _moduleId, coolString);
            for (int i = 1; i <= numSolvables; i++)
            {
                //var iZero = i - 1;
                stageNumbers.Add(UnityEngine.Random.Range(0, 36));
                stageColors.Add(UnityEngine.Random.Range(0, 5));
                if (numSolvables == i)
                {
                    //coolString = coolString + "and " + i + " is " + lightColors[(int)stageColors[iZero]] + " " + theValues[(int)stageNumbers[iZero]] + " (worth " + stageNumbers[iZero] + ").";
                }
                else
                { 
                    //coolString = coolString + i + " is " + lightColors[(int)stageColors[iZero]] + " " + theValues[(int)stageNumbers[iZero]] + " (worth " + stageNumbers[iZero] + "), ";
                }

            }
            //numSolvables = 7;
            if (numSolvables > 5)
            {
                finalStage[0] = UnityEngine.Random.Range(2, numSolvables);
                finalStage[1] = finalStage[0];
                finalStage[2] = finalStage[0];
                finalStage[3] = finalStage[0];
                finalStage[4] = finalStage[0];
                while (finalStage[1] == finalStage[0])
                {
                    finalStage[1] = UnityEngine.Random.Range(2, numSolvables);
                }
                while (finalStage[2] == finalStage[0] || finalStage[2] == finalStage[1])
                {
                    finalStage[2] = UnityEngine.Random.Range(2, numSolvables);
                }
                while (finalStage[3] == finalStage[0] || finalStage[3] == finalStage[1] || finalStage[3] == finalStage[2])
                {
                    finalStage[3] = UnityEngine.Random.Range(2, numSolvables);
                }
                while (finalStage[4] == finalStage[0] || finalStage[4] == finalStage[1] || finalStage[4] == finalStage[2] || finalStage[4] == finalStage[3])
                {
                    finalStage[4] = UnityEngine.Random.Range(2, numSolvables);
                }
            }
            else
            {
                finalStage[0] = UnityEngine.Random.Range(2, 1 + numSolvables);
                finalStage[1] = UnityEngine.Random.Range(2, 1 + numSolvables);
                finalStage[2] = UnityEngine.Random.Range(2, 1 + numSolvables);
                finalStage[3] = UnityEngine.Random.Range(2, 1 + numSolvables);
                finalStage[4] = UnityEngine.Random.Range(2, 1 + numSolvables);
            }
            var twenties = (int)(numSolvables / 20);
            var numberGroup = "";
            var colorGroup = "";
            var tN = 0;
            var unitNum = 0;
            
            for (tN = 0; tN < twenties; tN++)
            {
                for (unitNum = 0; unitNum < 20; unitNum++)
                {
                    numberGroup = numberGroup + theValues[stageNumbers[(tN * 20) + unitNum]];
                    colorGroup = colorGroup + lightColors[stageColors[(tN * 20) + unitNum]].Substring(0, 1);
                    if (unitNum > 0 && unitNum % 5 == 4)
                    {
                        numberGroup = numberGroup + " ";
                        colorGroup = colorGroup + " ";
                    }
                }

                //Debug.LogFormat("{1}", _moduleId, colorGroup);

                Debug.LogFormat("[Forget This #{0}] Stages {1} to {2}", _moduleId, 1 + (20 * tN), 20 + (20 * tN));
                Debug.LogFormat("[Forget This #{0}] C: {1}", _moduleId, colorGroup);
                Debug.LogFormat("[Forget This #{0}] V: {1}", _moduleId, numberGroup);
                Debug.LogFormat("[Forget This #{0}]", _moduleId);
                numberGroup = "";
                colorGroup = "";
            }
            for (unitNum = 0; unitNum < (numSolvables - (20 * (int)(numSolvables / 20))); unitNum++)
            {
                numberGroup = numberGroup + theValues[stageNumbers[(tN * 20) + unitNum]];
                colorGroup = colorGroup + lightColors[stageColors[(tN * 20) + unitNum]].Substring(0, 1);
                if (unitNum > 0 && unitNum % 5 == 4)
                {
                    numberGroup = numberGroup + " ";
                    colorGroup = colorGroup + " ";
                }
            }

            Debug.LogFormat("[Forget This #{0}] Stages {1} to {2}", _moduleId, 1 + (20 * tN), unitNum + (20 * tN));
            Debug.LogFormat("[Forget This #{0}] C: {1}", _moduleId, colorGroup);
            Debug.LogFormat("[Forget This #{0}] V: {1}", _moduleId, numberGroup);
            Debug.LogFormat("[Forget This #{0}]", _moduleId);
            numberGroup = "";
            colorGroup = "";
            Debug.LogFormat("[Forget This #{0}] Stages to implement, in the order in which they should be implemented: {1}, {2}, {3}, {4}, and {5}.", _moduleId, finalStage[0], finalStage[1],
                finalStage[2], finalStage[3], finalStage[4]);
            doSolution();
        }


        upButton.OnInteract += delegate () { OnPress(); doUp(); upButton.AddInteractionPunch(0.2f); return false; };
        downButton.OnInteract += delegate () { OnPress(); doDown();  downButton.AddInteractionPunch(0.2f); return false; };
        centerButton.OnInteract += delegate () { OnPress(); doSubmit();  downButton.AddInteractionPunch(0.2f); return false; };

        upButton.OnInteractEnded += delegate () { OnRelease(); };
        downButton.OnInteractEnded += delegate () { OnRelease(); };
        centerButton.OnInteractEnded += delegate () { OnRelease(); };
        Module.OnActivate += delegate { doActivationStuff(); };
    }

    void doUp()
    {
        if (canSolve)
        {
            curAnswer++;
            if (curAnswer > 35)
            {
                curAnswer -= 36;
            }
            inputDisplay.GetComponentInChildren<TextMesh>().text = theValues[curAnswer];
        }
        else if (justAfterStrike)
        {
            doRotate = false;
            curRotation++;
            if (curRotation > numSolvables)
            {
                curRotation = curRotation - numSolvables;
            }
            LED.material.color = colory[stageColors[curRotation - 1]];
            theLight.color = colory[stageColors[curRotation - 1]];
            if (colorblindModeEnabled)
            {
                cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curRotation - 1]].Substring(0, 1);
            }
            if (curRotation < 10)
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "00" + curRotation;
            }
            else if (curRotation < 100)
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "0" + curRotation;
            }
            else
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "" + curRotation;
            }


            stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[curRotation - 1]] + " ";
        }


    }

    void doDown()
    {
        if (canSolve)
        {
                curAnswer--;
                if (curAnswer < 0)
                {
                    curAnswer += 36;
                }
                inputDisplay.GetComponentInChildren<TextMesh>().text = theValues[curAnswer];
         }
        else if (justAfterStrike)
        {
            doRotate = false;
            curRotation--;
            if (curRotation < 1)
            {
                curRotation = numSolvables;
            }
            LED.material.color = colory[stageColors[curRotation - 1]];
            theLight.color = colory[stageColors[curRotation - 1]];
            if (colorblindModeEnabled)
            {
                cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curRotation - 1]].Substring(0, 1);
            }
            if (curRotation < 10)
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "00" + curRotation;
            }
            else if (curRotation < 100)
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "0" + curRotation;
            }
            else
            {
                stageDisplay[0].GetComponentInChildren<TextMesh>().text = "" + curRotation;
            }


            stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[curRotation - 1]] + " ";
        }


    }

    void doSubmit()
    {
        if (!isSolved)
        {
            if (canSolve)
            {
                if (curAnswer == theSolution) //Yippee!
                {
                    canPress = false;
                    canSolve = false;
                    isSolved = true;
                    LED.material.color = colory[5];
                    theLight.color = colory[5];
                    Debug.LogFormat("[Forget This #{0}] Input of {1} and the final result {2} match, you didn't forget it! Module disarmed!", _moduleId,
                        theValues[curAnswer], theValues[theSolution]);
                    for (int i = 0; i < 5; i++)
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "YES";

                    }
                    GetComponent<KMBombModule>().HandlePass();

                }
                else //d'oh
                {
                    doRotate = true;
                    canSolve = false;
                    justAfterStrike = true;
                    tix = 0;
                    Debug.LogFormat("[Forget This #{0}] Input of {1} and the final result {2} didn't match, strike given and stage implementation order shuffled!", _moduleId, 
                        theValues[curAnswer], theValues[theSolution]);
                    GetComponent<KMBombModule>().HandleStrike();
                    stageDisplay[1].GetComponentInChildren<TextMesh>().text = " ";
                    stageDisplay[2].GetComponentInChildren<TextMesh>().text = " ";
                    stageDisplay[3].GetComponentInChildren<TextMesh>().text = " ";
                    LED.material.color = colory[stageColors[0]];
                    theLight.color = colory[stageColors[0]];

                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "001";
                    stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[0]] + " ";
                    if (colorblindModeEnabled)
                    {
                        cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[0]].Substring(0, 1);
                    }
                }
            }
            else if (justAfterStrike)
            {
                doRotate = false;
                justAfterStrike = false;
                canSolve = true;
                if (numSolvables > 5)
                {
                    finalStage[0] = UnityEngine.Random.Range(2, numSolvables);
                    finalStage[1] = finalStage[0];
                    finalStage[2] = finalStage[0];
                    finalStage[3] = finalStage[0];
                    finalStage[4] = finalStage[0];
                    while (finalStage[1] == finalStage[0])
                    {
                        finalStage[1] = UnityEngine.Random.Range(2, numSolvables);
                    }
                    while (finalStage[2] == finalStage[0] || finalStage[2] == finalStage[1])
                    {
                        finalStage[2] = UnityEngine.Random.Range(2, numSolvables);
                    }
                    while (finalStage[3] == finalStage[0] || finalStage[3] == finalStage[1] || finalStage[3] == finalStage[2])
                    {
                        finalStage[3] = UnityEngine.Random.Range(2, numSolvables);
                    }
                    while (finalStage[4] == finalStage[0] || finalStage[4] == finalStage[1] || finalStage[4] == finalStage[2] || finalStage[4] == finalStage[3])
                    {
                        finalStage[4] = UnityEngine.Random.Range(2, numSolvables);
                    }
                }
                else
                {
                    finalStage[0] = UnityEngine.Random.Range(2, 1 + numSolvables);
                    finalStage[1] = UnityEngine.Random.Range(2, 1 + numSolvables);
                    finalStage[2] = UnityEngine.Random.Range(2, 1 + numSolvables);
                    finalStage[3] = UnityEngine.Random.Range(2, 1 + numSolvables);
                    finalStage[4] = UnityEngine.Random.Range(2, 1 + numSolvables);
                }
                Debug.LogFormat("[Forget This #{0}] Stages to implement, in the order in which they should be implemented: {1}, {2}, {3}, {4}, and {5}.", _moduleId, finalStage[0], finalStage[1],
                    finalStage[2], finalStage[3], finalStage[4]);
                doSolution();
                curAnswer = UnityEngine.Random.Range(0, 36);
                inputDisplay.GetComponentInChildren<TextMesh>().text = theValues[curAnswer];
                LED.material.color = colory[4];
                theLight.color = colory[4];
                if (colorblindModeEnabled)
                {
                    cbmColor.GetComponentInChildren<TextMesh>().text = "";
                }
                for (int i = 0; i < 5; i++)
                {
                    if (finalStage[i] < 10)
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "00" + finalStage[i];
                    }
                    else if (finalStage[i] < 100)
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "0" + finalStage[i];
                    }
                    else
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "" + finalStage[i];
                    }
                }
            }
            else
            {
                // debug doNextStage();
            }
        }

    }

    void doNextStage()
    {
        if (!isSolved)
        {
            //Debug.LogFormat("[Forget This #{0}] curStageNum is {1} and new numSolvables is {2}.", _moduleId, curStageNum, numSolvables);
            //Debug.LogFormat("[Forget This #{0}] canSolve is {1}.", _moduleId, canSolve);
            if (curStageNum == numSolvables && !canSolve && !isSolved) // Ready to solve!
            {
                curAnswer = UnityEngine.Random.Range(0, 36);
                inputDisplay.GetComponentInChildren<TextMesh>().text = theValues[curAnswer];
                LED.material.color = colory[5];
                theLight.color = colory[5];
                if (colorblindModeEnabled)
                {
                    cbmColor.GetComponentInChildren<TextMesh>().text = "";
                }
                for (int i = 0; i < 5; i++)
                {
                    if (finalStage[i] < 10)
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "00" + finalStage[i];
                    }
                    else if (finalStage[i] < 100)
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "0" + finalStage[i];
                    }
                    else
                    {
                        stageDisplay[i].GetComponentInChildren<TextMesh>().text = "" + finalStage[i];
                    }
                }
                canSolve = true;
                doRotate = false;
                newIncrement = false;
                timeX = 0;
            }
            else
            {
                curStageNum++;
                //Debug.LogFormat("[Forget This #{0}] adding one to curStageNum and it is now {1} and SolveCount is still {2}.", _moduleId, curStageNum, curSolved);
                if (!canSolve && !isSolved)
                {
                    stageDisplay[2].GetComponentInChildren<TextMesh>().text = "" + (curSolved + 1 - curStageNum);
                }
                LED.material.color = colory[stageColors[curStageNum - 1]];
                theLight.color = colory[stageColors[curStageNum - 1]];
                if (colorblindModeEnabled)
                {
                    cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curStageNum - 1]].Substring(0, 1);
                }
                if (curStageNum < 10)
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "00" + curStageNum;
                }
                else if (curStageNum < 100)
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "0" + curStageNum;
                }
                else
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "" + curStageNum;
                }

                stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[curStageNum - 1]] + " ";
            }
        }
       

    }

    void FixedUpdate()
    {
        curSolved = Bomb.GetSolvedModuleNames().Where(x => !listF.Contains(x)).Count();







        if (newIncrement && !(curStageNum == curSolved + 1) && !canSolve && !isSolved && !doRotate && !justAfterStrike)
        {
            if (timeX > 0)
            {
                timeX -= Time.fixedDeltaTime;
            }
            else if (timeX < 0)
            {
                timeX = 0;
            }
            else
            {
                timeX = 2;
                doNextStage();
                if (curStageNum == curSolved + 1)
                {
                    timeX = 0;
                    newIncrement = false;
                }

            }

            if (!canPress)
            {
                return;
            }
        }
        else
        {
            if (curStageNum < curSolved + 1 && curSolved > 0 && !canSolve && !isSolved && !doRotate && !justAfterStrike)
            {
                timeX = 2;
                newIncrement = true;
                //Debug.LogFormat("[Forget This #{0}] newIncrement is {1} and curStageNum is {2} and curSolved is {3} .", _moduleId, newIncrement, curStageNum, curSolved);
                if (!canSolve && !isSolved && !doRotate && !justAfterStrike)
                {
                    stageDisplay[2].GetComponentInChildren<TextMesh>().text = "" + (curSolved + 1 - curStageNum);
                }
            }


            /*
            if (curStageNum < Bomb.GetSolvedModuleNames().Where(x => !listF.Contains(x)).Count() + extraz + 1 && Bomb.GetSolvedModuleNames().Where(x => !listF.Contains(x)).Count() > 0
                && !(curStageNum == numSolvables))
            {
                Debug.LogFormat("[Forget This #{0}] Detected solves. curStageNum is {1} and new SolveCount is {2}.", _moduleId, curStageNum,
                    Bomb.GetSolvedModuleNames().Where(x => !listF.Contains(x)).Count() + extraz + 1);
                stageDisplay[2].GetComponentInChildren<TextMesh>().text = "" + (Bomb.GetSolvedModuleNames().Where(x => !listF.Contains(x)).Count() + extraz + 1 - curStageNum);


                newIncrement = true;
                timeX = 2;
            }*/
        }
        
        tix++;
        if (canSolve && !isSolved)
        {
            if (tix > 41)
            {
                LED.material.color = colory[5];
                theLight.color = colory[5];
            }
            else
            {
                LED.material.color = colory[3];
                theLight.color = colory[3];
            }
        }
        if (tix == 85)
        {
            tix = 0;
            if (doRotate)
            {
                curRotation++;
                if (curRotation > numSolvables)
                {
                    curRotation = curRotation - numSolvables;
                }
                LED.material.color = colory[stageColors[curRotation - 1]];
                theLight.color = colory[stageColors[curRotation - 1]];
                if (colorblindModeEnabled)
                {
                    cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curRotation - 1]].Substring(0, 1);
                }

                if (curRotation < 10)
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "00" + curRotation;
                }
                else if (curRotation < 100)
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "0" + curRotation;
                }
                else
                {
                    stageDisplay[0].GetComponentInChildren<TextMesh>().text = "" + curRotation;
                }


                stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[curRotation - 1]] + " ";
            }
            else
            {


            }
        }
            //Debug.Log(timeX);
            /*
            if (Display == null)
            {
                DisplayMesh.text = "";
                DisplayMeshBig.text = "";
                StageMesh.text = "";
                
            }
            else
            {
                int progress = Bomb.GetSolvedModuleNames().Where(x => !listM.Contains(x)).Count() + extraz;
                if (progress > displayCurStage)
                {
                    if (timeX <= 0)
                    {
                        timeX = STAGE_DELAY;
                        displayCurStage++;
                    }
                    progress = displayCurStage;
                }
                if (progress >= Display.Length)
                {
                    StageMesh.text = "--";
                    if (!done)
                    {
                        UpdateDisplayMesh(-1);
                        done = true;
                    }
                }
                else
                {
                    int stage = (progress + 1) % 100;
                    if (stage < 10)
                    {
                        if (Display.Length < 10) StageMesh.text = "" + stage;
                        else StageMesh.text = "0" + stage;
                    }
                    else StageMesh.text = "" + stage;

                    UpdateDisplayMesh(progress);
                }
            }
            */
        
    }

    void OnPress()
    {

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    void OnRelease()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);

    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Submit an answer when the module is ready with !{0} (submit/answer) 0. The answer must be a single digit from 0-9 and from A-Z. Use !{0} stage 999 to see a particular stage or !{0} reset to try again after a strike. !{0} colorblind enables colorblind mode.";
    private readonly bool TwitchShouldCancelCommand = false;
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {

        var pieces = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string theError;
        theError = "";
        yield return null;
        if (canSolve)
        {
            if (pieces.Count() == 0)
            {
                theError = "sendtochaterror Not enough arguments! You need 'submit/answer' with a number after.";
                yield return theError;
            }
            else if (pieces.Count() >= 1 && pieces[0] == "colorblind")
            {
                colorblindModeEnabled = true;
                yield return null;
            }
            else if (pieces.Count() == 1 && (pieces[0] == "submit" || pieces[0] == "answer"))
            {
                theError = "sendtochaterror Not enough arguments! You need a number to submit, e.g. !{0} submit 0.";
                yield return theError;

            }
            else if (pieces.Count() == 1)
            {

                theError = "sendtochaterror Invalid arguments! You need to submit/answer, e.g. !{0} submit 0.";
                yield return theError;
            }
            else if (pieces.Count() >= 2 && (pieces[0] == "submit" || pieces[0] == "answer"))
            {
                if (pieces[1].Length == 1)
                {
                    int isBad = 99;
                    for (int i = 0; i < 36; i++)
                    {
                        if (pieces[1].ToUpperInvariant() == theValues[i])
                        {
                            isBad = i;
                        }
                    }
                    if (isBad == 99)
                    {
                        theError = "sendtochaterror Invalid digit! It must be a digit from 0-9 or from A-Z.";
                        yield return theError;
                    }
                    else
                    {
                        tpTicks = isBad - curAnswer;
                        if (tpTicks > 18)
                        {
                            tpTicks -= 36;
                        }
                        else if (tpTicks < -18)
                        {
                            tpTicks += 36;
                        }
                        while (!(tpTicks == 0))
                        {
                            yield return new WaitForSeconds(.1f);
                            yield return null;
                            if (tpTicks > 0)
                            {
                                upButton.OnInteract();
                                tpTicks--;
                            }
                            else
                            {
                                downButton.OnInteract();
                                tpTicks++;
                            }
                        }
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        centerButton.OnInteract();
                    }

                }
                else
                {
                    theError = "sendtochaterror Invalid digit! It must be a single-character digit from 0-9 or from A-Z.";
                    yield return theError;
                }

            }
            else
            {

            }


        }
        else if (justAfterStrike)
        {
            if (pieces.Count() == 0)
            {
                theError = "sendtochaterror Not enough arguments! You need 'reset' to try again.";
                yield return theError;
            }
            else if (pieces.Count() >= 1 && pieces[0] == "colorblind")
            {
                colorblindModeEnabled = true;
                cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curRotation - 1]].Substring(0, 1);
                yield return null;
            }
            else if (pieces.Count() >= 2 && pieces[0] == "stage")
            {
                if (Int16.Parse(pieces[1]) == 0 || Int16.Parse(pieces[1]) > numSolvables)
                {
                    theError = "sendtochaterror " + pieces[1] + " is an invalid stage!";
                    yield return theError;
                }
                else
                {

                    tpTicks = Int16.Parse(pieces[1]) - curRotation;
                    int xyz = numSolvables / 2;
                    if (tpTicks > xyz)
                    {
                        tpTicks -= numSolvables;
                    }
                    else if (tpTicks < -xyz)
                    {
                        tpTicks += numSolvables;
                    }
                    while (!(tpTicks == 0))
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        if (tpTicks > 0)
                        {
                            upButton.OnInteract();
                            tpTicks--;
                        }
                        else
                        {
                            downButton.OnInteract();
                            tpTicks++;
                        }
                    }
                }

            }
            else if (pieces.Count() >= 1 && !(pieces[0] == "reset"))
            {

                theError = "sendtochaterror Invalid argument! You need to 'reset' to try again.";
                yield return theError;
            }
            else if (pieces.Count() >= 1 && pieces[0] == "reset")
            {
                yield return new WaitForSeconds(.1f);
                yield return null;
                centerButton.OnInteract();
            }
            else
            {
                theError = "sendtochaterror This probably shouldn't be seen on TP but if it is contact @JerryEris#6034 and tell him about it.";
                yield return theError;
            }

        }
        else if (pieces.Count() >= 1 && pieces[0] == "colorblind")
        {
            colorblindModeEnabled = true;
            cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[curStageNum - 1]].Substring(0, 1);
            yield return null;
        }
        else
        {

            theError = "sendtochaterror Can't solve yet! Wait for the five stages to implement to appear.";
            yield return theError;
        }
    }

    void doSolution()
    {
        theSolution = stageNumbers[0];

        Debug.LogFormat("[Forget This #{0}] Starting with Stage 1's value of {1}. Pointy brackets indicated decimal values, e.g. {1} is equal to <{2}>.", _moduleId, theValues[stageNumbers[0]], stageNumbers[0]);
        for (int i = 0; i < 5; i++)
        { //i = 2, sol = 11
            coolString = "Stage #" + finalStage[i] + "...";
            //finalStage[2] = 8
            coolString = coolString + " Stage color is " + lightColors[stageColors[finalStage[i] - 1]] + " and last stage color was " + lightColors[stageColors[finalStage[i] - 2]] + ". ";
            
            if ((stageColors[finalStage[i] - 1] + 2) % 5 == stageColors[finalStage[i] - 2])
            {

                coolString = coolString + "Don't do anything here! The solution stays at " + theValues[theSolution] + " <" + theSolution + ">.";
                //loggy thing
            }
            else
            {
                coolString = coolString + "Solution so far is " + theValues[theSolution] + " <" + theSolution + ">. ";
                switch (stageColors[finalStage[i] - 1])
                {
                    case 0:     //cyan
                        coolString = coolString + "Add " + theValues[stageNumbers[finalStage[i] - 1]] + " <" + stageNumbers[finalStage[i] - 1] + "> to get <" + 
                            (theSolution + stageNumbers[finalStage[i] - 1]) + ">.";
                        theSolution = theSolution + stageNumbers[finalStage[i] - 1];
                        break;
                    case 1:     //magenta
                        coolString = coolString + "Add (half of " + theValues[stageNumbers[finalStage[i] - 1]] + " <" + stageNumbers[finalStage[i] - 1] + "> rounded down, or <" +
                            stageNumbers[finalStage[i] - 1] / 2 + ">), to get <" + (theSolution + (int)(stageNumbers[finalStage[i] - 1] / 2)) + ">.";
                        theSolution = theSolution + (int)(stageNumbers[finalStage[i] - 1] / 2);
                        break;
                    case 2:     //yellow
                        coolString = coolString + "Add (double " + theValues[stageNumbers[finalStage[i] - 1]] + " <" + stageNumbers[finalStage[i] - 1] + ">, or <" 
                            + stageNumbers[finalStage[i] - 1] * 2 + ">), to get <" + (theSolution + (stageNumbers[finalStage[i] - 1] * 2)) + ">.";
                        theSolution = theSolution + (stageNumbers[finalStage[i] - 1] * 2);
                        break;
                    case 3:     //black
                        coolString = coolString + "Average with " + theValues[stageNumbers[finalStage[i] - 1]] + " <" + stageNumbers[finalStage[i] - 1] + ">, rounding up, to get ";
                        theSolution = theSolution + stageNumbers[finalStage[i] - 1];
                        if (theSolution % 2 == 1)
                        {
                            theSolution++;
                        }
                        theSolution = theSolution / 2;
                        coolString = coolString + "<" + theSolution + ">.";
                        break;
                    case 4:     //white
                        coolString = coolString + "Subtract " + theValues[stageNumbers[finalStage[i] - 1]] + " <" + stageNumbers[finalStage[i] - 1] + "> to get <" + 
                            (theSolution - stageNumbers[finalStage[i] - 1]) + ">.";
                        theSolution = theSolution - stageNumbers[finalStage[i] - 1];
                        break;
                    default:    //uh oh
                        break;
                }
                if (theSolution < 0)
                {
                    theSolution += 36;
                    coolString = coolString + " This is negative, so add <36> to get <" + theSolution + "> or " + theValues[theSolution] + ".";
                }
                else if (theSolution > 35)
                {

                    coolString = coolString + " This is 10 <36> or higher, so take <" + theSolution + "> mod <36> to get <" + theSolution % 36 + "> or " + theValues[theSolution % 36] + ".";
                    theSolution = theSolution % 36;
                }
                else
                {
                    coolString = coolString + " This is " + theValues[theSolution] + " is base 36.";
                }
            }

            Debug.LogFormat("[Forget This #{0}] {1}", _moduleId, coolString);
        }
            Debug.LogFormat("[Forget This #{0}] The solution should be {1} <{2}>", _moduleId, theValues[theSolution], theSolution);
    }

    void doActivationStuff()
    {
        if (autoSolved)
        {
            GetComponent<KMBombModule>().HandlePass();
            canPress = false;
            isSolved = true;
            theLight.enabled = true;
            LED.material.color = colory[5];
            theLight.color = colory[5];
        }
        else
        {
            curStageNum = 1;
            theLight.enabled = true;
            LED.material.color = colory[stageColors[0]];
            theLight.color = colory[stageColors[0]];

            stageDisplay[0].GetComponentInChildren<TextMesh>().text = "001";
            stageDisplay[4].GetComponentInChildren<TextMesh>().text = " " + theValues[stageNumbers[0]] + " ";
            if (colorblindModeEnabled)
            {
                cbmColor.GetComponentInChildren<TextMesh>().text = lightColors[stageColors[0]].Substring(0, 1);
            }
            

        }
    }
}