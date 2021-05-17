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
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using FFXI_Navmesh_Builder.Common;
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
                        AllObjBtn.Content = @"Stop building NavMeshes.";
                        _buildMeshes = true;
                        _cancellationToken = new CancellationTokenSource();
                        var path = $@"{ Directory.GetCurrentDirectory()}\Map Collision obj files";
                        var fileCount = Directory.GetFiles(path, "*.obj", SearchOption.AllDirectories).Length;
                        Log.AddDebugText(RtbDebug, $@"{fileCount.ToString()}.obj files fould in Map Collision obj folder");
                        foreach (var file in Directory.EnumerateFiles(string.Format(path, "*.obj")))
                        {
                            if (!_buildMeshes) continue;
                            var _fullPath = Path.GetFileName(file);
                            var _name = _fullPath.Substring(0, _fullPath.LastIndexOf(".", StringComparison.Ordinal) + 1);
                            Log.AddDebugText(RtbDebug, $@"Building NavMesh for {_name} please wait!...");
                            if (File.Exists($@"{Directory.GetCurrentDirectory()}\Dumped NavMeshes\\{_name}nav"))
                            {
                                var messageBoxText = $@"Are you sure you want to overwrite {_name}.nav ?";
                                var caption = "NavMesh";
                                var button = MessageBoxButton.YesNoCancel;
                                var icon = MessageBoxImage.Warning;
                                var result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                                switch (result)
                                {
                                    case MessageBoxResult.Cancel:
                                        return;
                                        break;

                                    case MessageBoxResult.Yes:
                                        _cancellationToken = new CancellationTokenSource();
                                        await BuildNavMesh(file);
                                        break;

                                    case MessageBoxResult.No:
                                        break;

                                    case MessageBoxResult.None:
                                        break;

                                    case MessageBoxResult.OK:
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            if (!File.Exists($@"{Directory.GetCurrentDirectory()}\Dumped NavMeshes\\{_name}nav"))
                            {
                                _cancellationToken = new CancellationTokenSource();
                                await BuildNavMesh(file);
                            }
                        }
                        AllObjBtn.Content = @"Build NavMeshes for all .OBJ files.";
                        _buildMeshes = false;
                        _cancellationToken?.Cancel();

                        return;
                        break;

                    case "Stop building NavMeshes.":
                        _buildMeshes = false;
                        _cancellationToken?.Cancel();
                        AllObjBtn.Content = @"Build NavMeshes for all .OBJ files.";
                        return;
                        break;
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
        /// Builds the nav mesh.
        /// </summary>
        /// <param name="file">The file.</param>
        private async Task BuildNavMesh(string file)
        {
            async Task Function()
            {
                try
                {
                    if (!_buildMeshes)
                    {
                        _cancellationToken?.Cancel();
                        return;
                    }
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    await _ffxiNav.Dump_NavMesh(file);
                    stopWatch.Stop();
                    var elapsed = stopWatch.Elapsed;
                    var elapsedTime = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds / 10:00}";
                    Log.AddDebugText(RtbDebug, $@"Time Taken to Build NavMesh = {elapsedTime}");
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
                        if (!File.Exists($@"{FFxiInstallPath}{zone.Path}")) continue;

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
                    if (_saveEntityinfo)
                    {
                        if (Dat != null)
                        {
                            if (zoneId < 1000 || zoneId > 1299)
                            {
                                var fileId = zoneId < 256 ? zoneId + 6720 : zoneId + 86235;
                                Dat.ParseDat(fileId);
                            }
                            else
                            {
                                var fileId = zoneId + 66911;
                                Dat.ParseDat(fileId);
                            }
                        }

                        if (Dat != null) Dat.Entity.DumpToXml(zoneId);
                    }
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    ZoneDat = new ParseZoneModelDat(Log, this, zoneId, datPath, FFxiInstallPath, _dumpSubregionInfoToXml);
                    var zoneDatPath = $@"{FFxiInstallPath}{datPath}";
                    Log.AddDebugText(RtbDebug, $@"Building an OBJ file using collision data for:  {zoneName} ID= {zoneId.ToString()}");
                    if (ZoneDat.LoadDat(zoneDatPath))
                    {
                        foreach (var sr in ZoneDat.Rid.SubRegions
                            .Where(sr =>
                                sr.RomPath != FFxiInstallPath && sr.FileId != 0 && sr.RomPath != zoneDatPath &&
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
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = $@"{Directory.GetCurrentDirectory()}\Map Collision obj files";
            if (openFileDialog.ShowDialog() != true) return;
            Log.AddDebugText(RtbDebug, $@"Obj File Selected = {openFileDialog.FileName}");
            _buildMeshes = true;

            var _fullPath = Path.GetFileName(openFileDialog.FileName);
            var _name = _fullPath.Substring(0, _fullPath.LastIndexOf(".", StringComparison.Ordinal) + 1);
            if (File.Exists($@"{Directory.GetCurrentDirectory()}\Dumped NavMeshes\\{_name}nav"))
            {
                var messageBoxText = $@"Are you sure you want to overwrite {_name}.nav ?";
                var caption = "NavMesh";
                var button = MessageBoxButton.YesNoCancel;
                var icon = MessageBoxImage.Warning;
                MessageBoxResult result;
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        break;

                    case MessageBoxResult.Yes:
                        _cancellationToken = new CancellationTokenSource();
                        await BuildNavMesh(openFileDialog.FileName);
                        break;

                    case MessageBoxResult.No:
                        break;

                    case MessageBoxResult.None:
                        break;

                    case MessageBoxResult.OK:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (!File.Exists($@"{Directory.GetCurrentDirectory()}\Dumped NavMeshes\\{_name}nav"))
            {
                _cancellationToken = new CancellationTokenSource();
                await BuildNavMesh(openFileDialog.FileName);
            }
            _buildMeshes = false;
        }

        /// <summary>
        /// Handles the Click event of the SettingsBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ffxiNav.ChangeNavMeshSettings(Convert.ToDouble(cellSizeValue.Value), Convert.ToDouble(cellHeightValue.Value)
                    , Convert.ToDouble(agentHeightValue.Value), Convert.ToDouble(agentRadiusValue.Value), Convert.ToDouble(maxClimbValue.Value)
                    , Convert.ToDouble(maxSlopeValue.Value), Convert.ToDouble(tileSizeValue.Value), Convert.ToDouble(regionMinSizeValue.Value),
                    Convert.ToDouble(regionMergeSizeValue.Value), Convert.ToDouble(edgeMaxLenValue.Value), Convert.ToDouble(edgeMaxErrorValue.Value), Convert.ToDouble(vertsPerPolyValue.Value)
                    , Convert.ToDouble(detailSampleDistanceValue.Value), Convert.ToDouble(detailSampleMaxErrorValue.Value),
                    dllDebugMode.IsChecked != null && ((bool)dllDebugMode.IsChecked));
                Log.AddDebugText(RtbDebug, "NavMesh Settings changed.");
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(HomeView));
                Log.AddDebugText(RtbDebug, $@"{ex} > {nameof(HomeView)}");
            }
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
            if (_ffxiNav == null && File.Exists($@"{Directory.GetCurrentDirectory()}\\FFXINAV.dll"))
            {
                _ffxiNav = new Ffxinav();
            }
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
                IDonlyCb.IsEnabled = true ;

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