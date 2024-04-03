﻿using System;
using System.Reflection;

namespace ArchitectureLibrary.Utils;

public static class Global
{
    /// <summary>
    /// Helper method for generating a "pack://" URI for a given relative file based on the
    /// assembly that this class is in.
    /// </summary>
    public static Uri MakePackUri(string relativeFile)
    {
        string uriString = $"pack://application:,,,/{AssemblyShortName};component/{relativeFile}";
        return new Uri(uriString);
    }

    private static string AssemblyShortName
    {
        get
        {
            if (_assemblyShortName == null)
            {
                Assembly a = typeof(Global).Assembly;

                // Pull out the short name.
                _assemblyShortName = a.ToString().Split(',')[0];
            }

            return _assemblyShortName;
        }
    }

    private static string _assemblyShortName;
}