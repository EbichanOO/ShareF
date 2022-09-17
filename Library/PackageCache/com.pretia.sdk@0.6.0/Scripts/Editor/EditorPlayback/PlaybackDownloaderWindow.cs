using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using PretiaArCloud.RecordingPlayback.FileTransfer.Models;

namespace PretiaArCloudEditor.RecordingPlayback
{
    /// <summary>
    /// Displays a window as user interface to download content.
    /// </summary>
    public class PlaybackDownloaderWindow : EditorWindow
    {
        private const string EDITOR_PREFS_IP = "EPREFS_ARRP_IP";
        private const string EDITOR_PREFS_PORT = "EPREFS_ARRP_PORT";

#region Fields

        /// <summary>
        /// Short cut to the playback settings where the IP and the port are
        /// written.
        /// </summary>
        private static PlaybackAndRecordingSettings Settings => 
            PlaybackAndRecordingSettings.CurrentSettings;

        /// <summary>
        /// Time that should update.
        /// </summary>
        private const float UPDATE_TIME = 30;
        private string ip = "192.168.1.1";
        private int port = 16000;

        #endregion

        #region Properties

        /// <summary>
        /// Flag that indicates whether the list is being updated.
        /// </summary>
        private bool IsUpdatingList { get; set; }
        
        /// <summary>
        /// Flag that indicates whether a file is being downloaded.
        /// </summary>
        private bool IsDownloading { get; set; }
        
        /// <summary>
        /// The current downloading progress.
        /// </summary>
        private float DownloadingProgress { get; set; }
        
        /// <summary>
        /// The path of the file to download.
        /// </summary>
        private string DestinationWithOutExtension { get; set; }
        
        /// <summary>
        /// Position of the scroller of the scroll view of the list of
        /// recordings.
        /// </summary>
        private Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Downloader to use that this interface is for.
        /// </summary>
        private PlaybackDownloader Downloader { get; } = PretiaArCloudEditor.Services.PlaybackDownloader;

        /// <summary>
        /// List of files that the device has ready to be downloaded.
        /// </summary>
        private static List<string> Files { get; } = new List<string>();

        /// <summary>
        /// time in which started to took the time for updating.
        /// </summary>
        private System.DateTime StartTime { get; set; }

        /// <summary>
        /// Flag to show the no device connected warning.
        /// </summary>
        private bool ShowNoDeviceConnectedWarning { get; set; }

#endregion

#region Methods

        /// <summary>
        /// Shows the the window of Playback Downloader.
        /// </summary>
        [MenuItem("Pretia/Playback Downloader")]
        internal static void ShowWindow()
        {
            var window = GetWindow<PlaybackDownloaderWindow>();
            window.titleContent = UI.WindowsTitle;
            window.minSize = new Vector2(340, 320);
            window.Show();
        }

        /// <summary>
        /// Called on enable.
        /// </summary>
        private void OnEnable()
        {
            Downloader.OnError += OnError;
            StartTime = System.DateTime.Now;
        }

        /// <summary>
        /// Called on disable.
        /// </summary>
        private void OnDisable()
        {
            Downloader.OnError -= OnError;
            Downloader.Dispose();
        }

        /// <summary>
        /// Called by the editor to draw the window.
        /// </summary>
        private void OnGUI()
        {
            // Displays the IP and the port.
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            if (EditorPrefs.HasKey(EDITOR_PREFS_IP))
            {
                ip = EditorPrefs.GetString(EDITOR_PREFS_IP);
            }

            if (EditorPrefs.HasKey(EDITOR_PREFS_PORT))
            {
                port = EditorPrefs.GetInt(EDITOR_PREFS_PORT);
            }

            ip = EditorGUILayout.TextField(UI.IP, ip);
            port = EditorGUILayout.IntField(UI.Port, port);

            EditorGUILayout.EndVertical();
            
            if (!PlaybackAndRecordingSettingsRegister.IsValidIp(ip))
            {
                EditorGUILayout.HelpBox(UI.WrongIP, MessageType.Error);
                return;
            }

            EditorPrefs.SetString(EDITOR_PREFS_IP, ip);
            EditorPrefs.SetInt(EDITOR_PREFS_PORT, port);

            // Assigns the IP and port to the downloader.
            Downloader.IP = ip;
            Downloader.Port = port;
            
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            DrawUpdateButton();
            DrawDownloadableFiles();
            EditorGUILayout.EndVertical();
            
            if (ShowNoDeviceConnectedWarning)
            {
                EditorGUILayout.HelpBox(UI.DeviceWarning);
            }
        }

        /// <summary>
        /// Called each frame.
        /// </summary>
        private async void Update()
        {
            AutoUpdate();
            if (!(IsDownloading || IsUpdatingList)) { return; }
            Repaint();

            if (DownloadingProgress < 1 || string.IsNullOrEmpty(DestinationWithOutExtension))
            {
                return;
            }

            var destinationWithOutExtension = DestinationWithOutExtension;
            IsDownloading = false;
            DestinationWithOutExtension = string.Empty;
            var dataFileDestinationFullPath = 
                destinationWithOutExtension 
                + "." + PlaybackImporter.META_FILE_EXT;

            // Waits for the downloader to finish to dispose the stream.
            await Task.Delay(1000);
            await PlaybackImporter.Import(dataFileDestinationFullPath);
        }

        /// <summary>
        /// Updates the list automatically.
        /// </summary>
        private void AutoUpdate()
        {
            var now = System.DateTime.Now;
            if (!((now - StartTime).TotalSeconds >= UPDATE_TIME))
            {
                return;
            }

            StartTime = now;
            if (IsDownloading || IsUpdatingList)
            {
                return;
            }
            
            try
            {
                IsUpdatingList = true;
                new Thread(() => Downloader.ListFiles(OnListUpdated))
                    {IsBackground = true}.Start();
            }
            catch (System.Exception)
            {
                //
            }
        }

        /// <summary>
        /// Draws the button to update the list.
        /// </summary>
        private void DrawUpdateButton()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            
            var label = IsDownloading 
                ? UI.GetDownloadingLabel(DownloadingProgress)
                : IsUpdatingList 
                    ? UI.Updating 
                    : UI.UpdateList;
            
            GUI.enabled = !(IsUpdatingList || IsDownloading);
            
            if (GUILayout.Button(
                label, 
                EditorStyles.miniButton, 
                GUILayout.Width(150)))
            {
                IsUpdatingList = true;
                new Thread(() => Downloader.ListFiles(OnListUpdated)) 
                    { IsBackground = true }.Start();
            }

            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Method called after the list is updated.
        /// </summary>
        /// <param name="updatedList"></param>
        private void OnListUpdated(ListFilesResponse updatedList)
        {
            ShowNoDeviceConnectedWarning = false;

            var completeList = updatedList.fileDetails;
            Files.Clear();

            for (var i = 0; i < completeList.Count; i++)
            {
                var fileDetail = completeList[i];
                var relativePah = fileDetail.relativePath;
                var fileName = Path.GetFileNameWithoutExtension(relativePah);
                if (Files.Exists(filePath => filePath.Contains(fileName)))
                {
                    continue;
                }

                Files.Add(relativePah);
            }
            
            IsUpdatingList = false; 
        }

        /// <summary>
        /// Draws the list of downloadable files.
        /// </summary>
        private void DrawDownloadableFiles()
        {
            if (IsDownloading) { return; }
            
            // Displays a message for empty list.
            if (Files == null || Files.Count == 0)
            {
                EditorGUILayout.LabelField(
                    UI.EmptyList, 
                    EditorStyles.centeredGreyMiniLabel
                );
                return;
            }
            
            var label = IsUpdatingList ? UI.Updating : UI.Download;

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

            // Displays the list of downloadable content.
            for (var i = 0; i < Files.Count; i++)
            {
                var relativePath = Files[i];
                var fileName = Path.GetFileNameWithoutExtension(relativePath);
                try
                {
                    EditorGUILayout.BeginHorizontal();
                }
                catch (Exception)
                {
                    continue;
                }
                
                // Displays the name.
                EditorGUILayout.LabelField(fileName);
                
                // Display a button to download the content.
                if (GUILayout.Button(label, EditorStyles.miniButton))
                {
                    // Get the destination path.
                    string dest;
                    if (string.IsNullOrEmpty(Settings.DownloadPath))
                    {
                        dest = EditorUtility.OpenFolderPanel(
                            UI.OpenFolderTitle,
                            Application.dataPath,
                            string.Empty
                        );
                        Settings.DownloadPath = dest;
                    }
                    else
                    {
                        dest = Settings.DownloadPath;
                        var splitted = dest.Split(Path.PathSeparator);
                        var directory = string.Empty;
                        for (var j = 0; j < splitted.Length; j++)
                        {
                            directory = j == 0 
                                ? splitted[0]
                                : Path.Combine(directory, splitted[j]);
                            
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                        }
                    }
                      
                    
                    // Continue if was canceled.
                    if (!string.IsNullOrEmpty(dest))
                    {
                        IsDownloading = true;
                        DownloadingProgress = 0;
                        DestinationWithOutExtension = Path.Combine(dest, fileName);
                        if (File.Exists(DestinationWithOutExtension)) { File.Delete(DestinationWithOutExtension); }
                        
                        new Thread(
                            () => DownloadFiles(
                                relativePath,
                                DestinationWithOutExtension,
                                OnDownLoadProgressUpdated
                            )
                        ) {IsBackground = true}.Start();
                    }
                }

                if (GUILayout.Button(UI.TrashIcon, EditorStyles.miniButton))
                {
                    new Thread(
                        () => RemoveFiles(
                            relativePath,
                            OnListUpdated
                        )
                    ) { IsBackground = true}.Start();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DownloadFiles(string relativePath, string destination, System.Action<float> onDownload)
        {
            var extension = Path.GetExtension(relativePath);
            var videoFileExtension = "." + PlaybackImporter.MP4_FILE_EXT;
            var metaFileExtension = "." + PlaybackImporter.META_FILE_EXT;
            var videoFilePath = relativePath.Replace(extension, videoFileExtension);
            var metaFilePath = relativePath.Replace(extension, metaFileExtension);

            Downloader.DownloadFile(videoFilePath, destination + videoFileExtension, onDownload);
            Downloader.DownloadFile(metaFilePath, destination + metaFileExtension, onDownload);
        }
        
        private void RemoveFiles(string relativePath, System.Action<ListFilesResponse> onListUpdated)
        {
            var extension = Path.GetExtension(relativePath);
            var videoFileExtension = "." + PlaybackImporter.MP4_FILE_EXT;
            var metaFileExtension = "." + PlaybackImporter.META_FILE_EXT;
            var videoFilePath = relativePath.Replace(extension, videoFileExtension);
            var metaFilePath = relativePath.Replace(extension, metaFileExtension);

            Downloader.RemoveFile(videoFilePath, onListUpdated);
            Downloader.RemoveFile(metaFilePath, onListUpdated);
        }

        /// <summary>
        /// Method called to update the progress of the downloading process.
        /// </summary>
        /// <param name="progress">Current progress.</param>
        private void OnDownLoadProgressUpdated(float progress)
        {
            DownloadingProgress = progress;
            if (progress < 1) { return; }
            DownloadingProgress = 1;
        }
        
        /// <summary>
        /// Called on error.
        /// </summary>
        /// <param name="e">Exception of the error.</param>
        private void OnError(System.Exception e)
        {
            IsUpdatingList = false;
            IsDownloading = false;
            DownloadingProgress = 0;

            if (!string.IsNullOrEmpty(DestinationWithOutExtension) && File.Exists(DestinationWithOutExtension))
            {
                File.Delete(DestinationWithOutExtension);
            }

            DestinationWithOutExtension = string.Empty;
            
            if (e is System.Net.Sockets.SocketException)
            {
                ShowNoDeviceConnectedWarning = true;
            }
            else
            {
                Debug.LogError(e);
            }
        }
        
#endregion

#region Nested

        /// <summary>
        /// Class with the content to display on the window.
        /// </summary>
        private static class UI
        {
            public const string OpenFolderTitle = "Select Destination";
            public const string WrongIP = "Please write a valid IP.";
            public static readonly GUIContent WindowsTitle =
                new GUIContent(
                    "Playback Downloader"
                );
            public static readonly GUIContent IP = 
                new GUIContent(
                    "IP", 
                    "IP of the device where to download the data from."
                );
            public static readonly GUIContent Port =
                new GUIContent(
                    "Port",
                    "Port where to connect to download files."
                );
            public static readonly GUIContent UpdateList =
                new GUIContent(
                    "Update Playback List",
                    "Updates the list of available recordings for download."
                );
            public static readonly GUIContent Updating =
                new GUIContent(
                    "Updating...",
                    "The list of available recordings is loading. Please wait."
                );
            private static readonly GUIContent Downloading =
                new GUIContent(
                    "Downloading",
                    "Currently downloading a record. Please wait."
                );
            public static readonly GUIContent EmptyList =
                new GUIContent(
                    "There are any recorded playbacks to download.",
                    "Please open the recorder to find available recordings."
                );
            public static readonly GUIContent Download =
                new GUIContent(
                    "Download",
                    "Download this record from the device."
                );

            public static readonly GUIContent DeviceWarning = new GUIContent(
                "Verify the IP, port and that the device is connected."
            );

            public static readonly GUIContent TrashIcon =
                EditorGUIUtility.IconContent(
                    "TreeEditor.Trash",
                    "Remove File."
                );

            public static GUIContent GetDownloadingLabel(float progress)
            {
                var content = new GUIContent(
                    Downloading.text,
                    Downloading.image, 
                    Downloading.tooltip
                );
                
                progress = Mathf.RoundToInt(progress * 100);
                content.text = content.text + " " + progress + "%";
                return content;
            }
        }
        
#endregion
        
    }
}