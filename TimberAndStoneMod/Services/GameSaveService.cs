using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class GameSaveService
    {
        private static readonly GameSaveService instance = new GameSaveService();
        public static GameSaveService getInstance() { return instance; }
        
        private const String SAVE_PATH = "saves\\";
        private const String SAVE_BACKUP_PATH = SAVE_PATH + "backups\\";
        private const String SAVE_EXTENSION = ".tass.gz";

        public FileSystemWatcher FileSystemWatcher { get; private set; }

        private GameSaveService() 
        {
            this.FileSystemWatcher = new FileSystemWatcher(SAVE_PATH);
        }   

        public bool isGameSave(String fileName)
        {
            return fileName.EndsWith(SAVE_EXTENSION);
        }

        public IEnumerable<SaveGameInfo> getSavedGames()
        {
            foreach (String fileName in Directory.GetFiles(SAVE_PATH, "*" + SAVE_EXTENSION))
            {
                yield return new SaveGameInfo(fileName);
            }
        }

        public IEnumerable<SaveGameInfo> getBackups()
        {
            if (!Directory.Exists(SAVE_BACKUP_PATH))
            {
                Directory.CreateDirectory(SAVE_BACKUP_PATH);
            }

            foreach (String fileName in Directory.GetFiles(SAVE_BACKUP_PATH, "*" + SAVE_EXTENSION))
            {
                yield return new SaveGameInfo(fileName);
            }
        }

        public void createBackup(SaveGameInfo saveGameInfo)
        {
            if (!Directory.Exists(SAVE_BACKUP_PATH))
            {
                Directory.CreateDirectory(SAVE_BACKUP_PATH);
            }

            File.Copy(saveGameInfo.FilePath, SAVE_BACKUP_PATH + createBackupName(saveGameInfo));
        }

        public void restoreBackup(SaveGameInfo saveGameInfo)
        {
            String savePath = SAVE_PATH + getSettlementName(saveGameInfo) + SAVE_EXTENSION;

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            if (File.Exists(saveGameInfo.FilePath))
            {
                File.Copy(saveGameInfo.FilePath, savePath);
            }
        }

        public void deleteSave(SaveGameInfo saveGameInfo)
        {
            File.Delete(saveGameInfo.FilePath);
        }

        public SaveGameInfo getSaveGameInfoFromSettlementName(String settlementName)
        {
            String savePath = SAVE_PATH + settlementName + SAVE_EXTENSION;
            return File.Exists(savePath) ? new SaveGameInfo(savePath) : null;
        }

        private String createBackupName(SaveGameInfo saveGameInfo)
        {
            FileInfo fileInfo = new FileInfo(saveGameInfo.FilePath);
            return saveGameInfo.Name + "_" + fileInfo.LastWriteTime.ToString("MMddyyyyhhmmssfff") + SAVE_EXTENSION;
        }

        public String getSettlementName(SaveGameInfo saveGameInfo)
        {
            return new Regex("_[0-9]{17,}").Replace(saveGameInfo.Name, String.Empty);
        }

        public String getDate(SaveGameInfo saveGameInfo)
        {
            FileInfo fileInfo = new FileInfo(saveGameInfo.FilePath);
            return fileInfo.LastWriteTime.ToString("MM/d/yyyy hh:mm:ss tt");
        }

        public sealed class SaveGameInfo
        {
            public String FilePath { get; private set; }
            public String Name { get; private set; }

            public SaveGameInfo(String filePath)
            {
                this.FilePath = filePath;
                this.Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath));
            }
        }
    }
}
