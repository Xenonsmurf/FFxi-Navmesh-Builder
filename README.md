
<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/xenonsmurf/Ffxi_Navmesh_Builder">
    <img src="images/icon.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">FFXI NavMesh Builder</h3>

  <p align="center">
    <br />
    <a href="https://github.com/xenonsmurf/Ffxi_Navmesh_Builder"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://www.youtube.com/playlist?list=PLsww_EXH6VoprH94s967sgt_RM4ENYmb0">View Demo</a>
    ·
    <a href="https://github.com/xenonsmurf/Ffxi_Navmesh_Builder/issues">Report Bug</a>
    ·
    <a href="https://github.com/xenonsmurf/Ffxi_Navmesh_Builder/issues">Request Feature</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#requirements">Requirements</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

This project was created in hopes to make building navmeshes for FFXI easier, and more efficient. It has been a learning experience in both c# and c++, it’s not perfect and I plan on making more improvements. 

Using this project you should be able to build .obj files using collision data stored in the Dats, and build navmeshes using those .obj files.


### Built With

* [Visual studios professional](https://visualstudio.microsoft.com/vs/professional/)
* [.net5](https://dotnet.microsoft.com/download/dotnet/5.0)

<!-- GETTING STARTED -->
## Getting Started

1. Run Ffxi_Navmesh_Builder.exe as Admin.
2. Make sure your ffxi installation is correctly set, you can do this on the “dat” tab and click “set ffxi installation Path" and editing the textbox.

### Requirements

1. You will need to install .net5 to be able to use this application.
please download .net5 from (https://dotnet.microsoft.com/download/dotnet/5.0)

2. FFXI Installed.


### Installation

1. Download or Clone the repo.

   git clone https://github.com/xenonsmurf/Ffxi_Navmesh_Builder.git
   
2. Run Ffxi_Navmesh_Builder.exe as Admin.
  
<!-- USAGE EXAMPLES -->
## Usage
### navmesh settings all meshes are dumped with, changing these settings will affect performance for Topaz.

* float m_tileSize = 256;        
* float m_cellSize = 0.40f;
* float m_cellHeight = 0.20f;
* float m_agentHeight = 1.8f;    
* float m_agentRadius = 0.3f;     
* float m_agentMaxClimb = 0.5f;   
* float m_agentMaxSlope = 46.0f;
* float m_regionMinSize = 8;
* float m_regionMergeSize = 20;
* float m_edgeMaxLen = 12.0f;
* float m_edgeMaxError = 1.3f;
* float m_vertsPerPoly = 6.0f;
* float m_detailSampleDist = 6.0f;
* float m_detailSampleMaxError = 1.0f;
	
### if building meshes for player movement, use these settings.

* float m_tileSize = 64;         <<<< this can be changed for small zones.
* float m_cellSize = 0.20f;
* float m_cellHeight = 0.010f;
* float m_agentHeight = 1.8f;    
* float m_agentRadius = 0.7f;     <<<< if you make this too big it will break the mesh. 0.7f has been tested on most zones.
* float m_agentMaxClimb = 0.5f;   <<<< this might need changing for some zones. max climb changes from 0.3f to 0.5f, trial and error
* float m_agentMaxSlope = 46.0f;
* float m_regionMinSize = 8;
* float m_regionMergeSize = 20;
* float m_edgeMaxLen = 12.0f;
* float m_edgeMaxError = 1.3f;
* float m_vertsPerPoly = 6.0f;
* float m_detailSampleDist = 6.0f;
* float m_detailSampleMaxError = 1.0f;

#### Dat Tab

Here you can dump the zone collision data to obj files,

1. Click Load Zones.
This will read the English ZoneList.dat and populate the DataGridView with the zones found in the .dat.

2. Select the zone you want to build the collision OBJ file for.

3. Click "Build a obj file for selected Zone." 
this will build an obj file using collision data from the main zone.dat plus any submodels that are loaded.

4 Click "Build obj files for all zones."
this will build all obj for all zones using collision data from the main zone.dat plus any submodels that are loaded.

#### NavMesh Tab
here you can build navmeshes using FFXINAV.dll

1. These settings are the "Default settings Topaz NavMeshes are made with. Changes to these settings will affect performance on the server.

2. Click "Apply NavMesh Settings" this is a must, ffxinav.dll needs these settings to be able to build navmeshes.

3. Click "Select obj file to build a NavMesh for." this will build a navmesh for the selected obj.

4. Click "Build NavMeshes for all obj files." this will build navmeshes for all obj files.


# Pleae Note: 
When you click stop it will finish the Current NavMesh build.


#### FAQ

##### How do I edit the navmesh that was built with FFXINAV.dll in RecastDemo.exe?.
    
     *  1. Place the zone.obj file in  RecastDemo/Meshes/.
     *  2. Place the navmesh.nav in the same folder as RecastDemo.exe "RecastDemo".
     *  3. Rename "navmesh.nav" to "all_tiles_navmesh.bin".
     *  4. Open RecastDemo.exe-> on the right hand side click -> choose sample -> Tile Mesh.
           Iput Mesh -> select the .obj file you want to edit.
     *  5. Once the .obj is loaded on screen -> scroll down and click load. you will see the mesh load on screen as "blue".
           This might take a long time depending on the size of the zone.
     *  6. To remove tiles, on the left hand side click "Create Tiles" then on the mesh click Shift+ left mouse button.
     *  7. To rebuild the tile click on the part of the mesh with left mouse button.
     *  8. You can create off-mesh links with the tool on the left hand side. 
           once you have added all your off-mesh links you then need to click "Create Tiles" and "Build all Tiles".
     *  9. When you are finished click save from the tool menu on the right hand side.
     *  10.It will save as "all_tiles_navmesh.bin" you will need to rename this to "ZoneName.nav".   
       
##### when I select a zone from the list to build a collision obj file for nothing happens or I get an error?. 
   
* open an issue and check what info is in log.bin.

##### How do i deal with doors? the navmesh wont go past them?.
   
* If this happens,you can...
1. lower the agent radius 
2. you can open the obj file in blender and delete the door, 
3. you can open the obj and navmesh in recastdemo and add an offmesh links.

#### FFXINAV DLL
* CanSeeDestination.
* Unload.
* Initialize // needs path to log config file (Default_Config.conf which is included).
* Load  // loads the navmesh.
* LoadOBJFile.
* DumpNavMesh.
* GetLogMessage.
* FindPath.
* FindRandomPath.
* FindClosestPath. // this prevents exploits with navmesh / impassible terrain
* IsValidPosition. // can be used to make a grid for A star pathfinding. (if you didn't want to use recast detour to find a path).
* GetDistanceToWall. //this is distance to navmesh edge
* GetRotation.
* IsNavMeshEnabled.
* PathPoints.
* NavMeshSettings. //Lets you change the NavMesh settings before building a new mesh
* GetWaypoints. //returns the waypoints in the path


<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/xenonsmurf/Ffxi_Navmesh_Builder/issues) for a list of proposed features (and known issues).

<!-- CONTACT -->
## Contact

Discord xenonsmurf#3618

Project Link: [https://github.com/xenonsmurf/Ffxi_Navmesh_Builder](https://github.com/xenonsmurf/Ffxi_Navmesh_Builder)


<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* Devi Ltti, and Atom0s for their patience with my stupid questions.
* Thorny for his improvements to ffxinav.dll
* Vulture for his original dat.cs (collision mesh extraction tool).
* The DarkStar project / Topaz team for providing invaluable insight into the underlying workings of the game. 
