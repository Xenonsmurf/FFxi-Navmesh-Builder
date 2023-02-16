// ***********************************************************************
// Assembly         : Author : Xenonsmurf Created : 12-28-2020 Last Modified By : Xenonsmurf Last Modified
// Author           : Xenonsmurf Created : 12-28-2020 Last Modified By : Xenonsmurf Last Modified
// Created          : 12-28-2020 Last Modified By : Xenonsmurf Last Modified
//
// Last Modified By : Xenonsmurf Last Modified
// Last Modified On : 05-15-2021
// ***********************************************************************
// <copyright file="Imports.cs" company="Xenonsmurf">
//     Copyright © 2020
// </copyright>
// <summary></summary>
// ***********************************************************************

// <summary>
// Special thanks to Thorny for his updated way for returning waypoints, you're awesome! thanks man.
// </summary>

// ***********************************************************************

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Ffxi_Navmesh_Builder.Common
{
    /// <summary>
    /// Class Ffxinav.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Ffxinav : IDisposable
    {
        /// <summary>
        /// The old string
        /// </summary>
        public string OldString = string.Empty;

        /// <summary>
        /// The native object
        /// </summary>
        private IntPtr _mPNativeObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ffxinav" /> class.
        /// </summary>
        public Ffxinav()
        {
            _mPNativeObject = CreateFFXINavClass();
            Waypoints = new List<PositionT>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Ffxinav" /> class.
        /// </summary>
        ~Ffxinav()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dumping mesh].
        /// </summary>
        /// <value><c> true </c> if [dumping mesh]; otherwise, <c> false </c>.</value>
        public bool DumpingMesh { get; set; }

        /// <summary>
        /// Gets or sets the way-points.
        /// </summary>
        /// <value>The way-points.</value>
        public List<PositionT> Waypoints { get; private set; }

        /// <summary>
        /// Releases the items.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="itemsHandle">The items handle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReleaseItems(IntPtr pFfxiNavClassObject, ItemsSafeHandle itemsHandle);

        /// <summary>
        /// Converts to single.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Single.</returns>
        public static float ToSingle(double value)
        {
            return (float)value;
        }

        /// <summary>
        /// Determines whether this instance [can see destination] the specified start. Raycast
        /// check between two points.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns><c> true </c> if this instance [can see destination] the specified start; otherwise, <c>
        /// false </c> .</returns>
        public bool CanWeSeeDestination(PositionT start, PositionT end)
        {
            return CanSeeDestination(_mPNativeObject, start, end);
        }

        /// <summary>
        /// Changes the NavMesh settings.
        /// </summary>
        /// <param name="cellSize">Size of the cell.</param>
        /// <param name="cellHeight">Height of the cell.</param>
        /// <param name="agentHeight">Height of the agent.</param>
        /// <param name="agentRadius">The agent radius.</param>
        /// <param name="maxClimb">The maximum climb.</param>
        /// <param name="maxSlope">The maximum slope.</param>
        /// <param name="tileSize">Size of the tile.</param>
        /// <param name="regionMinSize">Minimum size of the region.</param>
        /// <param name="regionMergeSize">Size of the region merge.</param>
        /// <param name="edgeMaxLen">Maximum length of the edge.</param>
        /// <param name="edgeError">The edge error.</param>
        /// <param name="vertsPp">The verts Per Polygon.</param>
        /// <param name="detailSampDistance">The detail sample distance.</param>
        /// <param name="detailMaxError">The detail maximum error.</param>
        /// <param name="debugMode">if set to <c>true</c> [debug mode].</param>
        public void ChangeNavMeshSettings(double cellSize, double cellHeight, double agentHeight, double agentRadius,
            double maxClimb,
            double maxSlope, double tileSize, double regionMinSize, double regionMergeSize, double edgeMaxLen,
            double edgeError, double vertsPp,
            double detailSampDistance, double detailMaxError, bool debugMode)
        {
            navMeshSettings(_mPNativeObject, cellSize, cellHeight, agentHeight, agentRadius, maxClimb, maxSlope,
                tileSize,
                regionMinSize, regionMergeSize, edgeMaxLen, edgeError, vertsPp, detailSampDistance, detailMaxError, debugMode);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Finds the distance from the specified position to the nearest polygon wall (NavMesh Edge).
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>System.Double.</returns>
        public double DistanceToWall(PositionT start)
        {
            try
            {
                if (start.X != 0 && start.Z != 0)
                    return GetDistanceToWall(_mPNativeObject, start);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// Builds and Saves a NavMesh, remember to pass NavMesh Settings to the DLL before you try
        /// and build a mesh.
        /// </summary>
        /// <param name="file">The file.</param>
        public async Task Dump_NavMesh(string file)
        {
            if (DumpNavMesh(_mPNativeObject, file))
            {
                Unload();
                UnloadMeshBuilder();
            }
        }

        /// <summary>
        /// Sets the user defined flags for the specified polygon. Enable/Disable nearest polygon.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="enable">if set to <c> true </c> [enable].</param>
        /// <param name="useCustom">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool EnableOrDisableNearestPoly(PositionT pos, bool enable, bool useCustom)
        {
            return EnableNearestPoly(_mPNativeObject, pos, enable, useCustom);
        }

        /// <summary>
        /// Finds the closest path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="useCustonNavMeshes">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        public void FindClosestPath(PositionT start, PositionT end, bool useCustonNavMeshes)
        {
            FindClosestPath(_mPNativeObject, start, end, useCustonNavMeshes);
        }

        /// <summary>
        /// Finds the path to position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="useCustonNavMeshes">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        public void FindPathToPosi(PositionT start, PositionT end, bool useCustonNavMeshes)
        {
            findPath(_mPNativeObject, start, end, useCustonNavMeshes);
        }

        /// <summary>
        /// Finds a random path Looks for a random location on the NavMesh within the reach of
        /// specified location, then finds a path to that location.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="maxRadius">The maximum radius.</param>
        /// <param name="maxTurns">The maximum turns.</param>
        /// <param name="useCustom">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool FindRandomPath(PositionT start, float maxRadius, sbyte maxTurns, bool useCustom)
        {
            return FindRandomPath(_mPNativeObject, start, maxRadius, maxTurns, useCustom);
        }

        /// <summary>
        /// Gets the Rotation between two points.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.SByte.</returns>
        public sbyte Getrotation(PositionT start, PositionT end)
        {
            return GetRotation(_mPNativeObject, start, end);
        }

        /// <summary>
        /// Gets the way-points from the DLL.
        /// </summary>
        public void GetWaypoints()
        {
            Waypoints = new List<PositionT>(Get_WayPoints_Wrapper());
        }

        /// <summary>
        /// Determines whether [NavMesh is enabled].
        /// </summary>
        /// <returns><c> true </c> if [NavMesh enabled]; otherwise, <c> false </c>.</returns>
        public bool IsNavMeshEnabled()
        {
            if (isNavMeshEnabled(_mPNativeObject) == false) return false;
            if (isNavMeshEnabled(_mPNativeObject))
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether [is position in water] [the specified position].
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns><c> true </c> if [is position in water] [the specified position]; otherwise, <c> false </c>.</returns>
        public bool IsPositionInWater(PositionT position)
        {
            return InWater(_mPNativeObject, position);
        }

        /// <summary>
        /// Determines whether [valid position on the mesh] [the specified start].
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="useCustom">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        /// <returns><c> true </c> if [is valid position] [the specified start]; otherwise, <c> false </c>.</returns>
        public bool IsValidPosition(PositionT start, bool useCustom)
        {
            return IsValidPosition(_mPNativeObject, start, useCustom);
        }

        /// <summary>
        /// Loads the NavMesh
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool LoadNavMesh(string file)
        {
            return LoadMesh(_mPNativeObject, file);
        }

        /// <summary>
        /// Loads the obj file. used when building NavMeshes.
        /// </summary>
        /// <param name="file">The file.</param>
        public void LoadObJfile(string file)
        {
            Thread.Sleep(2000);
            LoadOBJFile(_mPNativeObject, file);
        }

        /// <summary>
        /// Gets the current Way-Point count in the DLL.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int PathCount()
        {
            return pathpoints(_mPNativeObject);
        }

        /// <summary>
        /// Unloads the current loaded mesh, don't really need to do it as when you load a new mesh
        /// it will unload the current.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Unload()
        {
            return unload(_mPNativeObject);
        }

        /// <summary>
        /// Unloads the mesh builder.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UnloadMeshBuilder()
        {
            return unloadMeshBuilder(_mPNativeObject);
        }

        /// <summary>
        /// Determines whether this instance [can see destination (next way-point)], This uses
        /// Recast Detour Raycast. This method is meant to be used for quick, short distance checks.
        /// Use Case Restriction The raycast ignores the y-value of the end position. (2D check.)
        /// This places significant limits on how it can be used. For example: Consider a scene
        /// where there is a main floor with a second floor balcony that hangs over the main floor.
        /// So the first floor mesh extends below the balcony mesh. The start position is somewhere
        /// on the first floor. The end position is on the balcony. The raycast will search toward
        /// the end position along the first floor mesh. If it reaches the end position's
        /// x,z-coordinates it will indicate FLT_MAX (no wall hit), meaning it reached the end
        /// position. This is one example of why this method is meant for short distance checks.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns><c> true </c> If nothing blocks line of sight between two Way-points on the mesh;
        /// otherwise, <c> false </c>.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "CanSeeDestination", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool CanSeeDestination(IntPtr pFfxiNavClassObject, PositionT start, PositionT end);

        /// <summary>
        /// Creates the ffxinav class.
        /// </summary>
        /// <returns>IntPtr.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "CreateFFXINavClass", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr CreateFFXINavClass();

        /// <summary>
        /// Disposes the ffxinav class.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The ffxinav class object.</param>
        [DllImport("FFXINAV.dll", EntryPoint = "DisposeFFXINavClass", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void DisposeFFXINavClass(IntPtr pFfxiNavClassObject);

        /// <summary>
        /// Builds and saves the NavMesh.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="path">The path. e.g string.Format(@"{0}\\Dumped NavMeshes\\100.nav", Directory.GetCurrentDirectory())))</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "DumpNavMesh", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool DumpNavMesh(IntPtr pFfxiNavClassObject, string path);

        /// <summary>
        /// Sets the user defined flags for the specified polygon. Enable/Disable nearest polygon.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="pos">The position.</param>
        /// <param name="enable">if set to <c> true </c> [enable].</param>
        /// <param name="useCustomMesh">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "EnableNearestPoly", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool EnableNearestPoly(IntPtr pFfxiNavClassObject, PositionT pos, bool enable,
            bool useCustomMesh);

        /// <summary>
        /// Finds the closest path.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <param name="useCustomNavMesh">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        [DllImport("FFXINAV.dll", EntryPoint = "FindClosestPath", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void FindClosestPath(IntPtr pFfxiNavClassObject, PositionT start, PositionT end,
            bool useCustomNavMesh);

        /// <summary>
        /// Finds the path.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <param name="useCustomNavMesh">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        [DllImport("FFXINAV.dll", EntryPoint = "FindPath", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void findPath(IntPtr pFfxiNavClassObject, PositionT start, PositionT end,
            bool useCustomNavMesh);

        /// <summary>
        /// Finds a random path Looks for a random location on the NavMesh within the reach of
        /// specified location, then finds a path to that location.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The ffxinav class object.</param>
        /// <param name="start">The start.</param>
        /// <param name="maxRadius">The maximum radius.</param>
        /// <param name="maxTurns">The maximum turns.</param>
        /// <param name="useCustom">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from Noesis <c> true </c> [use custom].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "FindRandomPath", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool FindRandomPath(IntPtr pFfxiNavClassObject, PositionT start, float maxRadius,
            sbyte maxTurns, bool useCustom);

        /// <summary>
        /// Gets the way-points (path) that the DLL has found.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="pointer">A pointer to an IntPtr(pointer to a pointer) to hold vector address.</param>
        /// <returns>The amount of waypoints in path.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "Get_WayPoints", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Get_WayPoints(IntPtr pFfxiNavClassObject, out IntPtr pointer);

        /// <summary>
        /// Finds the distance from the specified position to the nearest polygon wall (NavMesh Edge).
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start position.</param>
        /// <returns>System.Double.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "GetDistanceToWall", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern double GetDistanceToWall(IntPtr pFfxiNavClassObject, PositionT start);

        /// <summary>
        /// Gets the rotation between two points.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <returns>System.SByte.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "GetRotation", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern sbyte GetRotation(IntPtr pFfxiNavClassObject, PositionT start, PositionT end);

        /// <summary>
        /// Checks if Position is in Water, this will only work if Water has been added to the mesh
        /// using Convex volumes.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="pos">The position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "InWater", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool InWater(IntPtr pFfxiNavClassObject, PositionT pos);

        /// <summary>
        /// Determines whether [NavMesh is enabled] [the specified p ffxinav class object].
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <returns><c> true </c> if [NavMesh is enabled] [the specified p ffxinav class object]; otherwise,
        /// <c> false </c>.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "isNavMeshEnabled", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool isNavMeshEnabled(IntPtr pFfxiNavClassObject);

        /// <summary>
        /// Determines whether [is valid position on the NavMesh] [the specified p ffxinav class object].
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="start">The start.</param>
        /// <param name="useCustom">always leave false if you are using meshes made with FFXINAV.dll or XIExporter, set true
        /// if meshes were built with obj's files from noesis <c> true </c> [use custom].</param>
        /// <returns><c> true </c> if [is valid position on the NavMesh] [the specified p ffxinav class
        /// object]; otherwise, <c> false </c>.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "IsValidPosition", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool IsValidPosition(IntPtr pFfxiNavClassObject, PositionT start, bool useCustom);

        /// <summary>
        /// Loads the mesh.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="path">The path to the NavMesh</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "LoadMesh", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool LoadMesh(IntPtr pFfxiNavClassObject, string path);

        /// <summary>
        /// Loads the object file.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="path">The path to the Obj file, used when building NavMeshes.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "LoadOBJFile", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool LoadOBJFile(IntPtr pFfxiNavClassObject, string path);

        /// <summary>
        /// NavMesh Settings used for building NavMeshes, you need to set these before you try and
        /// build a mesh. Here is a good post to read for the settings. http://digestingduck.blogspot.com/2009/08/recast-settings-uncovered.html.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <param name="cellSize">Size of the cell.</param>
        /// <param name="cellHeight">Height of the cell.</param>
        /// <param name="agentHight">The agent hight.</param>
        /// <param name="agentRadius">The agent radius.</param>
        /// <param name="maxClimb">The maximum climb.</param>
        /// <param name="maxSlope">The maximum slope.</param>
        /// <param name="tileSize">Size of the tile.</param>
        /// <param name="regionMinSize">Minimum size of the region.</param>
        /// <param name="regionMergeSize">Size of the region merge.</param>
        /// <param name="edgeMaxLen">Maximum length of the edge.</param>
        /// <param name="edgeError">The edge error.</param>
        /// <param name="vertsPp">The m_verts Per Polygon.</param>
        /// <param name="detailSampDistance">The detail sample distance.</param>
        /// <param name="detailMaxError">The detail maximum error.</param>
        /// <param name="debugMode">Enable or disable file logging</param>
        [DllImport("FFXINAV.dll", EntryPoint = "NavMeshSettings", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void navMeshSettings(IntPtr pFfxiNavClassObject, double cellSize, double cellHeight,
            double agentHight, double agentRadius, double maxClimb,
            double maxSlope, double tileSize, double regionMinSize, double regionMergeSize, double edgeMaxLen,
            double edgeError, double vertsPp,
            double detailSampDistance, double detailMaxError, bool debugMode);

        /// <summary>
        /// Gets the current Way-point count, the specified p ffxinav class object.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxinav class object.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "Pathpoints", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int pathpoints(IntPtr pFfxiNavClassObject);

        /// <summary>
        /// Unloads the NavMesh, and Resets.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The ffxinav class object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "unload", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern bool unload(IntPtr pFfxiNavClassObject);

        /// <summary>
        /// Unloads the mesh builder.
        /// </summary>
        /// <param name="pFfxiNavClassObject">The p ffxi nav class object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("FFXINAV.dll", EntryPoint = "unloadMeshBuilder", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern bool unloadMeshBuilder(IntPtr pFfxiNavClassObject);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="bDisposing"><c> true </c> to release both managed and unmanaged resources; <c> false </c> to release
        /// only unmanaged resources.</param>
        private void Dispose(bool bDisposing)
        {
            if (_mPNativeObject != IntPtr.Zero)
            {
                DisposeFFXINavClass(_mPNativeObject);
                _mPNativeObject = IntPtr.Zero;
            }

            if (bDisposing) GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the way points wrapper, used for returning Way-points from the DLL.
        /// </summary>
        /// <returns>An array of PositionT objects containing the current waypoints from DLL</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private PositionT[] Get_WayPoints_Wrapper()
        {
            var numberOfElements = Get_WayPoints(_mPNativeObject, out var unmanagedPointerToVector);
            var sizeOfElement = Marshal.SizeOf(typeof(PositionT));

            var returnArray = new PositionT[numberOfElements];
            for (var x = 0; x < numberOfElements; x++)
            {
                var unmanagedPointerToVectorEntry = new IntPtr(unmanagedPointerToVector.ToInt32() + x * sizeOfElement);
                returnArray[x] = (PositionT)Marshal.PtrToStructure(unmanagedPointerToVectorEntry, typeof(PositionT));
            }

            return returnArray;
        }

        // Create a SafeHandle, informing the base class that this SafeHandle instance "owns" the
        // handle, and therefore SafeHandle should call our ReleaseHandle method when the SafeHandle
        // is no longer in use.
        /// <summary>
        /// Class ItemsSafeHandle.
        /// </summary>
        /// <seealso cref="Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid" />
        public abstract class ItemsSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ItemsSafeHandle"/> class.
            /// </summary>
            protected ItemsSafeHandle()
                : base(true)
            {
            }

            /// <summary>
            /// When overridden in a derived class, executes the code required to free the handle.
            /// </summary>
            /// <returns><see langword="true" /> if the handle is released successfully; otherwise, in the event of a catastrophic failure, <see langword="false" />. In this case, it generates a releaseHandleFailed Managed Debugging Assistant.</returns>
            protected override bool ReleaseHandle()
            {
                return true;
            }
        }
    }
}