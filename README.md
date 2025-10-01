# D'Dialogue System-Extended
![Unity Version](https://img.shields.io/badge/Unity-6000.0.30%30LTS%2B-blueviolet?logo=unity)
![Unity Pipeline Support (Built-In)](https://img.shields.io/badge/BiRP_✔️-darkgreen?logo=unity)
![Unity Pipeline Support (URP)](https://img.shields.io/badge/URP_✔️-blue?logo=unity)
![Unity Pipeline Support (HDRP)](https://img.shields.io/badge/HDRP_✔️-darkred?logo=unity)

This project is a modified version of DoublSB's D'Dialogue System ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master)), a simple dialogue system that uses C# Code to play out dialogues. These modifications help to make the system more usable for a much more general use and for my own use. This modification allows for re-triggerable dialogues and for use in non 2D visual novel games. 

The original system was specialized for single-shot visual novels and 2D-based dialogue, which felt restrictive for me in using it for other uses (even in a visual novel context). I used an older version of this modified system on an old game project, but it has since been scrapped. I am putting in my changes here in this repo for anyone to see or use for thier own purposes under MIT license. I have tested the code on Unity 6 (6000.0.30f) and in the URP pipeline, but it should be able to work on older unity versions or other pipelines just fine (other than maybe some modifications if required).

Do note that I haven't draft out the documentation yet, I will draft out one in the future. You can still refer to the old documentation (it is in the repo as well), as most of its commands and original functionalities are still there.

## The changes to the original system
- Dialogues are now re-triggerable, allowing developers to decide when and how dialogues can be replayed
- Removed Character and Emotion functionality, focusing on text, actor names, and dialogue choices
- Added character name text integration into the UI and Dialogue Manager
- Upgraded to TextMeshPro for better text rendering (can be changed back to using Text instead of TextMeshPro)
- Simplified SFX audio playing
- Enhanced the DialogueAsset prefab to align with these changes

## Installation
1. Download and import the system (via UnityPackage, zip, clone, fork, etc.)
2. Locate the `DialogAsset` in the **Prefab** folder under `DDSystem` and add it to your scene
3. Create a custom dialogue script for your character game object, or refer to the provided `Dialogue_DEBUG` script
4. Assign references to the appropriate fields in the inspector

## Creating a basic dialogue script
Here’s a simple example of creating a dialogue script:
```
Public DialogManager dialogManager;

void Start(){
  //creates the list needed to play the dialogues out on the screen
  var dialogTexts = new List<DialogData>();

  //this will add a new dialogue to the list, saying "Hello World" by the character, Bill
  dialogTexts.Add(new DialogData("Hello World", "Bill"));

  //this will play out the dialogue, and it will pop on the screen. Once it is finished, it will automatically turn off
  dialogManager.Show(dialogTexts);
}
```

## Commands
All commands start with /(Command Name)/. An example of using a command is
```
//the command we are using is /sound/, which will play a custom SFX clip at that point in the dialogue
dialogTexts.Add(new DialogData("And here we go, /sound/the haha sound!.", "test"
```
List of possible commands (You can also refer to the documentation as well, just no /emote/ command):
<br>
Screenshots from the original repo about the commands (as I don't have time to write through all this at the moment, will update this later on, so treat the screenshots as temporary for now)
<br>
![image](https://github.com/user-attachments/assets/d9e1d5c0-3036-4fd8-ac36-42f5dd5062c2)
![image](https://github.com/user-attachments/assets/4b553556-62e2-4694-800b-dc40b3a46a8c)
<br>
The original system's /sound/ command uses /sound(name)/, but here, we can simply call /sound/ and have it replace the SFXaudio audiosource in the DialogAsset prefab
with the correct audio clip by utilizing callbacks. To better demonstrate what I mean, here is an example to explain it
```
//assume we give this public varaible the haha sfx
public Audioclip HahaAudio;

//We will be using a helper method as a callback. So after we finish the dialogue and want to play the next it, since we set our callback to DialogueExtra, we would first execute the DialogueExtra function
//before playing the next dialogue. Lets say in the DialogueExtra method, we take haha sfx that we have and insert it into the SFXaudio Audiosource's audio clip field in the DialogAsset prefab in our scene.
//So we insert the audio clip to the correct audio source in DialogAsset, so when we play the next dialogue, it will refer to that audiosource in that prefab and play it in that moment in the dialogue
dialogTexts.Add(new DialogData("Blah Blah", "Jill", () => DialogueExtra(HahaAudio)));

dialogTexts.Add(new DialogData("/sound/haha!", "Bill"));
```

for more information, you can refer to the old documentation or the info in the original repo ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master)).

## To-Do
1. Optimize the code
2. Better Dialogue Actor/Character to Dialogue Manager system
3. Multiple SFX capability for the /sound/ command
4. More in-text dialogue commands
5. Draft documentation
6. Unity DOTS intergration

## Credits/Assets used
D'Dialogue System by DoublSB ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master)). Licensed under MIT license - See [THIRD PARTY LICENSES](THIRD_PARTY_LICENSES) for details. (The base code was originally from DoublSB's D'Dialogue System repo, but I have since made modifications to the code to make it more suitable for general use and for my own use, and to also fix some bugs that was inherent within the original code when I applied the new changes to it)
