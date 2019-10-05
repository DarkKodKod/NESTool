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
[4. Known bugs](#Knownbugs)     

<a name="Overview"/>

## 1. Overview

![](/Images/nestool.png)

<a name="Menu"/>

### 1.1 Menu

![](/Images/menu.png)

#### File
![](/Images/file_menu.png)

From File is possible to create a new project or a new element like a [Tile Sets](#TileSets) or a [Character](#Characters).

A new project will have the extension **.proj** which is internaly a [TOML](https://github.com/toml-lang/toml) format and the name is given by the user. It will also create the folders **Banks**, **Characters**, **Maps** and **TileSets**. 

You can open an existing project, selecting a **.proj** file.

Close the current project.

Import any image from these formats: *.png, .bmp, .gif, .jpg, .jpeg, .jpe, .jfif, .tif, .tiff, .tga*. The image will reduce the colors with a Palette Quantizer algorithm to match the number of colors the NES can reproduce. More about this topic in the [Getting started](#Gettingstarted).

Recent project will contain all previous opened projects.

#### Edit
![](/Images/edit_menu.png)

Undo/Redo, Copy, Paste, Duplicate and Delete only affects the project's elements like a [Character](#Characters) element although the fucntionality is not complete yet so it is better not to use them.

#### Project
![](/Images/project_menu.png)

Project Properties is where is possible to reconfigure the project settings after the project is created. There are actually more options here change than when the project is created. For more information read the [Getting started](#Gettingstarted) section.

Build Project will create and export all maps, characters and pattern tables in the selected output folder. More on that in [Building the project](#Buildingtheproject) section.

#### Help
![](/Images/help_menu.png)

View Help redirects to this page in github.com.

<a name="Toolbar"/>

### 1.2 Toolbar

![](/Images/toolbar.png)

Toolbar Has the option to create a new project (explained in [Getting started](#Gettingstarted) section) or open an existing project, undo or redo (not recommended to use) and build project. The last one is explained in the [Building the project](#Buildingtheproject) section.

<a name="Gettingstarted"/>

## 2. Getting started

Once NESTool is opened for the first time, it will create in the root of the executable, the file **mappers.toml** and **config.toml** . Only the last one if some of the configuration of the tool changes like the window size.

![](/Images/newproject.png)

![](/Images/projectproperties.png)

![](/Images/newelement.png)

![](/Images/tree.png)

<a name="TileSets"/>

### 2.1 Tile Sets

![](/Images/mario.png)

![](/Images/importimage.png)

![](/Images/importedimage.png)

![](/Images/changingcolors.png)

<a name="Banks"/>

### 2.2 Banks

<a name="Characters"/>

### 2.3 Characters

<a name="Maps"/>

### 2.4 Maps

<a name="Buildingtheproject"/>

## 3. Building the project

<a name="Knownbugs"/>

## 4. Known bugs
