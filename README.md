# AliveCursor_ModUploader

## Introduction
I create this sample project to upload mods to [Steam Workshop](https://steamcommunity.com/profiles/76561199378980403/myworkshopfiles/?appid=1606490) for my indie Steam App [AliveCursor](https://store.steampowered.com/app/1606490/_/) using [AliveCursorSDK](https://github.com/Threeyes/AliveCursorSDK).
This project aimed for demonstrating how to using Plugins or custom Scripts to bring diffenent kinds of models alive. 

The items inside this project are for experimental purposes only, which will get updated frequently, may incompatible with old config data, so they may be imperfect for users. Think of this project as a tutorial, it won't cover every aspect but it will be a good start.

<p align="center">
    <img src="https://user-images.githubusercontent.com/13210990/195757514-014d8d7d-b0bf-438c-9e53-40300185e1a2.gif" alt="Item图片墙" width="600px" height="450px" />
    <br />
</p>

## Example (Simple)
* **Space Ship/Rocket Ship/Hover Car**: Tilt or rotate object base on cursor offset.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847247854"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/SpaceShip.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2947919750"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Rocket Ship.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2938058460"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Hover Car.gif" height="256" width="256" ></a>

* **2D Clock/HourGlass Clock**: Display system time in different ways.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847244998"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/2D Clock.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847245798"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/HourGlass Clock.gif" height="256" width="256" ></a>

* **IcoSphere AudioVisualizer**: Audio visualization.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847246006"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/IcoSphere AudioVisualizer.gif" height="256" width="256" ></a>

* **Billboard**: Using [Cloth](https://docs.unity3d.com/Manual/class-Cloth.html) Component for simulating fabrics, also provide customize pictures and text to user.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859024943"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Billboard.gif" height="256" width="256" ></a>

* **Golden Hoop**: Using [SplineMesh](https://github.com/methusalah/SplineMesh) plugin to create runtime Spline.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2858012046"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Golden Hoop.gif" height="256" width="256" ></a>

* **Hairy/ZhiYin Hair**: Using [Unity hair system](https://github.com/Unity-Technologies/com.unity.demoteam.hair) plugin to create realistic strand-based hair.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2889759157"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Hairy.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2956577286"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/ZhiYin_Hair.gif" height="256" width="256" ></a>


* **Physics Liquid**: Using [Zibra Liquids (Free version)](https://github.com/Shaidyk/com.zibra.liquids-free) plugin to create real-time liquid.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2857203254"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Physics Liquid.gif" height="256" width="256" ></a>

* **Snake**: Using [Animation Rigging](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.1/manual/index.html) plugin to create procedural joints.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847247577"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Snake.gif" height="256" width="256" ></a>

* **Master Chief**: Using other fun models (such as crosshair or knife) to replace cursor.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847246319"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Master Chief.gif" height="256" width="256" ></a>

* **Balloon**: Generate random objects.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2847245294"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Balloon.gif" height="256" width="256" ></a>



## Example (Complex)

* **Spider/Hand/Robotic Hand/Cat [Procedural]**: Create procedural spider-like creature, which can response to system audio. You can use it to make a role with any number of feet.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2868053102"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Spider.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2879016434"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Hand.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2914862770"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Robotic Hand.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2968253792"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Cat [Procedural].gif" height="256" width="256" ></a>

* **SpiderMan/Tears of the Kingdom Link**: Create a character that switch animation based on his/her states.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2974472367"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/SpiderMan.gif" height="256" width="256" ></a>
<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2984276367"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Tears of the Kingdom Link.gif" height="256" width="256" ></a>

* **Apple Pencil**: Draw doodles on Screen.

<a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2967916555"><img src="https://github.com/Threeyes/AliveCursor_ModUploader/wiki/images/readme/Apple Pencil.gif" height="256" width="256" ></a>


## Note
+ Always make sure that you have imported the latest simulator resources, see more info on [wiki](https://github.com/Threeyes/AliveCursorSDK/wiki/Setup-SDK).
+ **Don't** try to reupload any items using this project because them have been uploaded by me and their's id are settled in `WorkshopItemInfo.asset`!
+ If you want to use or modify some items in this project, please create a new item dir in your project using `Item Manager` Window, then copy all assets under `Assets/Items/(DesireItemName)` folder to your project's new item dir except `WorkshopItemInfo.asset`. 
+ I will keep uploading different kinds of models and creating general-purpose scripts out of them, feel free to contact me and tell me what else you wanna see on screen.
