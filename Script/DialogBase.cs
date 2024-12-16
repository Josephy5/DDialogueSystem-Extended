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

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System.Linq;

namespace Doublsb.Dialog
{
    #region Enum
    //enums for the state of the dialog system
    public enum State
    {
        Active,
        Wait,
        Deactivate
    }

    //Typically, I would have change them to be on camelcase but I left them be as I prefer them
    //to remain on lowercase

    //enums for the in text commands
    public enum Command
    {
        print = 0,
        color = 1,
        //part of the old code
        //emote = 2,
        size = 3,
        sound = 4,
        speed = 5,
        click = 6,
        close = 7,
        wait = 8
    }
    //enums for text colors for the color in-text command
    public enum TextColor
    {
        aqua,
        black,
        blue,
        brown,
        cyan,
        darkblue,
        fuchsia,
        green,
        grey,
        lightblue,
        lime,
        magenta,
        maroon,
        navy,
        olive,
        orange,
        purple,
        red,
        silver,
        teal,
        white,
        yellow
    }
    #endregion

    #region Old Code
    //part of the old code
    /*
    #region Emotion
    [Serializable]
    public class Emotion
    {
        //================================================
        //Public Variable
        //================================================
        private Dictionary<string, Sprite> _data;
        public Dictionary<string, Sprite> Data
        {
            get
            {
                if (_data == null) _init_emotionList();
                return _data;
            }
        }

        public string[] _emotion = new string[] { "Normal" };
        public Sprite[] _sprite;

        //================================================
        //Private Method
        //================================================
        private void _init_emotionList()
        {
            _data = new Dictionary<string, Sprite>();

            if (_emotion.Length != _sprite.Length)
                Debug.LogError("Emotion and Sprite have different lengths");

            for (int i = 0; i < _emotion.Length; i++)
                _data.Add(_emotion[i], _sprite[i]);
        }
    }
    #endregion
    */
    #endregion

    /// <summary>
    /// The class that converts string to Data. Contains List of DialogCommand and DialogFormat.
    /// </summary>
    public class DialogData
    {
        #region DialogData_Variables
        //the name of the character doing the dialog
        public string Character;
        public List<DialogCommand> Commands = new List<DialogCommand>();
        public DialogSelect SelectList = new DialogSelect();
        public DialogFormat Format = new DialogFormat();

        public string PrintText = string.Empty;

        public bool isSkippable = true;
        public UnityAction Callback = null;
        #endregion

        #region DialogData_Constructor
        public DialogData(string originalString, string character = "", UnityAction callback = null, bool isSkipable = true)
        {
            _convert(originalString);

            this.isSkippable = isSkipable;
            this.Callback = callback;
            this.Character = character;
        }
        #endregion

        #region DialogData_Private Methods
        /// <summary>
        /// Converts the original dialogue string into 2 parts, one which is the text that will be printed on screen 
        /// and one that hosts the commands and its included text within the printable text. 
        /// Any found commands will be sent to _convert_Syntex_To_Command() method
        /// </summary>
        /// <param name="originalString"> Original dialogue string that we want to input</param>
        private void _convert(string originalString)
        {
            string printText = string.Empty;

            for (int i = 0; i < originalString.Length; i++)
            {
                if (originalString[i] != '/') printText += originalString[i];

                else // If find '/'
                {
                    // Convert last printText to command
                    if (printText != string.Empty)
                    {
                        Commands.Add(new DialogCommand(Command.print, printText));
                        printText = string.Empty;
                    }

                    // Substring /CommandSyntex/
                    var nextSlashIndex = originalString.IndexOf('/', i + 1);
                    string commandSyntex = originalString.Substring(i + 1, nextSlashIndex - i - 1);

                    // Add converted command
                    var com = _convert_Syntex_To_Command(commandSyntex);
                    if (com != null) Commands.Add(com);

                    // Move i
                    i = nextSlashIndex;
                }
            }

            if (printText != string.Empty) Commands.Add(new DialogCommand(Command.print, printText));
        }
        /// <summary>
        /// Converts the given string into tangaible commands that we can execute
        /// </summary>
        /// <param name="text">The string that contains our commands</param>
        /// <returns></returns>
        private DialogCommand _convert_Syntex_To_Command(string text)
        {
            var spliter = text.Split(':');

            Command command;
            if (Enum.TryParse(spliter[0], out command))
            {
                if (spliter.Length >= 2) return new DialogCommand(command, spliter[1]);
                else return new DialogCommand(command);
            }
            else
                Debug.LogError("Cannot parse to commands");

            return null;
        }
        #endregion
    }

    /// <summary>
    /// The class for the RichText tagger of size and color.
    /// </summary>
    public class DialogFormat
    {
        #region DialogFormat_Variables
        public string DefaultSize = "60";
        private string _defaultColor = "black";

        private string _color;
        private string _size;
        #endregion

        #region DialogFormat_Constructor
        public DialogFormat(string defaultSize = "", string defaultColor = "")
        {
            _color = string.Empty;
            _size = string.Empty;

            if (defaultSize != string.Empty) DefaultSize = defaultSize;
            if (defaultColor != string.Empty) _defaultColor = defaultColor;
        }
        #endregion

        #region DialogFormat_Public Methods
        public string Color
        {
            set
            {
                if (isColorValid(value))
                {
                    _color = value;
                    if (_size == string.Empty) _size = DefaultSize;
                }
            }

            get => _color;
        }

        public string Size
        {
            set
            {
                if (isSizeValid(value))
                {
                    _size = value;
                    if (_color == string.Empty) _color = _defaultColor;
                }
            }

            get => _size;
        }

        public string OpenTagger
        {
            get
            {
                if (isValid) return $"<color={Color}><size={Size}>";
                else return string.Empty;
            }
        }

        public string CloseTagger
        {
            get
            {
                if (isValid) return "</size></color>";
                else return string.Empty;
            }
        }

        public void Resize(string command)
        {
            if (_size == string.Empty) Size = DefaultSize;

            switch (command)
            {
                case "up":
                    _size = (int.Parse(_size) + 10).ToString();
                    break;

                case "down":
                    _size = (int.Parse(_size) - 10).ToString();
                    break;

                case "init":
                    _size = DefaultSize;
                    break;

                default:
                    _size = command;
                    break;
            }
        }
        #endregion

        #region DialogFormat_Private Methods
        private bool isValid
        {
            get => _color != string.Empty && _size != string.Empty;
        }

        private bool isColorValid(string Color)
        {
            TextColor textColor;
            Regex hexColor = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");

            return Enum.TryParse(Color, out textColor) || hexColor.Match(Color).Success;
        }

        private bool isSizeValid(string Size)
        {
            float size;
            return float.TryParse(Size, out size);
        }
        #endregion
    }

    /// <summary>
    /// The class for the dialogue commands
    /// </summary>
    public class DialogCommand
    {
        #region DialogCommand_Variables
        public Command Command;
        public string Context;
        #endregion

        #region DialogCommand_Constructor
        public DialogCommand(Command command, string context = "")
        {
            Command = command;
            Context = context;
        }
        #endregion
    }

    /// <summary>
    /// The class for the dialogue choice selection
    /// </summary>
    public class DialogSelect
    {
        #region DialogSelect_Variables
        private List<DialogSelectItem> ItemList;
        #endregion

        #region DialogSelect_Constructor
        public DialogSelect()
        {
            ItemList = new List<DialogSelectItem>();
        }
        #endregion
        
        #region DialogSelect_Public Methods
        public int Count
        {
            get => ItemList.Count;
        }

        public DialogSelectItem GetByIndex(int index)
        {
            return ItemList[index];
        }

        public List<DialogSelectItem> Get_List()
        {
            return ItemList;
        }

        public string Get_Value(string Key)
        {
            return ItemList.Find((var) => var.isSameKey(Key)).Value;
        }

        public void Clear()
        {
            ItemList.Clear();
        }

        public void Add(string Key, string Value)
        {
            ItemList.Add(new DialogSelectItem(Key, Value));
        }

        public void Remove(string Key)
        {
            var item = ItemList.Find((var) => var.isSameKey(Key));

            if (item != null) ItemList.Remove(item);
        }
        #endregion
    }

    /// <summary>
    /// The class for the dialogue choice item
    /// </summary>
    public class DialogSelectItem
    {
        #region DialogSelectItem_Variables
        public string Key;
        public string Value;
        #endregion

        #region DialogSelectItem_Constructor
        public DialogSelectItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        #endregion

        #region DialogSelectItem_Public Methods
        public bool isSameKey(string key)
        {
            return Key == key;
        }
        #endregion
    }
}