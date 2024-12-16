/*
The MIT License

Copyright (c) 2020 DoublSB
https://github.com/DoublSB/UnityDialogAsset

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System;
using TMPro;

namespace Doublsb.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        #region DialogManager_Variables
        [Header("Game Objects")]
        [Tooltip("The Gameobject Parent holding the dialogue text")]
        public GameObject Printer;
        
        // part of the old code
        // public GameObject Characters;
        
        //main ui canvas
        //public GameObject canvas;

        [Header("UI Objects")]
        [Tooltip("The UI Gameobject holding the dialogue text, can be Text or TextMeshPro")]
        public /*Text*/TextMeshProUGUI PrinterText;
        [Tooltip("The UI Gameobject holding the name of the character speaking, default is TextMeshPro but can be modified to use Text instead")]
        public TextMeshProUGUI ActorNameText;

        [Header("Audio Objects")]
        [Tooltip("The Audiosource Gameobject that hosts our SFX for dialogue typing")]
        public AudioSource DialogueTypingSFX;
        [Tooltip("The Audiosource Gameobject that hosts our SFX for the in-text /sound/ command")]
        public AudioSource SFXAudio;

        [Header("Preference")]
        [Tooltip("The delay for the dialogue typing animation. Higher = will type more slowly, Lower = will type much faster")]
        [Range(0f, 100f)]
        public float Delay = 0.1f;

        [Header("Selector")]
        [Tooltip("The game object parent containing the dialogue choices")]
        public GameObject DialogueChoice;
        [Tooltip("The game object containing template dialogue choice")]
        public GameObject DialogueChoiceItem;
        [Tooltip("The game object containing the text within the template dialogue choice")]
        public /*Text*/TextMeshProUGUI DialogueChoiceItemText;

        //mostly for debuging or used in situations where we need to get the state of the dialogue
        [SerializeField]
        private State state;

        //mostly for debuging or used in situations where we need to get the exact dialogue text, for logging purposes, dialogue choices,
        //or other things like the log function commonly found in visual novels
        //[HideInInspector]
        public string Result { get; set; }
        
        //mostly for debuging or used in situations where we need to get the correct state of the dialogue being finished
        //[HideInInspector]
        public bool hasFinished = false;

        //part of the old code
        //private Character _current_Character;
        
        //the current dialog data object
        private DialogData _current_Data;

        //floats for our delays in the text printing
        private float _currentDelay;
        private float _lastDelay;
        
        //coroutines for the printing and text related stuff
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;
        #endregion

        /// <summary>
        /// the method to initialize everything needed to print our dialogue
        /// </summary>
        private void _initialize()
        {
            //Profiler.BeginSample("DialogManager (_initialize)");
            _currentDelay = Delay;
            _lastDelay = 0.1f;
            PrinterText.text = string.Empty;

            //commented out for now, should be unnecessary
            //canvas.SetActive(true);

            Printer.SetActive(true);
            if (!ActorNameText.gameObject.activeInHierarchy) ActorNameText.gameObject.SetActive(true);

            //part of the old code
            //Characters.SetActive(_current_Character != null);
            //foreach (Transform item in Characters.transform) item.gameObject.SetActive(false);
            //if(_current_Character != null) _current_Character.gameObject.SetActive(true);
            //Profiler.EndSample();
        }

        #region Essential UI Methods {Show, Hide, & Click_Window}
        /// <summary>
        /// Used to call the coroutine to show the dialogue text
        /// </summary>
        /// <param name="Data">The dialogue's data</param>
        public void Show(DialogData Data)
        {
            //Profiler.BeginSample("DialogManager (Show)");
            _current_Data = Data;
            
            //part of the old code
            //_find_character(Data.Character);

            //if(_current_Character != null)
                //_emote("Normal");

            _textingRoutine = StartCoroutine(Activate());
            //Profiler.EndSample();
        }
        /// <summary>
        /// Used to call the coroutine to show the dialogue choices
        /// </summary>
        /// <param name="Data">The data containing our dialogue's choices</param>
        public void Show(List<DialogData> Data)
        {
            //[Debug]
            //Debug.Log("DialogManager - Show");
            //Profiler.BeginSample("DialogManager (ShowChoices)");
            StartCoroutine(Activate_List(Data));
            //Profiler.EndSample();
        }
        /// <summary>
        /// Used to skip the dialogue to the next one or the end if the dialogue box button is pressed or is
        /// called by somewhere else
        /// </summary>
        public void Click_Window()
        {
            //Profiler.BeginSample("DialogManager (ClickWindow)");
            switch (state)
            {
                case State.Active:
                    //[Debug]
                    //Debug.Log("DialogManager - ClickWin.Active");
                    StartCoroutine(_skip()); break;

                case State.Wait:
                    //[Debug]
                    //Debug.Log("DialogManager - ClickWin.Wait");
                    if (_current_Data.SelectList.Count <= 0) Hide(); break;
            }
            //Profiler.EndSample();
        }
        /// <summary>
        /// Stop all tasks related to the displaying of the dialogue text and hide it
        /// </summary>
        public void Hide()
        {
            //Profiler.BeginSample("DialogManager (Hide)");
            if (_textingRoutine != null)
                StopCoroutine(_textingRoutine);

            if(_printingRoutine != null)
                StopCoroutine(_printingRoutine);

            //part of the old code
            //Characters.SetActive(false);

            ActorNameText.gameObject.SetActive(false);
            Printer.SetActive(false);
            DialogueChoice.SetActive(false);

            //commented out for now, should be unnecessary
            //canvas.SetActive(false);

            state = State.Deactivate;

            if (_current_Data.Callback != null)
            {
                //[Debug]
                //Debug.LogWarning("DialogManager - CALLBACK +"+" "+_current_Data.Callback.Method.Name);

                _current_Data.Callback.Invoke();
                //_current_Data.Callback = null;
            }
            //Profiler.EndSample();
        }
        #endregion

        #region SFX Methods {Play_DialogueTypingSFX & Play_SFXAudio}
        /// <summary>
        /// Plays the SFX whenever the dialogue text is being typed out
        /// </summary>
        public void Play_DialogueTypingSFX()
        {
            //part of the old code
            /*
            if (_current_Character != null)
            {
                SEAudio.clip = _current_Character.ChatSE[UnityEngine.Random.Range(0, _current_Character.ChatSE.Length)];
                SEAudio.Play();
            }
            */
            DialogueTypingSFX.Play();
        }
        /// <summary>
        /// Plays the SFX whenever the /sound/ command is being called in the dialogue
        /// </summary>
        public void Play_SFXAudio(/*string SEname*/)
        {
            //part of the old code
            /*if (_current_Character != null)
            {
                var FindSE = Array.Find(_current_Character.CallSE, (SE) => SE.name == SEname);

                CallAudio.clip = FindSE;
                CallSEAudio.Play();
            }*/
            SFXAudio.Play();
        }

        #endregion

        #region Speed Methods {Set_Speed}
        /// <summary>
        /// sets the text typing speed based on given command and values from the dialogue
        /// </summary>
        /// <param name="speed">the inputted speed value</param>
        public void Set_Speed(string speed)
        {
            //Profiler.BeginSample("DialogManager (Set_Speed)");
            switch (speed)
            {
                //if its /speed:up/, speeding up the typing speed
                case "up":
                    _currentDelay -= 0.25f;
                    if (_currentDelay <= 0) _currentDelay = 0.001f;
                    break;
                //if its /speed:down/, slowing the typing speed down
                case "down":
                    _currentDelay += 0.25f;
                    break;
                //if its /speed:init/, aka set back to default typing speed
                case "init":
                    _currentDelay = Delay;
                    break;
                //if its /speed:(float)/, set the delay to input number
                default:
                    _currentDelay = float.Parse(speed);
                    break;
            }

            _lastDelay = _currentDelay;
            //Profiler.EndSample();
        }

        #endregion

        #region Dialogue Choice Methods {Select, _init_choice, _clear_choice, _add_ChoiceItem, Activate_List}
        /// <summary>
        /// Called whenever the dialogue choice gets selected by the player
        /// </summary>
        /// <param name="index">the selected dialogue choice</param>
        public void Select(int index)
        {
            //Profiler.BeginSample("DialogManager (Select)");
            //[Debug]
            //Debug.Log("DialogManager - Sel |"+ _current_Data.SelectList.GetByIndex(index).Key);

            Result = _current_Data.SelectList.GetByIndex(index).Key;
            Hide();
            //Profiler.EndSample();
        }
        /// <summary>
        /// initializing the dialogue choices
        /// </summary>
        private void _init_choice()
        {
            //Profiler.BeginSample("DialogManager (_init_choice)");
            //[Debug]
            //Debug.Log("DialogManager - SelectorINital");
            _clear_choice();

            //[Debug]
            //Debug.Log(_current_Data.SelectList.Count);
            if (_current_Data.SelectList.Count > 0)
            {
                DialogueChoice.SetActive(true);

                for (int i = 0; i < _current_Data.SelectList.Count; i++)
                {
                    _add_ChoiceItem(i);
                }
            }
                
            else DialogueChoice.SetActive(false);
            //Profiler.EndSample();
        }
        /// <summary>
        /// clears all out active choices
        /// </summary>
        private void _clear_choice()
        {
            //Profiler.BeginSample("DialogManager (_clear_choice)");
            for (int i = 1; i < DialogueChoice.transform.childCount; i++)
            {
                Destroy(DialogueChoice.transform.GetChild(i).gameObject);
            }
            //Profiler.EndSample();
        }
        /// <summary>
        /// adds in a new dialogue choice item to the list of dialogue choices
        /// </summary>
        /// <param name="index">the index to be put into</param>
        private void _add_ChoiceItem(int index)
        {
            //Profiler.BeginSample("DialogManager (_add_ChoiceItem)");
            //[Debug]
            //Debug.Log("DialogManager - SelAdd");
            DialogueChoiceItemText.text = _current_Data.SelectList.GetByIndex(index).Value;

            var NewItem = Instantiate(DialogueChoiceItem, DialogueChoice.transform);
            NewItem.GetComponent<Button>().onClick.AddListener(() => Select(index));
            NewItem.SetActive(true);
            //Profiler.EndSample();
        }
        /// <summary>
        /// activates the list of dialogue choices
        /// </summary>
        /// <param name="DataList"></param>
        private IEnumerator Activate_List(List<DialogData> DataList)
        {
            //Profiler.BeginSample("DialogManager (Activate_List)");
            //[Debug]
            //Debug.Log("DialogManager - ActivateList");
            state = State.Active;

            foreach (var Data in DataList)
            {
                Show(Data);
                _init_choice();

                while (state != State.Deactivate) { yield return null; }
            }
            //Profiler.EndSample();
        }
        #endregion

        #region General Methods {Activate, _waitInput, _print}
        /// <summary>
        /// Called when we want to show the dialogue choices, executes the commands in our dialogue's data
        /// </summary>
        private IEnumerator Activate()
        {
            //Profiler.BeginSample("DialogManager (Activate)");
            //[Debug]
            //Debug.Log("DialogManager - Activate");
            _initialize();

            state = State.Active;

            foreach (var item in _current_Data.Commands)
            {
                switch (item.Command)
                {
                    case Command.print:
                        yield return _printingRoutine = StartCoroutine(_print(item.Context));
                        break;

                    case Command.color:
                        _current_Data.Format.Color = item.Context;
                        break;
                    
                    //part of the old code
                    //case Command.emote:
                        //_emote(item.Context);
                        //break;

                    case Command.size:
                        _current_Data.Format.Resize(item.Context);
                        break;

                    case Command.sound:
                        Play_SFXAudio(/*item.Context*/);
                        break;

                    case Command.speed:
                        Set_Speed(item.Context);
                        break;

                    case Command.click:
                        yield return _waitInput();
                        break;

                    case Command.close:
                        Hide();
                        yield break;

                    case Command.wait:
                        yield return new WaitForSeconds(float.Parse(item.Context));
                        break;
                }
            }

            state = State.Wait;
            //Profiler.EndSample();
        }
        /// <summary>
        /// forces the dialogue to wait for a moment, usually called when there is a /wait/ command
        /// </summary>
        private IEnumerator _waitInput()
        {
            //Profiler.BeginSample("DialogManager (_waitInput)");
            while (!Input.GetMouseButtonDown(0)) yield return null;
            _currentDelay = _lastDelay;
            //Profiler.EndSample();
        }
        /// <summary>
        /// prints out our dialogue on the screen, usually called when there is a trigger for dialogue
        /// </summary>
        /// <param name="Text">the input dialogue text</param>
        private IEnumerator _print(string Text)
        {
            //Profiler.BeginSample("DialogManager (_print)");
            hasFinished = false;

            //if it is not active, set it active, otherwise ignore. But ultimately, set its text to the correct name
            if (!ActorNameText.gameObject.activeInHierarchy) ActorNameText.gameObject.SetActive(true);
            ActorNameText.text = _current_Data.Character;

            //prepare for prinitng by adding in the opening tags/commands
            _current_Data.PrintText += _current_Data.Format.OpenTagger;

            //print out the dialogue text, no fancy animation, just print it out with delay
            for (int i = 0; i < Text.Length; i++)
            {
                _current_Data.PrintText += Text[i];
                PrinterText.text = _current_Data.PrintText + _current_Data.Format.CloseTagger;

                if (Text[i] != ' ') Play_DialogueTypingSFX();
                if (_currentDelay != 0) yield return new WaitForSeconds(_currentDelay);
            }

            //add in the closing tags/commands to finish it off
            _current_Data.PrintText += _current_Data.Format.CloseTagger;

            hasFinished = true;
            //Profiler.EndSample();
        }
        #endregion

        #region Skipping Text {_skip}
        /// <summary>
        /// the method to handle matters where we want to skip ahead in the dialogue, ending it faster
        /// </summary>
        private IEnumerator _skip()
        {
            //Profiler.BeginSample("DialogManager (_skip)");
            if (_current_Data.isSkippable)
            {
                _currentDelay = 0;
                while (state != State.Wait) yield return null;
                _currentDelay = Delay;
            }
            //Profiler.EndSample();
        }
        #endregion

        #region Old Code
        //part of the old code
        /*
        private void _find_character(string name)
        {
            if (name != string.Empty)
            {
                Transform Child = Characters.transform.Find(name);
                if (Child != null) _current_Character = Child.GetComponent<Character>();
            }
        }
        */
        #endregion

        #region Old Code #2
        //part of the old code
        /*
        public void _emote(string Text)
        {
            _current_Character.GetComponent<Image>().sprite = _current_Character.Emotion.Data[Text];
        }
        */
        #endregion
    }
}