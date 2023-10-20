# XREyeTrackingAndClickSelection
XR eye tracking and click selection Unity project. Selection of objects done through eye tracking and click (in wearOS app).

This is a project developed for Oculus, to be used with an accompanying [wearOS app](https://github.com/SRSAS/ClickSelectionWearApp).

Objects are displayed in line, to select an object the user must first hover the object by looking at it. Then, while hovering the object, they must click on the screen of the accompanying wearOS app. Once they lift their finger from the screen, the object is selected.

The hovering is done with the Meta Quest Pro's eyetracking. The selecting is done by sending the app's user input information through a UDP socket.

This is meant as subtle and medium cost method of selection for XR.

## Authorship
Project authored by **Sebastião Andrade e Sousa**

[LinkedIn](https://www.linkedin.com/in/sebasti%C3%A3o-andrade-e-sousa-700827270/) -- [GitHub](https://github.com/SRSAS)


Developed during an internship researching subtle interactions with mobile XR for [HCI Lab @ IST](https://web.tecnico.ulisboa.pt/augusto.esteves/)

Project supervised by Professor [Augusto Esteves](http://web.tecnico.ulisboa.pt/augusto.esteves/EstevesCV-September2023.pdf)

All rights belong to the HCI Lab.

## Project Structure
### SDKs
This project uses Meta's [Movement SDK for Unity](https://developer.oculus.com/documentation/unity/move-overview/).
### Scripts
|Script|Description|
|----|-----------|
|**EyeHoverer**|Every _FixedUpdate()_ casts a ray in the direction that the object is facing. If it hits another object, displays that object as a public field.|
|**Selectable**|Holding this script is what makes an object selectable. Displays the object's selection state. Listens to the **SelectionManager** for user clicks. If this object is hovered during a user click, it is selected.|
|**SelectionManager**|This script controls the selection parameters (time and colors). It gets the infromation from the **SelectConfirmer**, and holds a _UnityEvent_ that tells other objects when the user has clicked. It also resloves conflicts if the eyes are hovering 2 different objects. It also sends information about the user's selection to the **TestManager**.|
|**SelectConfirmer**| C# native socket and threading used to receive user click. Only once the user lifts their finger does it confirm a selection, and holds it for 2 frames.|
|**TestManager**|Chooses a random **Selectable** for the user to select, then shuffles the positions of the objects and starts counting the time the user takes to select it. Also, listens to **SelectionManager** _UnityEvents_ to gather information about the user's selection process, and prints a report with that information to the console.|
### Prefabs
|Name|Scripts|Description|
|----|-------|-----------|
|**Eye**|_OVREyeGaze_, _EyeHoverer_|Representation of the user's eye. Must be placed on the **OVRCameraRig** > **TrackingSpace**, and there should be two of them (left and right). In the **Inspector Panel**, on the **OVEyeGaze** script, the eye field should be set to the corresponding eye side, Confidence Threshold can be 0, Apply Position and Apply Rotation should be ticked, Reference Frame can be an empty GameObject child of **OVRCameraRig** > **TrackingSpace** with position (0, 0, 1), and Tracking Mode should be **_World Space_**.|
|**Selector**| _SelectionManager_, _SelectConfirmer_| This object manages the whole selection process. For the scripts to function properly, on the **Inspector Panel**, in the Dominant Eye, and Non Dominant Eye fields you should place the eye objects that are tracking the user's eyes.|
|**Tester**|_TestManager_|Object for gathering information on user selection process. Add this to the scene to add a target for the user to select, and for shuffling of the objects.|
### SampleScene
- OVRCameraRig:
    - This is the camera for the Meta headset. It is a prefab taken directly from the Meta movement SDK. For more information, please refer to Meta's [documentation](https://developer.oculus.com/documentation/unity/unity-tutorial-hello-vr/).
    - Eyes: The objects tracking the user's eyes. From this project's prefab **Eye**.
- Selector.
## Cloning and Setup

1.  Clone the repository;
2.  Open the project on Unity Hub[^1];
3.  On the Unity Editor go to **File** > **Build Settings...** and click on **Android**;
4.  Click on **Switch Platform** (lower right corner);
5.  Then on that same window, in the lower left corner click on **Player Settings...**;
6.  On the column on the left select **Oculus**;
7.  On that page, check each tab's check list for outstanding issues and press **Fix All** if there are any;
8.  On the Project panel, go to Assets > Scenes and place the SampleScene onto the Hierarchy panel;
9.  Finally, delete the unnamed scene that was in the Hierarchy by default.

## Deploying on the Meta Quest Pro
If you encounter any problem deploying on the Meta Quest Pro, please follow the [official Meta documentation](https://developer.oculus.com/documentation/unity/unity-tutorial-hello-vr/).

1.  Connect the **Meta Quest Pro** via USB to your computer [^2];
2.  on the Unity Editor go to **File** > **Build Settings...**;
3.  Focus on the **Run Device** list and select the **Meta Quest Pro**;
4.  Finally, click on **Build and Run** to run the scene on your headset.




## Pairing with the App
On the wearOS app simply input the headset's _IPV4_ and click on **Connect**.
You can start sending user input.

**_IMPORTANT: The device running the wearOS app must be connected to the same network as the headset_**


## Adding targets
### Adding targets
To add a target, simply add whatever object you want to the scene, place it where you want it, and add the **Selectable** script to it.

## Getting test data
To get data from the user attempts to select objects, on the **Project Panel**, go to **Assets** > **Prefab**, and add the **Tester** prefab to the scene.

A random object will be picked and the text will tell the user (by the object's name) to select it. The objects' positions will then be shuffled, and the time that the user takes to select the object will be counted, as well as other data. A report will be made with that data.

This report is only printed to the console. To alter the report, or to send it to any other output stream, changes to the **TestManager** script must be made.

[^1]:To open the project, open Unity Hub, click on **Open**, then select the repository directory
[^2]:For troubleshooting connecting the Meta Quest Pro to your computer, see the [official Meta documentation](https://developer.oculus.com/documentation/unity/unity-env-device-setup/)
