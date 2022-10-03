# NESTool
Tool written in C# and WPF using the MVVM design pattern to manage asset for a NES game by Felipe Reinaud. Resources files are in TOML format, [https://github.com/toml-lang/toml](https://github.com/toml-lang/toml). 

This tool is far from comlete. Current version is 1.0.0

### Table of Content  
[1. Overview](#Overview)   
[1.1. Menu](#Menu)   
[1.2. Toolbar](#Toolbar)   
[2. Getting started](#Gettingstarted)    
[2.1 Create project elements](#CreateElements)    
[3. Managing resources](#ManagingResources)    
[3.1. Tile Sets](#TileSets)    
[3.2. Banks](#Banks)    
[3.3. Palettes](#Palettes)    
[3.4. Characters](#Characters)    
[3.5. Entities](#Entities)    
[3.6. Maps](#Maps)  
[3.7. Worlds](#Worlds)  
[4. Building the project](#Buildingtheproject)    
[5. Future features](#Futurefeatures)     
[6. Known bugs](#Knownbugs)     

<a name="Overview"/>

## 1. Overview

![](/Images/nestool.png)

<a name="Menu"/>

### 1.1 Menu

![](/Images/menu.png)

#### File
![](/Images/file_menu.png)

From File is possible to create a new project or a new element like a [Tile Sets](#TileSets) or a [Character](#Characters).

* A new project will have the extension **.proj** which is internaly a [TOML](https://github.com/toml-lang/toml) format and the name is given by the user. It will also create the folders **Banks**, **Palettes**, **Characters**, **Maps** and **TileSets**. 

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

![](/Images/projectproperties2.png)

From Project properties is possible to change the Mapper type, PRG size, CHR size, mirroring. For now Battery is listed here as a possible future feature. The whole idea is maybe the tool can generate a header file based on this information.

<a name="CreateElements"/>

### 2.1 Create project elements

![](/Images/newelement.png)

Creating elements is possible from the menu File > New > New Element (Ctrl + N) or right click on any root folder to open the context menu and select *Create New Element*.

There are only five type of elements to create, [Tile Sets](#TileSets), where you can import a new image and change its pixels, [Banks](#Banks), where you can create banks of any size or NES pattern tables using the [Tile Sets](#TileSets) as input, [Palettes](#Palettes), are the actual colors for any sprite used by the caracteres, [Characters](#Characters), is an element created by [Banks](#Banks) as its input and there, it is possible to create meta sprites and animations, and finally, [Maps](#Maps), also uses [Banks](#Banks) as its input. It is really important to understand that all of the links between elements are just references to each other. For example: the bank pattern table could use different tile sets but if one of those tile sets changes something it will also change the tile inside the pattern table and immediately in the character or map that is using that specific bank.

![](/Images/tree.png)

Is possible to create folders inside each root folder and move elements of the same type to any sub folder by just dragging the element. Each element including folders has a context menu with the right click.

To start creating assets for the NES, the very first thing to create is the Tile Set explained in the section below.

<a name="ManagingResources"/>

## 3. Managing resources

<a name="TileSets"/>

### 3.1 Tile Sets

Tile Sets are the basic element to start constructing NES assets but they are not exported directly, they are only used to build [Banks](#Banks), those are exported. This is explained more in depth in the [Building the project](#Buildingtheproject) section. Tile Sets are images from these formats: *.png, .bmp, .gif, .jpg, .jpeg, .jpe, .jfif, .tif, .tiff, .tga*. The image will reduce the colors with a Palette Quantizer algorithm to match the number of colors the NES can reproduce. Once is imported, it is possible to use each 8x8 or 8x16 pixels of the image as sprites to construct a character meta sprite including its animation or a map.

![](/Images/pedro.png)

Let´s pick up this image or sprite sheet from Montezuma's Revenge.

![](/Images/importimage2.png)

There are two ways to import a new image to a *Tile Set* element, first one is to use File > Import > Image... (Ctrl + I). This will create a *Tile Set* element with the name of the image. The second way to import an image is to create a *Tile Set* element, explained the the [Getting started](#Gettingstarted) section and then click over the new element and then click the *tree dots* button on the top part of the element window to browse your computer for an image.

All images after being imported will create if it doesn't exist already a folder name **Images** in the project root directory and I will copy the new imported image there with the extension *.bmp*.

![](/Images/importedimage2.png)

After the image is imported it is possible to zoom in/out with the mouse's scroll wheel and it will appear a new toobar button to hide or show the 8x8/8x16 grid.

![](/Images/changingcolors2.png)

Clicking any 8x8 cell in the image will show it in the zoon in quadrant at the left where is possible to change the colors. It is important to press the button save to apply the changes.

> ⚠️**WARNING**: Each 8x8/8x16 cell must be 4 colors maximum. Transparent color counts as one color leaving 3 colors left. This tool does not check if a cell has more than 4 colors.

The field **Pseudonym** is to give a name to any 8x8 cell. This is helpful when the proyect is exported, it will create a .asm file with all the cells used by the [Banks](#Banks) with this pseudonym as a constant value so you dont have to hardcode the actual tile index in the code. The value is taken from the [Bank](#Banks) itself. To know more about the generated files read the [Building the project](#Buildingtheproject) section.

<a name="Banks"/>

### 3.2 Banks

Banks are tiles grouped together. Is possible to have banks in different sizes (1kb, 2kb, 4kb). **Pattern Tables** are 4kb banks used as the main source for NES graphics. Pattern Tables can be either background or sprites. Banks are constructed from [Tile Sets](#TileSets). This will form a link inside the banks to each Tile Set used. If a Tile Set changes its tiles, it is renamed or removed, it will automatically update the bank. The field **Group** is to group tiles to share the same color order. By default each cell has its own group index but if one or more cells share the same group index then the colors they share, they will have the exact same order of appearance. The **Use** dropdown button is just to categorize the bank, later when you are creating a [Character](#Characters) for example, it will display only banks with type **Characters**.

When building the project, see: [Building the project](#Buildingtheproject), it will generate bank files for each bank object in the project.

![](/Images/bank2.png)

In the image above, we can have a bank for the sprites with 2kb of size. Then later in the code we can for example use this 2kb of a bank along side with music in the CHR Rom chip. Like this example using CA65:

```
.segment "CHARS"
    .incbin "../assets/sprites.bin"       ; 2kb for the sprites bank
    .include "sound/musicData.asm"        ; 2kb for music data
    .incbin "../assets/background.bin"    ; 4kb for bankground bank
```
Then it will look like this:

![](/Images/charsmemory.png)

<a name="Palettes"/>

### 3.3 Palettes

Here it is just simply, one four color palette where is possible to pick colors. This palettes are referenced by name for the [Characters](#Characters) and [Maps](#Maps).

![](/Images/palette.png)

<a name="Characters"/>

### 3.4 Characters

Characters are created by using banks. The tiles from this bank will be stored as a link to them, if one of those banks changes, it is renamed or deleted it will automatically updates the character.

If the bank is set to pattern table for background tiles, it won't appear as an option for character creation.

![](/Images/emptyCharacter.png)

Press the plus button in the tab to create a new animation.

![](/Images/emptyFrame2.png)

From here it is possible to create frames for the animation. Clicking the plus button will create a new frame of the animation. When there is more than one frame, the play button, stop, pause, previous frame and next frame are available.

Here is also possible to set the animation speed. This value is in seconds per frame and this is also used when building the project to be used in the assembly. More details explained in [Building the project](#Buildingtheproject) section.

![](/Images/editframe2.png)

<a name="Entities"/>

### 3.5 Entities

Entities are here to add sprites or dynamic elements to each map. Each entity will have by default an X and Y coordinate property and this can be adjusted when this entity is added to a map (read the [Maps](#Maps) section for how to add entities in a map). The second thing is to give it a distintic numeric Id for further identification.

An entity can be a meta sprite or a bunch of tiles from the background. That's why the first thing to do is to select the **Source** of the entity. This can be from a **Bank** or from a **Character**.

From a Bank, you can just add tiles from the selected bank on the right.

![](/Images/entities.png)

From a Character, you can select any already created character from the list under Characters and a different pose from the list of animation this character might have.

![](/Images/entities_2.png)

To finish the entity configuration, you can add properties to each entity that are needed for your particular game. X and Y coordinates are already the defaults for each entity but here by pressing the **Add** button, you can add for example the level where this entity is going to appear or extra attributes that might use.

![](/Images/add_property.png)

Once the entity is created, it will appear as an option under **Map elements** once you press the button **Add**.

<a name="Maps"/>

### 3.6 Maps

Maps are created by using banks. If the bank is set to pattern table for sprite tiles, it won't appear as an option for map creation.

From the tool bar is possible to select a 16x16 quadrant and assing a palette number from 0 to 3. Here is imposed the NES hardware restriction that palettes are only assigned to 16x16 pixels cell. Next to the selection tool is the paint tool and by selecting any cell from the [Bank](#Banks) area it is possible to "paint" over the map area. Keeping the mouse down will help the "paint" of multiple 8x8 cells at a time. Next is the **erase** tool. This tool will erase and will leave no data to that cell.

![](/Images/map2.png)

<a name="Buildingtheproject"/>

<a name="Worlds"/>

### 3.7 Worlds

todo: work in progress

## 4. Building the project

![](/Images/build3.png)

Building the project will create a bunch of files in the output directory:

+ For each [Bank](#Banks) element, it will generate a .bin file.
+ A .s file for each [Map](#Maps) element. This will use the field **Use RLE on maps** to compress or not the maps.
+ A .s file containing all the [Palette](#Palettes) elements called **palettes.s**.
+ A .s file containing all the tiles used by the banks called **tile_definition.s**.

<a name="Futurefeatures"/>

## 5. Future features

+ Support 8x16 sprites
+ Support for palette animations.
+ Potentially add Python support to elimiate the hardcoded process of exporting the files and for example the RLE compression can be adjusted as a script or something else custom needed depending on the current game project.

<a name="Knownbugs"/>

## 6. Known bugs

Real Github's Issues are needed here. For now the list is just here with no real ticket created.

+ Reimporting the same image to a TileSet element will make the tool crash.
+ Undo, Redo is not working inside each element.
+ After an image is imported, the source image is not released.
