using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.IO;

namespace NESTool.Building;

public static class EntitiesBuilding
{
    public static void Execute()
    {
        List<FileModelVO> entitiesVOs = ProjectFiles.GetModels<EntityModel>();

        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), $"entities.s");

        using StreamWriter outputFile = new(fullPath);

        outputFile.WriteLine("; This file is auto-generated!");
        outputFile.WriteLine("");

        foreach (FileModelVO entityVO in entitiesVOs)
        {
            if (entityVO.Model is not EntityModel entity)
                continue;

            outputFile.Write($"Entity_{entityVO.Name} = ${entity.EntityId:X2}{Environment.NewLine}");
        }
    }
}
