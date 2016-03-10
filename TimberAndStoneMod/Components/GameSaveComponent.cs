using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class GameSaveComponent : GUIPluginComponent
    {
        private enum ViewState { LIST_SAVES, LIST_BACKUPS }

        private readonly ModSettings modSettings = ModSettings.getInstance();
        private readonly GameSaveService gameSaveService = GameSaveService.getInstance();

        private GUISection sectionScroll = new GUISection();

        private ViewState viewState;
        private GameSaveService.SaveGameInfo selectedSave;
        private List<GameSaveService.SaveGameInfo> gameSaves; 

        private FileSystemWatcher fileSystemWatcher;

        public override void OnStart()
        {
            title = "Game Save Manager";
            setWindowSize(260f + sectionMain.ControlMargin * 2, Screen.height / 2);
            setWindowPosition(0f, 0f);
            setUpdatesPerSecond(1);

            isVisibleInGame = false;
            isVisibleInMainMenu = true;
            isVisibleDuringGameOver = false;

            sectionMain.Direction = GUISection.FlowDirection.VERTICAL;
            sectionMain.Flow = GUISection.Overflow.HIDDEN;

            viewState = GameSaveComponent.ViewState.LIST_SAVES;
            gameSaves = gameSaveService.getSavedGames().ToList();

            fileSystemWatcher = gameSaveService.getFileSystemWatcher();
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;

            modSettings.loadSettings();
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (modSettings.isAutoBackupsEnabled)
            {
                if (gameSaveService.isGameSave(e.FullPath))
                {
                    GameSaveService.SaveGameInfo saveGameInfo = new GameSaveService.SaveGameInfo(e.FullPath);
                    gameSaveService.createBackup(saveGameInfo);
                    log("Backup saved for: " + saveGameInfo.Name);
                }
            }
        }
        
        public override void OnDraw(int windowId)
        {
            Window(this.title);

            sectionMain.Begin(0, WINDOW_TITLE_HEIGHT + sectionMain.ControlPadding, this.ParentContainer.width, this.ParentContainer.height);

            if (selectedSave == null && selectedSave == null)
            {
                switch (this.viewState)
                {
                    case ViewState.LIST_SAVES:
                        if (selectedSave == null)
                        {
                            if (sectionMain.CheckBox("Auto Backup", ref modSettings.isAutoBackupsEnabled))
                            {
                                modSettings.saveSettings();
                            }
                        }
                        sectionMain.LabelCentered("Saves");
                        break;

                    case ViewState.LIST_BACKUPS:
                        sectionMain.LabelCentered("Backups");
                        break;
                }
            }

            sectionScroll.Begin(0, sectionMain.ControlYPosition - sectionScroll.ControlMargin, this.ParentContainer.width, sectionMain.ControlHeight * 10);

            switch (this.viewState)
            {
                case ViewState.LIST_SAVES:
                    if (selectedSave == null)
                    {
                        foreach (GameSaveService.SaveGameInfo saveGameInfo in this.gameSaves)
                        {
                            if (sectionScroll.Button(saveGameInfo.Name))
                            {
                                selectedSave = saveGameInfo;
                            }
                        }                        
                    }
                    else
                    {
                        sectionScroll.LabelCentered(selectedSave.Name);
                        sectionScroll.LabelCentered(gameSaveService.getDate(selectedSave));

                        if (sectionScroll.Button("Backup"))
                        {
                            gameSaveService.createBackup(selectedSave);
                            selectedSave = null;
                        }

                        if (sectionScroll.Button("Delete"))
                        {
                            gameSaveService.deleteSave(selectedSave);
                            gameSaves = gameSaveService.getSavedGames().ToList();
                            selectedSave = null;
                        }
                    }

                    break;

                case ViewState.LIST_BACKUPS:
                    if (selectedSave == null)
                    {
                        foreach (GameSaveService.SaveGameInfo saveGameInfo in this.gameSaves)
                        {
                            if (sectionScroll.Button(gameSaveService.getSettlementName(saveGameInfo)))
                            {
                                selectedSave = saveGameInfo;
                            }

                            sectionScroll.LabelCentered(gameSaveService.getDate(saveGameInfo));
                        }
                    }
                    else
                    {
                        sectionScroll.LabelCentered(gameSaveService.getSettlementName(selectedSave));
                        sectionScroll.LabelCentered(gameSaveService.getDate(selectedSave));

                        if (sectionScroll.Button("Restore"))
                        {
                            gameSaveService.restoreBackup(selectedSave);
                            selectedSave = null;
                        }

                        if (sectionScroll.Button("Delete"))
                        {
                            gameSaveService.deleteSave(selectedSave);
                            gameSaves = gameSaveService.getBackups().ToList();
                            selectedSave = null;
                        }
                    }

                    break;
            }

            sectionScroll.End();            

            if (sectionScroll.hasChildren)
            {
                sectionMain.addSection(sectionScroll);
            }

            sectionMain.LabelLeft(String.Empty);

            switch (this.viewState)
            {
                case ViewState.LIST_SAVES:
                    if (selectedSave == null)
                    {
                        if (sectionMain.Button("Restore"))
                        {
                            this.viewState = ViewState.LIST_BACKUPS;
                            this.gameSaves = gameSaveService.getBackups().ToList();
                        }
                    }
                    else
                    {
                        if (sectionMain.Button("Cancel"))
                        {
                            selectedSave = null;
                        }
                    }
                    break;

                case ViewState.LIST_BACKUPS:
                    if (selectedSave == null)
                    {
                        if (sectionMain.Button("Cancel"))
                        {
                            this.viewState = ViewState.LIST_SAVES;
                            this.gameSaves = gameSaveService.getSavedGames().ToList();
                        }
                    }
                    else
                    {
                        if (sectionMain.Button("Cancel"))
                        {
                            selectedSave = null;
                        }
                    }
                    break;
            }

            sectionMain.End();

            GUI.DragWindow();

            if (sectionMain.hasChildren)
            {
                this.containerHeight = sectionMain.ControlYPosition + sectionMain.ControlMargin;
            }
            else this.containerHeight = sectionMain.ControlYPosition;
        }
    }
}
