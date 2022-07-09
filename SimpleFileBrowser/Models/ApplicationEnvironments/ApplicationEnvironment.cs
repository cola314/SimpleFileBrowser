using System;

namespace SimpleFileBrowser.Models.ApplicationEnvironments;

public class ApplicationEnvironment
{
    public string StoragePath { get; protected set; }
}

public class DevApplicationEnvironment : ApplicationEnvironment
{
    public DevApplicationEnvironment()
    {
        this.StoragePath = "C:\\SimpleFileBrowserData";
    }
}

public class ProductionApplicationEnvironment : ApplicationEnvironment
{
    public ProductionApplicationEnvironment()
    {
        this.StoragePath = "/data";
    }
}