using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using TMPro;

public class DialogueMasterClass : MonoBehaviour
{
    #region DialogueMasterClass Variables
    [Header("System Manager Game Objects")]
    [Tooltip("The DD'System Dialog Manager")]
    public DialogManager DialogManager;
    [Tooltip("The Manager to check for dialogue state, not necessary, part of old code")]
    public DialogStateManager dialogState;

    //part of the old code
    //public TextMeshProUGUI nameText;
    //public string actorName = "test";

    [Header("Dialogue Character models")]
    [Tooltip("Character models to enable and disable for dialogue")]
    public GameObject[] dialogActorGameObj;

    [Header("Dialogue Choices")]
    [Tooltip("If you want to start with dialogue choices first")]
    public bool startWithChoices;
    [Tooltip("Keys for the dialogue choice, aka the choice's key identifier in the choice list")]
    public string[] currentDialogueChoicesKey;
    [Tooltip("Dialogue text for the dialogue choices")]
    public string[] currentDialogueChoicesVal;

    [Header("SFX")]
    [Tooltip("If you want to use a custom SFX for the /sound/ command")]
    public bool useCustomSE;
    [Tooltip("The array for containing the custom SFX clips")]
    public AudioClip[] CustomSEaudioClip;

    [Header("Dialogue State")]
    public bool isPlaying = false;

    //part of the old code
    //public GameObject sideEvidencePopup;
    //public GameObject evidencePopup;
    #endregion

    //part of the old code
    /*
    public void setNameTextVisbility(bool x)
    {
        nameText.gameObject.SetActive(x);
    }
    public virtual void setNameText(string x)
    {
        nameText.text = x;
    }
    */

    public virtual void setStartWithChoices(bool x)
    {
        startWithChoices = x;
    }
    public virtual void setCustomSE(int index)
    {
        if (useCustomSE && CustomSEaudioClip.Length > 0)
        {
            DialogManager.SFXAudio.resource = CustomSEaudioClip[index];
        }
    }
    /// <summary>
    /// Disables all character models in dialogActorGameObj array variable from being shown
    /// </summary>
    public virtual void disableAllActors()
    {
        foreach(GameObject x in dialogActorGameObj)
        {
            x.SetActive(false);
        }
    }
    //simulates auto skipping, very rough and ghetto way of doing it
    public virtual void autoClick()
    {
        DialogManager.Click_Window();
    }
    /// <summary>
    /// Helper method to help add on other things that we need to execute, this one is to modify the dialogue choices
    /// </summary>
    /// <param name="modifyChoices">Want to modify dialogue choices?</param>
    /// <param name="newKey">New key identifier for the dialogue</param>
    /// <param name="newTextVal">New dialogue text for the list</param>
    /// <param name="whichChoiceIndex">Which choice to change by index</param>
    public virtual void DialogueExtra(bool modifyChoices, string newKey, string newTextVal, int whichChoiceIndex)
    {
        if (modifyChoices)
        {
            this.currentDialogueChoicesKey[whichChoiceIndex] = newKey;
            this.currentDialogueChoicesVal[whichChoiceIndex] = newTextVal;
        }
    }
    /// <summary>
    /// Helper method to help add on other things that we need to execute, this one is to add in playing specific character animations and play a singular SFX clip under the /sound/ command
    /// </summary>
    /// <param name="showActorIndex">Index for the character model</param>
    /// <param name="playActorAnim">Want to play an animation on the model?</param>
    /// <param name="ActorAnim">The name of the animation state in the character</param>
    /// <param name="useCustomSE">Want to use custom SFX for the /sound/ command?</param>
    /// <param name="customSEIndex">The index of the SFX audio clip to play for the /sound/</param>
    public virtual void DialogueExtra(/*string actorName,*/ int showActorIndex, bool playActorAnim, string ActorAnim, bool useCustomSE, int customSEIndex = 0)
    {
        if (playActorAnim == true && ActorAnim.Length > 0 && dialogActorGameObj.Length > 0)
        {
            //play anim
            if (dialogActorGameObj[showActorIndex].TryGetComponent<Animator>(out Animator characterAnimator))
            {
                characterAnimator.Play(ActorAnim);
            }
        }
        if (useCustomSE)
        {
            setCustomSE(customSEIndex);
        }

        //old code for actor name text
        //setNameText(actorName);
        //setNameTextVisbility(true);

        //if we want to show a specific actor model, disable all and reshow the new one
        if (dialogActorGameObj.Length > 0)
        {
            disableAllActors();
            dialogActorGameObj[showActorIndex].SetActive(true);
        }
    }
    /// <summary>
    /// Helper method to help add on other things that we need to execute, this one is to add in playing specific character animations, play a singular SFX clip under the /sound/ command, have an auto skipper
    /// </summary>
    /// <param name="showActorIndex">Index for the character model</param>
    /// <param name="playActorAnim">Want to play an animation on the model?</param>
    /// <param name="ActorAnim">The name of the animation state in the character</param>
    /// <param name="autoSkipAfterDelayTime">Delay time for the auto skip</param>
    /// <param name="useCustomSE">Want to use custom SFX for the /sound/ command?</param>
    /// <param name="customSEIndex">The index of the SFX audio clip to play for the /sound/</param>
    public virtual void DialogueExtra(/*string actorName,*/ int showActorIndex, bool playActorAnim, string ActorAnim, float autoSkipAfterDelayTime, bool useCustomSE, int customSEIndex = 0)
    {
        //invokes an autoclicker to simulate an auto skip
        Invoke("autoClick", autoSkipAfterDelayTime);
        if (playActorAnim == true && ActorAnim.Length > 0 && dialogActorGameObj.Length > 0)
        {
            //play anim
            if (dialogActorGameObj[showActorIndex].TryGetComponent<Animator>(out Animator characterAnimator))
            {
                characterAnimator.Play(ActorAnim);
            }
        }
        if (useCustomSE)
        {
            setCustomSE(customSEIndex);
        }

        //old code for actor name text
        //setNameText(actorName);
        //setNameTextVisbility(true);

        disableAllActors();
        dialogActorGameObj[showActorIndex].SetActive(true);
    }
    /// <summary>
    /// Helper method to help tidy everything up when called and do what is needed when a dialogue ends
    /// </summary>
    /// <param name="hideAllActors">Want to disable all character models?</param>
    public virtual void Finished(bool hideAllActors = false)
    {
        //[Debug]
        Debug.Log("Finished");

        //sends to a class called dialogState to keep track if player is in dialog, may not be unecessary but did it back then to not have to go to dialog manager to get it
        //and prefer to do it here to get it at the specific moment
        //dialogState.setIsInDialog(false);
        isPlaying = false;

        //old code
        //DialogManager.state = State.Active;
        //setNameText("");
        //setNameTextVisbility(false);
        
        //if we want to hide all actor models, disable it
        if (hideAllActors)
        {
            disableAllActors();
        }
    }
    public virtual void playDialogue()
    {
        Debug.LogError("this is coming from the master class, thats not supposed to happhen - JustDialogue");
    }
    public virtual void playDialogueChoice()
    {
        Debug.LogError("this is coming from the master class, thats not supposed to happhen - DialogueChoices");
    }

    //old code
    /*
    public virtual void Check_Correct()
    {
        Debug.LogError("this is coming from the master class, thats not supposed to happhen - Check_Correct");
    }*/
    /*public virtual void playDialogueChoice_Start()
    {
        Debug.Log("this is coming from the master class, thats not supposed to happhen, please override - DialogueChoices_Start");
    }*/
}
