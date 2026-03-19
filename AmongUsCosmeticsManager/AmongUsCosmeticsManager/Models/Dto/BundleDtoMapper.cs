using System.Collections.Generic;
using System.Linq;
using AmongUsCosmeticsManager.Models.Config;

namespace AmongUsCosmeticsManager.Models.Dto;

public static class BundleDtoMapper
{
    public static ProjectDto ToProjectDto(IEnumerable<CosmeticBundle> bundles)
    {
        var dto = new ProjectDto();
        foreach (var bundle in bundles)
            dto.Bundles.Add(ToDto(bundle));
        return dto;
    }

    public static List<CosmeticBundle> FromProjectDto(ProjectDto dto)
    {
        return dto.Bundles.Select(FromDto).ToList();
    }

    public static BundleDto ToDto(CosmeticBundle bundle)
    {
        var dto = new BundleDto
        {
            Name = bundle.Name,
            IsCompiled = bundle.IsCompiled,
            CompiledDateTicks = bundle.CompiledDate?.Ticks ?? 0
        };

        foreach (var section in bundle.Sections)
        {
            foreach (var item in section.Items)
            {
                var itemDto = new ItemDto
                {
                    TypeId = section.TypeDefinition.Id,
                    Name = item.Name,
                    Author = item.Author,
                    LastModifiedTicks = item.LastModified.Ticks
                };

                foreach (var prop in item.Properties)
                {
                    itemDto.Properties.Add(new PropertyDto
                    {
                        Id = prop.Definition.Id,
                        Value = prop.BoolValue
                    });
                }

                foreach (var res in item.Resources)
                {
                    itemDto.Resources.Add(new ResourceDto
                    {
                        Id = res.Definition.Id,
                        FileName = res.FileName,
                        Data = res.Data ?? []
                    });
                }

                foreach (var fl in item.FrameLists)
                {
                    var flDto = new FrameListDto { Id = fl.Definition.Id };
                    foreach (var frame in fl.Frames)
                        flDto.Frames.Add(frame);
                    itemDto.FrameLists.Add(flDto);
                }

                dto.Items.Add(itemDto);
            }
        }

        return dto;
    }

    public static CosmeticBundle FromDto(BundleDto dto)
    {
        var bundle = new CosmeticBundle(CosmeticTypeDefinition.All)
        {
            Name = dto.Name,
            IsCompiled = dto.IsCompiled,
            CompiledDate = dto.CompiledDateTicks > 0 ? new System.DateTime(dto.CompiledDateTicks) : null
        };

        foreach (var itemDto in dto.Items)
        {
            var section = bundle.GetSection(itemDto.TypeId);
            if (section == null) continue;

            var item = new CosmeticItem(section.TypeDefinition)
            {
                Name = itemDto.Name,
                Author = itemDto.Author,
                LastModified = new System.DateTime(itemDto.LastModifiedTicks)
            };

            foreach (var propDto in itemDto.Properties)
            {
                var pv = item.GetProperty(propDto.Id);
                if (pv != null) pv.BoolValue = propDto.Value;
            }

            foreach (var resDto in itemDto.Resources)
            {
                var rv = item.GetResource(resDto.Id);
                if (rv != null && resDto.Data.Length > 0)
                {
                    rv.FileName = resDto.FileName;
                    rv.Data = resDto.Data;
                }
            }

            foreach (var flDto in itemDto.FrameLists)
            {
                var fl = item.FrameLists.FirstOrDefault(f => f.Definition.Id == flDto.Id);
                if (fl != null)
                {
                    foreach (var frame in flDto.Frames)
                        fl.Frames.Add(frame);
                }
            }

            section.Items.Add(item);
        }

        return bundle;
    }
}
