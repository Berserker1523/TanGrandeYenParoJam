using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container for the configuration data
/// </summary>
public class ConfigurationData
{
    #region Fields

    private const string ConfigurationDataFileName = "ConfigurationData.csv";
    private Dictionary<ConfigurationDataValueName, float> values = new Dictionary<ConfigurationDataValueName, float>();

    #endregion

    #region Properties

    /// <summary>
    /// Configuration1
    /// </summary>
    public float Configuration1
    {
        get { return values[ConfigurationDataValueName.Configuration1]; }
    }

    /// <summary>
    /// Configuration2
    /// </summary>
    public int Configuration2
    {
        get { return (int)values[ConfigurationDataValueName.Configuration2]; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// Reads configuration data from a file. If the file
    /// read fails, the object contains default values for
    /// the configuration data
    /// </summary>
    public ConfigurationData()
    {
        StreamReader file = null;
        try
        {
            file = File.OpenText(Path.Combine(Application.streamingAssetsPath, ConfigurationDataFileName));

            string currentLine = file.ReadLine();
            while (currentLine != null)
            {
                string[] tokens = currentLine.Split(',');
                ConfigurationDataValueName valueName = 
                    (ConfigurationDataValueName)Enum.Parse(typeof(ConfigurationDataValueName), tokens[0]);
                values.Add(valueName, float.Parse(tokens[1]));
                currentLine = file.ReadLine();
            }

        }
        catch(Exception e)
        {
            Debug.LogError(e);
            SetDeafultValues();
        }
        finally
        {
            if(file != null)
            {
                file.Close();
            }
        }
    }

    #endregion

    #region Methods

    private void SetDeafultValues()
    {
        values.Clear();
        values.Add(ConfigurationDataValueName.Configuration1, 1);
        values.Add(ConfigurationDataValueName.Configuration2, 2);
    }

    #endregion
}
