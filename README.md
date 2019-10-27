# NESTool
Tool written in C# and WPF to manage asset for a NES game by Felipe Reinaud. Current implementation only supports NROM (Mapper 0). Resources files are in TOML format, [https://github.com/toml-lang/toml](https://github.com/toml-lang/toml). 

This tool is far from comlete. Current version is 1.0.0.0

### Table of Content  
[1. Overview](#Overview)   
[1.1. Menu](#Menu)   
[1.2. Toolbar](#Toolbar)   
[2. Getting started](#Gettingstarted)    
[2.1. Tile Sets](#TileSets)    
[2.2. Banks](#Banks)    
[2.3. Characters](#Characters)    
[2.4. Maps](#Maps)    
[3. Building the project](#Buildingtheproject)    
[4. Future features](#Futurefeatures)     
[5. Known bugs](#Knownbugs)     

<a name="Overview"/>

## 1. Overview

![](/Images/nestool.png)

<a name="Menu"/>

### 1.1 Menu

![](/Images/menu.png)

#### File
![](/Images/file_menu.png)

From File is possible to create a new project or a new element like a [Tile Sets](#TileSets) or a [Character](#Characters).

* A new project will have the extension **.proj** which is internaly a [TOML](https://github.com/toml-lang/toml) format and the name is given by the user. It will also create the folders **Banks**, **Characters**, **Maps** and **TileSets**. 

* You can open an existing project, where the **.proj** file exist.

* Close the current project.

* Import any image from these formats: *.png, .bmp, .gif, .jpg, .jpeg, .jpe, .jfif, .tif, .tiff, .tga*. The image will reduce the colors with a Palette Quantizer algorithm to match the number of colors the NES can reproduce. More about this topic in the [Getting started](#Gettingstarted).

* Recent project will contain all previous opened projects.

#### Edit
![](/Images/edit_menu.png)

Undo/Redo, Copy, Paste, Duplicate and Delete only affects the project's elements like a [Character](#Characters) element although the fucntionality is not complete yet so it is better not to use them.

#### Project
![](/Images/project_menu.png)

* Project Properties is where is possible to reconfigure the project settings after the project is created. There are actually more options here change than when the project is created. For more information read the [Getting started](#Gettingstarted) section.

* Build Project will create and export all maps, characters and pattern tables in the selected output folder. More on that in [Building the project](#Buildingtheproject) section.

#### Help
![](/Images/help_menu.png)

View Help redirects to this page in github.com.

<a name="Toolbar"/>

### 1.2 Toolbar

![](/Images/toolbar.png)

Toolbar Has the option to create a new project (explained in [Getting started](#Gettingstarted) section) or open an existing project, undo or redo (not recommended to use) and build project. The last one is explained in the [Building the project](#Buildingtheproject) section.

<a name="Gettingstarted"/>

## 2. Getting started

Once NESTool is opened for the first time, it will create in the root of the executable, the file **mappers.toml** and **config.toml** . Only the last one if some of the configuration of the tool changes like the window size. **Mappers.toml** contains the information of all mappers configuration and the idea is that if an especific mapper is selected, it will reconfigure what the tool is capable of doing and exporting. So far, only mapper 0 or NROM is available.

![](/Images/newproject.png)

To craete a new project click File > New > New Project (Ctrl + Shift + N) or ![](/Images/newproject_toolbar.png) in the toolbar. From there is possible to name the project and a location. Selecting the mapper will change the options available for PRG size and CHR size. Right now is only possible to select no mapper (NROM). After that press the button *Create project* and this will create in the specified location the file *name of the project*.proj in [TOML](https://github.com/toml-lang/toml) format, and the folders *Banks*, *Characters*, *Maps*, *TileSets*.

Once the project is created, NESTool will always open the last opened project. 

At any time is possible to change the project configurations from the menu Project > Project Properties...

![](/Images/projectproperties.png)

From Project properties is possible to change the Mapper type, PRG size, CHR size, mirroring, sprite size between 8x8 or 8x16, etc.

![](/Images/newelement.png)

Creating elements is possible from the menu File > New > New Element (Ctrl + N) or right click on any root folder to open the context menu and select *Create New Element*.

There are only four type of elements to create, [Tile Sets](#TileSets), where you can import a new image and change its pixels, [Banks](#Banks), where you can create banks of any size or NES pattern tables using the [Tile Sets](#TileSets) as input, [Characters](#Characters), is an element created to by [Banks](#Banks) as its input and there it is possible to create meta sprites and animations, and finally, [Maps](#Maps), also uses [Banks](#Banks) as its input. It is really important to understand that all of the links between elements are just references to each other. For example: the bank pattern table could use different tile sets but if one of those tile sets changes something it will also change the tile inside the pattern table and immediately in the character or map that is using that specific bank.

![](/Images/tree.png)

Is possible to create folders inside each root folder and move elements of the same type to any sub folder by just dragging the element. Each element including folders has a context menu with the right click.

To start creating assets for the NES, the very first thing to create is the Tile Set explained in the section below.

<a name="TileSets"/>

### 2.1 Tile Sets

Tile Sets are the basic element to start constructing NES assets but they are not exported directly, they are only used to build [Banks](#Banks), those are exported. This is explained more in depth in the [Building the project](#Buildingtheproject) section. Tile Sets are images from these formats: *.png, .bmp, .gif, .jpg, .jpeg, .jpe, .jfif, .tif, .tiff, .tga*. The image will reduce the colors with a Palette Quantizer algorithm to match the number of colors the NES can reproduce. Once is imported, it is possible to use each 8x8 or 8x16 pixels of the image as sprites to construct a character meta sprite including its animation or a map.

![](/Images/mario.png)

Let´s pick up this image from Super Mario World. Ideally should be an image with the right size to be use because this tool does not support scaling.

![](/Images/importimage.png)

There are two ways to import a new image to a *Tile Set* element, first one is to use File > Import > Image... (Ctrl + I). This will create a *Tile Set* element with the name of the image. The second way to import an image is to create a *Tile Set* element, explained the the [Getting started](#Gettingstarted) section and then click over the new element and then click the *tree dots* button on the top part of the element window to browse your computer for an image.

All images after being imported will create if it doesn't exist already a folder name **Images** in the project root directory and I will copy the new imported image there with the extension *.bmp*.

![](/Images/importedimage.png)

After the image is imported it is possible to zoom in/out with the mouse's scroll wheel and it will appear a new toobar button to hide or show the 8x8/8x16 grid.

![](/Images/changingcolors.png)

Clicking any 8x8 cell in the image will show it in the zoon in quadrant at the left where is possible to change the colors. It is important to press the button save to apply the changes.

> ⚠️**WARNING**: Each 8x8/8x16 cell must be 4 colors maximum. Transparent color counts as one color leaving 3 colors left. This tool does not check if a cell has more than 4 colors.

<a name="Banks"/>

### 2.2 Banks

Banks are tiles grouped together. Is possible to have banks in different sizes (1kb, 2kb, 4kb). **Pattern Tables** are 4kb banks used as the main source for NES graphics. Pattern Tables can be either background or sprites. Banks are constructed from [Tile Sets](#TileSets). This will form a link inside the banks to each Tile Set used. If a Tile Set changes its tiles, it is renamed or removed, it will automatically update the bank.

<a name="Characters"/>

### 2.3 Characters

Characters are created by using banks. The tiles from this bank will be stored as a link to them, if one of those banks changes, it is renamed or deleted it will automatically updates the character.

If the bank is set to pattern table for background tiles, it won't appear as an option for character creation.

From here it is possible to create animations. Clicking the plus button will create a new frame of the animation. When there is more than one frame, the play button, stop, pause, previous frame and next frame are available.

Here is also possible to set the animation speed. This value is in seconds per frame and this is also used when building the project to be used in the assembly. More details explained in [Building the project](#Buildingtheproject) section.

<a name="Maps"/>

### 2.4 Maps

Maps are created by using banks. If the bank is set to pattern table for sprite tiles, it won't appear as an option for map creation.

<a name="Buildingtheproject"/>

## 3. Building the project

![](/Images/build.png)

Building the project will create a bunch of files in the output directory. 

There can only be two [Pattern Tables](#Banks), one for sprites and one for backgrounds.

<a name="Futurefeatures"/>

## 4. Future features

+ Support 8x16 sprites
+ Support multiple banks with different sizes.

<a name="Knownbugs"/>

## 5. Known bugs

Real Github's Issues are needed here. For now the list is just here with no real ticket created.

+ Reimporting the same image to a TileSet element will make the tool crash.
+ Undo, Redo is not working inside each element.
