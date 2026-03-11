using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using InternProj.Data;

public class DatabaseWatcherService
{
    private readonly FileSystemWatcher _watcher;
    // This is the "Doorbell" other parts of your app can listen for
    public event EventHandler DatabaseChanged;

    public DatabaseWatcherService()
    {
        // 1. Get the raw path from your constant
        string fullPath = Constants.DatabasePath;

        // 2. IMPORTANT: Remove "Data Source=" if it exists
        if (fullPath.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            fullPath = fullPath.Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase);
        }

        // 3. Split it into Folder and Filename
        string folderPath = Path.GetDirectoryName(fullPath);
        string fileName = Path.GetFileName(fullPath);

        // 4. Initialize the watcher with the CLEAN folder path
        _watcher = new FileSystemWatcher(folderPath)
        {
            Filter = fileName,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += (s, e) => DatabaseChanged?.Invoke(this, EventArgs.Empty);
        _watcher.Renamed += (s, e) => DatabaseChanged?.Invoke(this, EventArgs.Empty);
    }
}
