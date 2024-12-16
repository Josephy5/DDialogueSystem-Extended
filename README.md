# D'Dialogue System (modified)
![Unity Version](https://img.shields.io/badge/Unity-6000.0.30%30LTS%2B-blueviolet?logo=unity)
![Unity Pipeline Support (Built-In)](https://img.shields.io/badge/BiRP_✔️-darkgreen?logo=unity)
![Unity Pipeline Support (URP)](https://img.shields.io/badge/URP_✔️-blue?logo=unity)
![Unity Pipeline Support (HDRP)](https://img.shields.io/badge/HDRP_✔️-darkred?logo=unity)

This project is a modified version of DoublSB's D'Dialogue System ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master)). I made these modifications to the dialogue system so that its usable for one of my old projects
(An Ace Attorney like game in 3D space, has been scrapped now due to overscoping) as the original system felt restritive due to its specialization towards it being a single shot visual novel and 2D based dialogue system. These modifications would make it more 
suitable for general use, like 3D games or any other games that require dialogue to be re-triggerable. I removed some of the unnecssary bits like the character and emotion stuff since I don't need that in my projects at that time (and still don't). I have also 
took the time to organize and clean it a bit so that its easier to read. The purpose of this repo is just to showcase some of my changes made to the system to the public so that others can take a look at it or use it for thier own purposes. 

The changes will make it a bit more dependant on the developer to code thier way through but the modified system still retains original system's simple implementation design. If you want to know what changes are been made, refer to the The changes to the original system.
I have tested the code on Unity 6 (6000.0.30f) and in the URP pipeline, but it should be able to work on older unity versions or other pipelines just fine (other than maybe some modifications if required).

Do note that I haven't draft out the documentation yet, I will draft out one in the future. You can still refer to the old documentation (it is in the repo as well), as most of its commands and original functionalities are still there. Also, there is still some
more things to be done or have planned to add in this system, as can be seen in the To-Do section.

## The changes to the original system
- Dialogue is no longer single shot and can be used more for general purposes
  - So rather than to make the dialogue single shot, All dialogue can be replayable (so if you added dialogue to a npc, you can now always retrigger the same dialogue as much as you like depending on how you code it)
    - So the dialogues will no longer be automatically untriggerable after it finishes playing, it will be more determined on how the developer codes thier dialogue scripts to call the dialogue system
- Removed Character and Emotion class and functionalities (just text, the actor's name, and callback)
- Added character's name UI text on the dialogue box UI and intergrated it with the dialogue manager to automatically output the character's name
- Modified the DialogueAsset prefab to be more reflective to the new changes
  - Sub-canvases for the dialogue text, character's name text, and dialogue choices elements
  - Replaced text with TextMeshPro (you can always modify it to change it back if you prefer to use text instead)
- Added tooltips and headers to dialogue manager's varaibles
- Modified the SFX audio playing to be more simple (once, I make the new dialog character class and system, this will be changed)

## Installation
1. Download and import the system (Download the UnityPackage from releases, zip the repo, clone the repo, fork it, etc)
2. Find the DialogAsset in the Prefab folder in DDSystem and move into the scene
3. Create a dialogue script for your character game object (if you don't have one, you can create a simple cube game object). 
4. [Optional] If you don't want to create or need to refer to one, you can use or refer to the provided Dialogue_DEBUG script.
5. Click and drag in all the references into the approraite empty input fields

## Creating a basic dialogue script
Here is a snippit of code that creates a basic dialogue
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
Screenshots from the original repo about the commands (as I don't have time to write through all this at the moment, will update this later on, so treat the screenshots as temporary for now)
![image](https://github.com/user-attachments/assets/d9e1d5c0-3036-4fd8-ac36-42f5dd5062c2)
![image](https://github.com/user-attachments/assets/4b553556-62e2-4694-800b-dc40b3a46a8c)
<br>
The original system's /sound/ command uses /sound(name)/, but here, we can simply call /sound/ and have it replace the SFXaudio audiosource in the DialogAsset prefab
with the correct audio clip by utilizing callbacks. To better demostrate what I mean, here is an example to explain it
```
//assume we give this public varaible the haha sfx
public Audioclip HahaAudio;

//We will be using a helper method as a callback. So after we finish the dialogue and want to play the next it, since we set our callback to DialogueExtra, we would first execute the DialogueExtra function
//before playing the next dialogue. Lets say in the DialogueExtra method, we take haha sfx that we have and insert it into the SFXaudio Audiosource's audio clip field in the DialogAsset prefab in our scene.
//So we insert the audio clip to the correct audio source in DialogAsset, so when we play the next dialogue, it will refer to that audiosource in that prefab and play it in that moment in the dialogue
dialogTexts.Add(new DialogData("Blah Blah", "Jill", () => DialogueExtra(HahaAudio)));

dialogTexts.Add(new DialogData("/sound/haha!", "Bill"));
```

for more infomation, you can refer to the old documentation or the info in the original repo ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master)).

## To-Do
1. Optimize the code
2. Better Dialogue Actor/Character to Dialogue Manager system
3. Multiple SFX capability for the /sound/ command
4. More in-text dialogue commands
5. Draft documentation
6. Unity DOTS intergration

## Credits/Assets used
The base code of the dialogue system is from DoublSB's D'Dialogue System ([Original GitHub Repository](https://github.com/DoublSB/UnityDialogAsset/tree/master))
