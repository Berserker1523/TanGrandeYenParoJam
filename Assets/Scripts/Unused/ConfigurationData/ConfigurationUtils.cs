using UnityEngine;

/// <summary>
/// Provides access to configuration data
/// </summary>
public static class ConfigurationUtils
{
    private static ConfigurationData configurationData;

    #region Properties

    /// <summary>
    /// Configuration1
    /// </summary>
    public static float Configuration1
    {
        get { return configurationData.Configuration1; }
    }

    /// <summary>
    /// Configuration2
    /// </summary>
    public static int Configuration2
    {
        get { return configurationData.Configuration2; }
    }

    #endregion

    /// <summary>
    /// Initializes the configuration utils
    /// </summary>
    public static void Initialize()
    {
        configurationData = new ConfigurationData();
    }
}
