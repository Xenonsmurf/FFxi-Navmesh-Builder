// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-16-2021
// ***********************************************************************
// <copyright file="HomeView.xaml.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using Ffxi_Navmesh_Builder.Common;
using Ffxi_Navmesh_Builder.Common.dat;
using FFXI_Navmesh_Builder.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FFXI_Navmesh_Builder.Views
{
    /// <summary>
    /// Class HomeView.
    /// Implements the <see cref="System.Windows.Controls.UserControl" />
    /// Implements the <see cref="System.Windows.Markup.IComponentConnector" />
    /// Implements the <see cref="System.Windows.Markup.IStyleConnector" />
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <seealso cref="System.Windows.Markup.IStyleConnector" />
    public partial class HomeView
    {
        /// <summary>
        /// The build meshes
        /// </summary>
        private bool _buildMeshes;

        /// <summary>
        /// True while a NavMesh build is running. FFXINAV.dll is not thread safe, so
        /// only one build may talk to it at a time.
        /// </summary>
        private bool _isNavBuildRunning;

        /// <summary>
        /// Gets or sets the tnames.
        /// </summary>
        /// <value>The tnames.</value>
        public TopazNames Tnames { get; set; }

        /// <summary>
        /// The cancellation token
        /// </summary>
        private CancellationTokenSource _cancellationToken;

        /// <summary>
        /// The dumping map dats
        /// </summary>
        private bool _dumpingMapDats;

        /// <summary>
        /// The dump subregion information to XML
        /// </summary>
        private bool _dumpSubregionInfoToXml;

        /// <summary>
        /// The save entityinfo
        /// </summary>
        private bool _saveEntityinfo;

        /// <summary>
        /// The save sub regioninfo
        /// </summary>
        private bool _saveSubRegioninfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class.
        /// </summary>
        public HomeView()
        {
            InitializeComponent();
            Log = new Log();
            Dat = new dat(Log, this, FFxiInstallPath);
            Tnames = new TopazNames();
            var version = GetType().Assembly.GetName().Version;
            if (version is not null)
                VersionLb.Content = version.ToString();
        }

        /// <summary>
        /// Gets or sets the f fxi install path.
        /// </summary>
        /// <value>The f fxi install path.</value>
        public string FFxiInstallPath { get; set; } = "C:/Program Files (x86)/PlayOnline/SquareEnix/FINAL FANTASY XI/";
        public string Zone0 { get; set; } = "C:/Program Files (x86)/PlayOnline/SquareEnix/FINAL FANTASY XI/ROM/0/28.DAT";
        public string Zone0bk { get; set; } = "ROM/0/28.DAT";

        /// <summary>
        /// Gets or sets the ffxi nav.
        /// </summary>
        /// <value>The ffxi nav.</value>
        private Ffxinav _ffxiNav { get; set; }

        /// <summary>
        /// Gets or sets the dat.
        /// </summary>
        /// <value>The dat.</value>
        private dat Dat { get; set; }

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>The log.</value>
        private Log Log { get; set; }

        /// <summary>
        /// Gets or sets the zone dat.
        /// </summary>
        /// <value>The zone dat.</value>
        private ParseZoneModelDat ZoneDat { get; set; }

        /// <summary>
        /// Handles the Click event of the AllOBJBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async void AllOBJBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (AllObjBtn.Content)
                {
                    case "Build NavMeshes for all .OBJ files.":
                        if (_isNavBuildRunning)
                        {
                            Log.AddDebugText(RtbDebug, @"A NavMesh build is already running, wait for it to finish.");
                            return;
                        }

                        var path = $@"{Directory.GetCurrentDirectory()}\Map Collision obj files";
                        if (!Directory.Exists(path))
                        {
                            Log.AddDebugText(RtbDebug, $@"Folder not found: {path}");
                            return;
                        }

                        // The old code passed the pattern through string.Format, which ignored it
                        // and fed every file type (nav, mtl, ...) to the native obj loader.
                        var objFiles = Directory.GetFiles(path, "*.obj", SearchOption.TopDirectoryOnly);
                        Log.AddDebugText(RtbDebug, $@"{objFiles.Length} .obj files found in Map Collision obj files folder.");
                        if (objFiles.Length == 0) return;

                        AllObjBtn.Content = @"Stop building NavMeshes.";
                        _isNavBuildRunning = true;
                        SelectObjBtn.IsEnabled = false;
                        _buildMeshes = true;
                        try
                        {
                            for (var i = 0; i < objFiles.Length; i++)
                            {
                                if (!_buildMeshes) break;
                                var file = objFiles[i];
                                Log.AddDebugText(RtbDebug, $@"[{i + 1}/{objFiles.Length}] Building NavMesh for {Path.GetFileName(file)}, please wait!...");

                                var navPath = Path.Combine(Directory.GetCurrentDirectory(), "Dumped NavMeshes",
                                    $"{Path.GetFileNameWithoutExtension(file)}.nav");
                                if (File.Exists(navPath))
                                {
                                    var result = MessageBox.Show(
                                        $@"Are you sure you want to overwrite {Path.GetFileName(navPath)} ?",
                                        "NavMesh", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                                    if (result == MessageBoxResult.Cancel) break;
                                    if (result != MessageBoxResult.Yes) continue;
                                }

                                _cancellationToken = new CancellationTokenSource();
                                await BuildNavMesh(file);
                            }
                        }
                        finally
                        {
                            AllObjBtn.Content = @"Build NavMeshes for all .OBJ files.";
                            _buildMeshes = false;
                            _isNavBuildRunning = false;
                            SelectObjBtn.IsEnabled = true;
                        }
                        Log.AddDebugText(RtbDebug, @"Finished building NavMeshes.");
                        return;

                    case "Stop building NavMeshes.":
                        _buildMeshes = false;
                        _cancellationToken?.Cancel();
                        AllObjBtn.Content = @"Build NavMeshes for all .OBJ files.";
                        Log.AddDebugText(RtbDebug, @"Stopping after the current NavMesh finishes...");
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
            }
        }

        /// <summary>
        /// Handles the Click event of the BuildAllObJbtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BuildAllObJbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (BuildAllObJbtn.Content)
                {
                    case "Build obj files for all zones.":
                        Log.AddDebugText(RtbDebug, @"Dumping all map.dats = true");
                        BuildAllObJbtn.Content = @"Stop Building obj files for all zones.";
                        _dumpingMapDats = true;
                        SubRegion.IsEnabled = false;
                        Entity.IsEnabled = false;
                        SubTp.IsEnabled = false;
                        EntTp.IsEnabled = false;
                        _ = DumpAllObjFilesAsync(true);
                        break;

                    case "Stop Building obj files for all zones.":
                        {
                            Log.AddDebugText(RtbDebug, @"Dumping all map.dats = false");
                            BuildAllObJbtn.Content = @"Build obj files for all zones.";
                            _ = DumpAllObjFilesAsync(false);
                            _dumpingMapDats = false;
                            SubTp.IsEnabled = true;
                            EntTp.IsEnabled = true;
                            SubRegion.IsEnabled = true;
                            Entity.IsEnabled = true;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
            }
        }

        /// <summary>
        /// Builds the nav mesh. Ensures FFXINAV.dll is loaded and has validated settings before
        /// the native build runs — calling DumpNavMesh without settings kills the whole process
        /// with a native access violation that .NET cannot catch.
        /// </summary>
        /// <param name="file">The .obj file to build a NavMesh from.</param>
        private async Task BuildNavMesh(string file)
        {
            // UI-thread work first: create the native class and push validated settings into it.
            if (!EnsureFfxiNav()) return;
            if (!ApplyNavMeshSettingsFromUi()) return;

            var navDir = Path.Combine(Directory.GetCurrentDirectory(), "Dumped NavMeshes");
            Directory.CreateDirectory(navDir);
            var navPath = Path.Combine(navDir, $"{Path.GetFileNameWithoutExtension(file)}.nav");

            async Task Function()
            {
                try
                {
                    if (!_buildMeshes)
                    {
                        _cancellationToken?.Cancel();
                        return;
                    }

                    if (!ValidateObjFile(file)) return;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    Log.AddDebugText(RtbDebug, $@"Handing {Path.GetFileName(file)} to FFXINAV.dll, this can take a while for big zones...");

                    var built = await _ffxiNav.Dump_NavMesh(file);
                    stopWatch.Stop();
                    var elapsed = stopWatch.Elapsed;
                    var elapsedTime = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds / 10:00}";

                    if (built && File.Exists(navPath))
                    {
                        var navSize = new FileInfo(navPath).Length;
                        Log.AddDebugText(RtbDebug, $@"NavMesh saved to {navPath} ({navSize / 1024:N0} KB). Time Taken to Build NavMesh = {elapsedTime}");
                        Log.LogFile($"NavMesh built: {navPath} ({navSize} bytes) in {elapsedTime}", nameof(HomeView));
                    }
                    else if (built)
                    {
                        Log.AddDebugText(RtbDebug, $@"FFXINAV.dll reported success but {navPath} was not created. Time Taken = {elapsedTime}");
                        Log.LogFile($"NavMesh build reported success but output missing: {navPath}", nameof(HomeView));
                    }
                    else
                    {
                        Log.AddDebugText(RtbDebug, $@"FFXINAV.dll could not build a NavMesh from {Path.GetFileName(file)} (build returned false). Time Taken = {elapsedTime}");
                        Log.LogFile($"NavMesh build failed for {file}", nameof(HomeView));
                    }
                }
                catch (Exception ex)
                {
                    Log.LogFile(ex.ToString(), nameof(HomeView));
                    Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
                }
            }
            await Task.Run(Function, _cancellationToken.Token);
        }

        /// <summary>
        /// Makes sure the FFXINAV.dll wrapper exists, creating it if needed. Previously the wrapper
        /// was only created when the NavMesh tab got focus, so builds could run against a null or
        /// settings-less instance.
        /// </summary>
        /// <returns><c>true</c> if the wrapper is ready; otherwise <c>false</c>.</returns>
        private bool EnsureFfxiNav()
        {
            if (_ffxiNav != null) return true;
            try
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FFXINAV.dll")))
                {
                    Log.AddDebugText(RtbDebug, @"FFXINAV.dll was not found next to the application, cannot build NavMeshes.");
                    return false;
                }

                _ffxiNav = new Ffxinav();
                Log.AddDebugText(RtbDebug, @"FFXINAV.dll loaded.");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"Failed to load FFXINAV.dll: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reads the NavMesh settings from the UI (falling back to the documented defaults when a
        /// field was cleared) and applies them to FFXINAV.dll. The DLL needs settings before every
        /// build; empty fields used to be sent as 0 which crashes the native tiler.
        /// </summary>
        /// <returns><c>true</c> if settings were applied; otherwise <c>false</c>.</returns>
        private bool ApplyNavMeshSettingsFromUi()
        {
            try
            {
                var cellSize = cellSizeValue.Value ?? 0.4;
                var cellHeight = cellHeightValue.Value ?? 0.2;
                var agentHeight = agentHeightValue.Value ?? 1.8;
                var agentRadius = agentRadiusValue.Value ?? 0.3;
                var maxClimb = maxClimbValue.Value ?? 0.5;
                var maxSlope = maxSlopeValue.Value ?? 46.0;
                var tileSize = tileSizeValue.Value ?? 256.0;
                var regionMinSize = regionMinSizeValue.Value ?? 8.0;
                var regionMergeSize = regionMergeSizeValue.Value ?? 20.0;
                var edgeMaxLen = edgeMaxLenValue.Value ?? 12.0;
                var edgeMaxError = edgeMaxErrorValue.Value ?? 1.3;
                var vertsPerPoly = vertsPerPolyValue.Value ?? 6.0;
                var detailSampleDist = detailSampleDistanceValue.Value ?? 6.0;
                var detailSampleMaxError = detailSampleMaxErrorValue.Value ?? 1.0;
                var dllDebug = dllDebugMode.IsChecked == true;

                _ffxiNav.ChangeNavMeshSettings(cellSize, cellHeight, agentHeight, agentRadius, maxClimb,
                    maxSlope, tileSize, regionMinSize, regionMergeSize, edgeMaxLen, edgeMaxError, vertsPerPoly,
                    detailSampleDist, detailSampleMaxError, dllDebug);

                Log.AddDebugText(RtbDebug,
                    $@"NavMesh settings applied: cellSize={cellSize}, cellHeight={cellHeight}, agentHeight={agentHeight}, agentRadius={agentRadius}, maxClimb={maxClimb}, maxSlope={maxSlope}, tileSize={tileSize}, regionMinSize={regionMinSize}, regionMergeSize={regionMergeSize}, edgeMaxLen={edgeMaxLen}, edgeMaxError={edgeMaxError}, vertsPerPoly={vertsPerPoly}, detailSampleDist={detailSampleDist}, detailSampleMaxError={detailSampleMaxError}, dllDebugMode={dllDebug}");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"Failed to apply NavMesh settings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Streams through an .obj file and sanity checks it before it is handed to FFXINAV.dll.
        /// Junk geometry (NaN vertices, or vertices millions of units out — see upstream issue #7
        /// with Ceizak Battlegrounds dats) makes the native tiler allocate a gigantic grid and
        /// crash the process, so bad files are rejected here with a readable error instead.
        /// </summary>
        /// <param name="file">The .obj file to validate.</param>
        /// <returns><c>true</c> if the file looks buildable; otherwise <c>false</c>.</returns>
        private bool ValidateObjFile(string file)
        {
            const double saneCoordinateLimit = 20000;
            try
            {
                var info = new FileInfo(file);
                if (!info.Exists)
                {
                    Log.AddDebugText(RtbDebug, $@"Obj file not found: {file}");
                    return false;
                }
                if (info.Length == 0)
                {
                    Log.AddDebugText(RtbDebug, $@"Obj file is empty: {file}");
                    return false;
                }
                if (!string.Equals(Path.GetExtension(file), ".obj", StringComparison.OrdinalIgnoreCase))
                {
                    Log.AddDebugText(RtbDebug, $@"Skipping {Path.GetFileName(file)}, not a .obj file.");
                    return false;
                }
                if (info.Length > 300L * 1024 * 1024)
                    Log.AddDebugText(RtbDebug, $@"Warning: {Path.GetFileName(file)} is {info.Length / (1024 * 1024)} MB, a 32-bit build may run out of memory on files this big.");

                Log.AddDebugText(RtbDebug, $@"Validating {Path.GetFileName(file)} ({info.Length / (1024 * 1024)} MB)...");

                long vertexCount = 0, faceCount = 0, badVertexCount = 0;
                double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
                double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

                using (var reader = new StreamReader(file))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length < 2) continue;
                        if (line[0] == 'v' && line[1] == ' ')
                        {
                            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length < 4
                                || !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                                || !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                                || !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var z)
                                || double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z)
                                || double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z))
                            {
                                badVertexCount++;
                                continue;
                            }

                            vertexCount++;
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                            if (z < minZ) minZ = z;
                            if (z > maxZ) maxZ = z;
                        }
                        else if (line[0] == 'f' && line[1] == ' ')
                        {
                            faceCount++;
                        }
                    }
                }

                if (badVertexCount > 0)
                {
                    Log.AddDebugText(RtbDebug, $@"{Path.GetFileName(file)} contains {badVertexCount} unreadable/NaN vertices, refusing to build (re-dump the obj for this zone).");
                    Log.LogFile($"Obj validation failed for {file}: {badVertexCount} bad vertices", nameof(HomeView));
                    return false;
                }
                if (vertexCount == 0 || faceCount == 0)
                {
                    Log.AddDebugText(RtbDebug, $@"{Path.GetFileName(file)} has no geometry (vertices={vertexCount}, faces={faceCount}), refusing to build.");
                    Log.LogFile($"Obj validation failed for {file}: no geometry", nameof(HomeView));
                    return false;
                }

                var extentX = maxX - minX;
                var extentY = maxY - minY;
                var extentZ = maxZ - minZ;
                if (Math.Abs(minX) > saneCoordinateLimit || Math.Abs(maxX) > saneCoordinateLimit ||
                    Math.Abs(minY) > saneCoordinateLimit || Math.Abs(maxY) > saneCoordinateLimit ||
                    Math.Abs(minZ) > saneCoordinateLimit || Math.Abs(maxZ) > saneCoordinateLimit)
                {
                    Log.AddDebugText(RtbDebug, $@"{Path.GetFileName(file)} bounding box is insane (X {minX:F1}..{maxX:F1}, Y {minY:F1}..{maxY:F1}, Z {minZ:F1}..{maxZ:F1}). It likely contains junk vertices from the dat (known issue with some zones). Refusing to build, FFXINAV.dll would crash trying to tile this.");
                    Log.LogFile($"Obj validation failed for {file}: bounding box out of range X {minX}..{maxX} Y {minY}..{maxY} Z {minZ}..{maxZ}", nameof(HomeView));
                    return false;
                }

                Log.AddDebugText(RtbDebug, $@"{Path.GetFileName(file)} looks good: {vertexCount:N0} vertices, {faceCount:N0} faces, bounds X {minX:F1}..{maxX:F1}, Y {minY:F1}..{maxY:F1}, Z {minZ:F1}..{maxZ:F1} (extent {extentX:F0} x {extentY:F0} x {extentZ:F0}).");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"Could not validate {file}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// build ob JBTN click as an asynchronous operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void BuildObJbtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancellationToken = new CancellationTokenSource();
                BuildObJbtn.IsEnabled = false;
                if (Zonelist.SelectedItem is Zones selectedItem)
                {
                    foreach (var zone in Dat.Dms._zones.Where(zone => zone.Name == selectedItem.Name))
                    {
                        if (zone.Path == "NILL") continue;
                        if (zone.Path == "") continue;
                        if (!File.Exists($@"{FFxiInstallPath}{zone.Path}"))
                        {
                            Log.AddDebugText(RtbDebug, $@"File not found,{FFxiInstallPath}{zone.Path}.");
                            continue;
                        }

                        await DumpZoneDat(zone.Id, zone.Name, zone.Path);
                        if ((bool)TPNamesCB.IsChecked)
                        {
                            foreach (KeyValuePair<int, string> tz in Tnames.zoneNames)
                            {
                                if (tz.Key == zone.Id)
                                {
                                    ZoneDat.Mzb.WriteObj(tz.Value);
                                }
                            }
                        }
                        else
                            ZoneDat.Mzb.WriteObj(IDonlyCb.IsChecked == true ? zone.Id.ToString() : zone.Name);

                        BuildObJbtn.IsEnabled = true;
                    }
                }
                else Log.AddDebugText(RtbDebug, "Please Select a zone to Dump.");
                BuildObJbtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
                BuildObJbtn.IsEnabled = true;
            }
        }

        /// <summary>
        /// Controls the c copy command can execute.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CtrlCCopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Controls the c copy command executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CtrlCCopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var lb = (ListBox)(sender);
            var selected = lb.SelectedItem;
            if (selected != null) Clipboard.SetText(selected.ToString() ?? string.Empty);
        }

        /// <summary>
        /// dump all object files as an asynchronous operation.
        /// </summary>
        /// <param name="run">if set to <c>true</c> [run].</param>
        private async Task DumpAllObjFilesAsync(bool run)
        {
            _cancellationToken = new CancellationTokenSource();
            switch (run)
            {
                case true:
                    {
                        if (Dat.Dms._zones.Count > 0)
                        {
                            var stopWatch = new Stopwatch();
                            SubTp.IsEnabled = false;
                            EntTp.IsEnabled = false;
                            stopWatch.Start();
                            foreach (var zone in Dat.Dms._zones)
                            {
                                await DumpZoneDat(zone.Id, zone.Name, zone.Path);

                                if ((bool)TPNamesCB.IsChecked)
                                {
                                    foreach (KeyValuePair<int, string> tz in Tnames.zoneNames)
                                    {
                                        if (tz.Key == zone.Id)
                                        {
                                            ZoneDat.Mzb.WriteObj(tz.Value);
                                        }
                                    }
                                }
                                else
                                    switch (IDonlyCb.IsChecked)
                                    {
                                        case true when ZoneDat.Mzb.WriteObj(zone.Id.ToString()):
                                        case false when ZoneDat.Mzb.WriteObj(zone.Name):
                                            continue;
                                    }
                            }
                            stopWatch.Stop();
                            var ts = stopWatch.Elapsed;
                            var elapsedTime = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}.{ts.Milliseconds / 10:00}";
                            Log.AddDebugText(RtbDebug, $@"Time taken to dump all collision obj files {elapsedTime}");
                            BuildAllObJbtn.Content = @"Build obj files for all zones.";
                            _dumpingMapDats = false;
                            SubTp.IsEnabled = true;
                            EntTp.IsEnabled = true;
                            SubRegion.IsEnabled = true;
                            Entity.IsEnabled = true;
                        }
                        else
                            Log.AddDebugText(RtbDebug, "Please click Load Zones, before you try and build obj files!.");
                        BuildAllObJbtn.Content = @"Build obj files for all zones.";
                        _dumpingMapDats = false;
                        SubTp.IsEnabled = true;
                        EntTp.IsEnabled = true;
                        SubRegion.IsEnabled = true;
                        Entity.IsEnabled = true;
                        break;
                    }
                case false:
                    {
                        _cancellationToken?.Cancel();
                        BuildAllObJbtn.Content = @"Build obj files for all zones.";
                        _dumpingMapDats = false;
                        SubTp.IsEnabled = true;
                        EntTp.IsEnabled = true;
                        SubRegion.IsEnabled = true;
                        Entity.IsEnabled = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Dumps the zone dat.
        /// </summary>
        /// <param name="zoneId">The zone identifier.</param>
        /// <param name="zoneName">Name of the zone.</param>
        /// <param name="datPath">The dat path.</param>
        private async Task DumpZoneDat(int zoneId, string zoneName, string datPath)
        {
            async Task Function()
            {
                try
                {
                    if (_saveEntityinfo && Dat != null)
                    {
                        var fileId = zoneId < 1000 || zoneId > 1299 ?
                            (zoneId < 256 ? zoneId + 6720 : zoneId + 86235) :
                            zoneId + 66911;

                        Dat.ParseDat(fileId);
                        Dat.Entity.DumpToXml(zoneId);
                    }
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    ZoneDat = new ParseZoneModelDat(Log, this, zoneId, datPath, FFxiInstallPath, _dumpSubregionInfoToXml);
                    var zoneDatPath = $@"{FFxiInstallPath}{datPath}";
                    if (zoneDatPath.Contains("ROM\\0\\28.DAT")) return;
                    Log.AddDebugText(RtbDebug, $@"Building an OBJ file using collision data for:  {zoneName} ID= {zoneId.ToString()}");
                    if (ZoneDat.LoadDat(zoneDatPath))
                    {
                        foreach (var sr in ZoneDat.Rid.SubRegions
                            .Where(sr =>
                                sr.RomPath != FFxiInstallPath
                                 && sr.RomPath != Zone0
                                 && sr.RomPath != Zone0bk
                                && sr.FileId != 0 && sr.RomPath != zoneDatPath &&
                                sr.RomPath != string.Empty).Where(sr => ZoneDat.LoadDat(sr.RomPath))) ;
                    }

                    await Task.Delay(100);
                    if (_saveSubRegioninfo)
                    {
                        ZoneDat.Rid.DumpToXml(zoneId);
                    }
                    stopWatch.Stop();
                    var ts = stopWatch.Elapsed;
                    var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                    Log.AddDebugText(RtbDebug, $@"Finished dumping {zoneName} collision data to {zoneName}.obj, Time taken {elapsedTime}");
                }
                catch (Exception ex)
                {
                    Log.LogFile(ex.ToString(), nameof(HomeView));
                    Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
                }
            }

            await Task.Run(Function, _cancellationToken.Token);
        }

        /// <summary>
        /// Handles the Click event of the EntityCb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EntityCb_Click(object sender, RoutedEventArgs e)
        {
            _saveEntityinfo = EntityCb.IsChecked == true;
        }

        /// <summary>
        /// Handles the GotFocus event of the EntTp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EntTp_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Equals(MyTabControl.SelectedItem, EntTp) || ZoneDat == null) return;
            if (!Dat.Entity._entities.Any()) return;
            var _itemSourceList = new CollectionViewSource() { Source = Dat.Entity._entities };

            var Itemlist = _itemSourceList.View;
            Entity.ItemsSource = Itemlist;
        }

        /// <summary>
        /// Handles the TextChanged event of the FfxiPathTb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void FfxiPathTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FfxiPathTb.Text == string.Empty) return;
            FFxiInstallPath = FfxiPathTb.Text;
            if (Dat != null)
            {
                Dat.ChangePath(FFxiInstallPath);
            }

            Log?.AddDebugText(RtbDebug, $@"FFxi installation path = {FFxiInstallPath}");
        }

        /// <summary>
        /// Handles the Click event of the LoadZonesBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LoadZonesBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadZonesBtn.IsEnabled = false;
                Zonelist.DataContext = null;
                Dat.ParseDat(55465);

                if (Dat.Dms._zones.Count > 0)
                {
                    Zonelist.Visibility = Visibility.Visible;
                    Zonelist.ItemsSource = Dat.Dms._zones;
                    Zonelist.AutoGenerateColumns = true;
                }
                Log.AddDebugText(RtbDebug, $@"{(Dat.Dms._zones.Count - 1).ToString()} Zones found.");
            }
            catch (Exception ex)
            {
                LoadZonesBtn.IsEnabled = true;
                Log.LogFile(ex.Message, Name);
                Log.AddDebugText(RtbDebug, $@"{ex.Message} > {Name}");
            }
        }

        /// <summary>
        /// Rights the click copy command can execute.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void RightClickCopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Rights the click copy command executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void RightClickCopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var selected = mi.DataContext;
            if (selected != null) Clipboard.SetText(selected.ToString() ?? string.Empty);
        }

        /// <summary>
        /// Handles the MouseEnter event of the SearchBoxTb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void SearchBoxTb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (SearchBoxTb.Text == "Search...")
                SearchBoxTb.Clear();
        }

        /// <summary>
        /// Handles the TextChanged event of the SearchBoxTb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void SearchBoxTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBoxTb.Text != "Search.." && SearchBoxTb.Text != string.Empty && Dat != null)
            {
                var itemSourceList = new CollectionViewSource() { Source = Dat.Dms._zones };

                var itemlist = itemSourceList.View;
                var name = new Predicate<object>(item => ((Zones)item).Name.ToLower().Contains(SearchBoxTb.Text.ToLower()));

                itemlist.Filter = name;

                Zonelist.ItemsSource = itemlist;
            }

            if (SearchBoxTb.Text != string.Empty || Dat == null) return;
            var _itemSourceList = new CollectionViewSource() { Source = Dat.Dms._zones };
            var Itemlist = _itemSourceList.View;
            Zonelist.ItemsSource = Itemlist;
        }

        /// <summary>
        /// Handles the MouseEnter event of the SearchBoxTb2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void SearchBoxTb2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (SearchBoxTb2.Text == "Search...")
                SearchBoxTb2.Clear();
        }

        /// <summary>
        /// Handles the TextChanged event of the SearchBoxTb2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void SearchBoxTb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBoxTb2.Text != "Search.." && SearchBoxTb2.Text != string.Empty && Dat != null)
            {
                var itemSourceList = new CollectionViewSource() { Source = Dat.Dms._zones };

                var itemlist = itemSourceList.View;

                var id = new Predicate<object>(item => ((Zones)item).Id.ToString().Contains(SearchBoxTb2.Text.ToLower()));

                itemlist.Filter = id;

                Zonelist.ItemsSource = itemlist;
            }

            if (SearchBoxTb2.Text != string.Empty || Dat == null) return;
            {
                var itemSourceList = new CollectionViewSource() { Source = Dat.Dms._zones };
                var itemlist = itemSourceList.View;
                Zonelist.ItemsSource = itemlist;
            }
        }

        /// <summary>
        /// Handles the Click event of the SelectOBJBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async void SelectOBJBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavBuildRunning)
            {
                Log.AddDebugText(RtbDebug, @"A NavMesh build is already running, wait for it to finish.");
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = $@"{Directory.GetCurrentDirectory()}\Map Collision obj files",
                Filter = "Wavefront OBJ (*.obj)|*.obj|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() != true) return;
            Log.AddDebugText(RtbDebug, $@"Obj File Selected = {openFileDialog.FileName}");

            var navPath = Path.Combine(Directory.GetCurrentDirectory(), "Dumped NavMeshes",
                $"{Path.GetFileNameWithoutExtension(openFileDialog.FileName)}.nav");
            if (File.Exists(navPath))
            {
                var result = MessageBox.Show(
                    $@"Are you sure you want to overwrite {Path.GetFileName(navPath)} ?",
                    "NavMesh", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (result != MessageBoxResult.Yes) return;
            }

            try
            {
                _isNavBuildRunning = true;
                SelectObjBtn.IsEnabled = false;
                AllObjBtn.IsEnabled = false;
                _buildMeshes = true;
                _cancellationToken = new CancellationTokenSource();
                await BuildNavMesh(openFileDialog.FileName);
            }
            finally
            {
                _buildMeshes = false;
                _isNavBuildRunning = false;
                SelectObjBtn.IsEnabled = true;
                AllObjBtn.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the SettingsBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureFfxiNav()) return;
            ApplyNavMeshSettingsFromUi();
        }

        /// <summary>
        /// Handles the Click event of the SubRegionCb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SubRegionCb_Click(object sender, RoutedEventArgs e)
        {
            _saveSubRegioninfo = SubRegionCb.IsChecked == true;
        }

        /// <summary>
        /// Handles the GotFocus event of the SubTp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SubTp_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Equals(MyTabControl.SelectedItem, SubTp) || ZoneDat == null) return;
            if (!ZoneDat.Rid.SubRegions.Any()) return;
            var _itemSourceList = new CollectionViewSource() { Source = ZoneDat.Rid.SubRegions };

            var Itemlist = _itemSourceList.View;
            SubRegion.ItemsSource = Itemlist;
        }

        /// <summary>
        /// Handles the GotFocus event of the TabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            EnsureFfxiNav();
        }

        /// <summary>
        /// Handles the Click event of the TPNamesCB control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TPNamesCB_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)TPNamesCB.IsChecked)
            {
                IDonlyCb.IsEnabled = false;
            }
            else
                IDonlyCb.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Click event of the IDonlyCb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void IDonlyCb_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)IDonlyCb.IsChecked)
            {
                TPNamesCB.IsEnabled = false;
            }
            else TPNamesCB.IsEnabled = true;
        }
    }
}