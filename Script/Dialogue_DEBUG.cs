using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using TMPro;

public class Dialogue_DEBUG : DialogueMasterClass
{
    private void Awake()
    {
        currentDialogueChoicesKey[0] = "0";
        currentDialogueChoicesKey[1] = "1";
        currentDialogueChoicesKey[2] = "2";
        currentDialogueChoicesVal[0] = "says choice 1";
        currentDialogueChoicesVal[1] = "mods choice 2";
        currentDialogueChoicesVal[2] = "getEvi & ends choice 3";
    }
    private void Start()
    {
        playDialogue();
    }
    private void switchToChoices()
    {
        Finished();
        //DialogManager.Result = "";
        playDialogueChoice();
    }
    public override void playDialogue()
    {
        if (startWithChoices)
        {
            playDialogueChoice();
        }
        else if(startWithChoices==false)
        {
            dialogState.setIsInDialog(true);
            isPlaying = true;
            var dialogTexts = new List<DialogData>();

            dialogTexts.Add(new DialogData("/close/", "test", () => DialogueExtra(0, false, "", false)));
            dialogTexts.Add(new DialogData("Hi, my name is Li.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("I am Sa. Popped out to let you know Asset can show other characters.", "test1", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("This Asset, The D'Dialog System has many features.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("You can easily change text /color:red/color, /color:black/and /size:up//size:up/size/size:init/ like this.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("Just put the command in the string!", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("You can also change the character's sprite like this.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("If you need an emphasis effect, /wait:0.2/wait... /click/or click command.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("Text can be /speed:down/slow... /speed:init//speed:up/or fast.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("/speed:0.1/AND YOU CAN'T SKIP THIS SENTENCE.", "test", () => DialogueExtra(0, false, "", true, 0)));

            dialogTexts.Add(new DialogData("And here we go, /sound/the haha sound!.", "test", () => DialogueExtra(0, false, "", false)));

            dialogTexts.Add(new DialogData("That's it! Please check the documents. Good luck to you.", "test", () => switchToChoices()));

            //dialogTexts.Add(new DialogData("", "", () => Finished()));

            startWithChoices = true;
            DialogManager.Show(dialogTexts);
        }
        #region old dialogue code from the demo, used as reference
        /*if (!isPlaying)
        {
            dialogState.setIsInDialog(true);
            isPlaying = true;
            var dialogTexts = new List<DialogData>();

            dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Li."));

            dialogTexts.Add(new DialogData("I am Sa. Popped out to let you know Asset can show other characters."));

            dialogTexts.Add(new DialogData("This Asset, The D'Dialog System has many features."));

            dialogTexts.Add(new DialogData("You can easily change text /color:red/color, /color:white/and /size:up//size:up/size/size:init/ like this."));

            dialogTexts.Add(new DialogData("Just put the command in the string!"));

            dialogTexts.Add(new DialogData("You can also change the character's sprite like this."));

            dialogTexts.Add(new DialogData("If you need an emphasis effect, /wait:0.5/wait... /click/or click command."));

            dialogTexts.Add(new DialogData("Text can be /speed:down/slow... /speed:init//speed:up/or fast."));

            dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/"));

            dialogTexts.Add(new DialogData("/speed:0.1/AND YOU CAN'T SKIP THIS SENTENCE."));

            dialogTexts.Add(new DialogData("And here we go, the haha sound! /click/."));

            dialogTexts.Add(new DialogData("That's it! Please check the documents. Good luck to you.","", () => Finished()));

            //dialogTexts.Add(new DialogData("", "", () => Finished()));

            DialogManager.Show(dialogTexts);
        }*/
        #endregion
    }
    public override void playDialogueChoice()
    {
        if (!isPlaying)
        {
            Debug.Log("choices");
            Debug.Log(DialogManager.Result);
            isPlaying = true;
            dialogState.setIsInDialog(true);

            if (dialogActorGameObj.Length > 0)
            {
                if (dialogActorGameObj[0].activeInHierarchy != true)
                {
                    disableAllActors();
                    dialogActorGameObj[0].SetActive(true);
                }
            }

            var dialogTexts = new List<DialogData>();
            var Text1 = new DialogData("Test?");
            Text1.SelectList.Add(currentDialogueChoicesKey[0], currentDialogueChoicesVal[0]);
            Text1.SelectList.Add(currentDialogueChoicesKey[1], currentDialogueChoicesVal[1]);
            Text1.SelectList.Add(currentDialogueChoicesKey[2], currentDialogueChoicesVal[2]);

            Text1.Callback = () => Check_Correct();

            dialogTexts.Add(Text1);

            DialogManager.Show(dialogTexts);
        }
    }
    
    public void Check_Correct()
    {
        //[Debug]
        //Debug.Log("checking");

        int x = int.Parse(DialogManager.Result);

        //[Debug]
        //Debug.Log(x.GetType() + " "+x);

        //var dialogTexts = new List<DialogData>();
        switch (x)
        {
            case 0:
                var dialogTexts1 = new List<DialogData>();
                dialogTexts1.Add(new DialogData("/close/", "test", () => DialogueExtra(0, false, "", false)));
                dialogTexts1.Add(new DialogData("hello", "test", () => switchToChoices()));
                //dialogTexts.Add(new DialogData("test"));

                DialogManager.Show(dialogTexts1); break;
            case 1:
                var dialogTexts2 = new List<DialogData>();
                dialogTexts2.Add(new DialogData("/close/", "test", () => DialogueExtra(0, false, "", false)));
                dialogTexts2.Add(new DialogData("modified choice 2.", "test", () => DialogueExtra(true, "3", "new choice 2_2", 1)));
                dialogTexts2.Add(new DialogData("/close/", "test", () => switchToChoices()));
                //dialogTexts.Add(new DialogData("test"));
                //dialogTexts.Add(new DialogData("ignore this", "", () => DialogueExtra(true, "3","new choice 2_2", 1)));

                DialogManager.Show(dialogTexts2); break;
            case 2:
                var dialogTexts3 = new List<DialogData>();
                dialogTexts3.Add(new DialogData("/close/", "test", () => DialogueExtra(0, false, "", false)));
                dialogTexts3.Add(new DialogData("side evi popup|custom event 1(not implemented yet)", "test", () => DialogueExtra(0, false, "", false)));
                dialogTexts3.Add(new DialogData("main evi popup|custom event 2(not implemented yet)", "test", () => DialogueExtra(0, false, "", false)));
                dialogTexts3.Add(new DialogData("time to end dialogue", "test", () => Finished(true)));

                DialogManager.Show(dialogTexts3); break;
            case 3:
                var dialogTexts4 = new List<DialogData>();
                dialogTexts4.Add(new DialogData("/close/", "test", () => DialogueExtra(1, false, "", false)));
                dialogTexts4.Add(new DialogData("this is new dialogue", "test1", () => switchToChoices()));
                //dialogTexts.Add(new DialogData("test"));

                DialogManager.Show(dialogTexts4); break;
        }
        /*
        if (DialogManager.Result == "Correct")
        {
            var dialogTexts = new List<DialogData>();

            dialogTexts.Add(new DialogData("You are right.", "", () => Finished()));

            DialogManager.Show(dialogTexts);
        }
        else if (DialogManager.Result == "Wrong")
        {
            var dialogTexts = new List<DialogData>();

            dialogTexts.Add(new DialogData("You are wrong.", "", () => Finished()));

            DialogManager.Show(dialogTexts);
        }
        else
        {
            var dialogTexts = new List<DialogData>();

            dialogTexts.Add(new DialogData("Right. You don't have to get the answer.", "", () => Finished()));

            DialogManager.Show(dialogTexts);
        }*/
    }
    /*
    private void Update()
    {
        //debug purposes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playDialogue();
        }
    }
    */
}
