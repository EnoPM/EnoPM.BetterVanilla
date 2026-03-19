using System.Collections.Generic;
using System.IO;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Dto;
using BSerializer;

namespace AmongUsCosmeticsManager.Services;

public class ProjectService
{
    public string? ProjectFilePath { get; private set; }
    public bool HasProject => ProjectFilePath != null;

    public void SetProjectFile(string filePath)
    {
        ProjectFilePath = filePath;
    }

    public List<CosmeticBundle> Load()
    {
        if (ProjectFilePath == null || !File.Exists(ProjectFilePath)) return [];

        var data = File.ReadAllBytes(ProjectFilePath);
        var dto = Serializer.Deserialize<ProjectDto>(data);
        return BundleDtoMapper.FromProjectDto(dto);
    }

    public void Save(IEnumerable<CosmeticBundle> bundles)
    {
        if (ProjectFilePath == null) return;

        var dto = BundleDtoMapper.ToProjectDto(bundles);
        var data = Serializer.Serialize(dto);
        File.WriteAllBytes(ProjectFilePath, data);
    }
}
