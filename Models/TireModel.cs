using System.Xml.Linq;

namespace TireSearchMVC.Models;

public class TireModel
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? DetailDescription { get; set; }
    public string? Dimension { get; set; }
    public string? PatternDescription { get; set; }
    public string? PatternString { get; set; }
    public string? TechnicalGrouping { get; set; }
    public string? LoadIndex { get; set; }
    public string? SpeedRating { get; set; }
    public string? ModifiedTime { get; set; }
    public string? Brand { get; set; }
    public string? Season { get; set; }
    public string? Size { get; set; }
    
    // Additional detailed properties for popup
    public string? ActivePassiveClassification { get; set; }
    public string? ActivePassiveDescription { get; set; }
    public string? BrandCode { get; set; }
    public string? NoiseEmission { get; set; }
    public string? NoiseEmissionLevel { get; set; }
    public string? PcmParentGroupCode { get; set; }
    public string? PcmParentGroupDescription { get; set; }
    public string? PcmServiceCode { get; set; }
    public string? PcmServiceDescription { get; set; }
    public string? PositionCode { get; set; }
    public string? PositionDescription { get; set; }
    public string? RollingResistance { get; set; }
    public string? SeasonCode { get; set; }
    public string? SeasonUrlCode { get; set; }
    public string? SizeCode { get; set; }
    public string? SpecialUsageCode { get; set; }
    public string? SpecialUsageDescription { get; set; }
    public string? VehicleClass { get; set; }
    public string? VehicleUse { get; set; }
    public string? VehicleUseDescription { get; set; }
    public string? WetGrip { get; set; }
    public string? AspectRatio { get; set; }
    public string? RimDiameter { get; set; }
    public string? SectionWidth { get; set; }
    public string? ThreadPatternCode { get; set; }
    public string? ThreadPatternName { get; set; }
    public string? ThreadPatternDetailDescription { get; set; }
    public string? ThreadPatternSummaryDescription { get; set; }
    public string? TsegmentCode { get; set; }
    public string? TsegmentDescription { get; set; }
    public string? UsageSegmentCode { get; set; }

    public static TireModel FromXml(XElement element)
    {
        return new TireModel
        {
            Code = element.Attribute("code")?.Value,
            Name = element.Element("name")?.Value,
            DetailDescription = element.Element("detailDescription")?.Value,
            Dimension = element.Element("dimension")?.Value,
            PatternDescription = element.Element("patternDescription")?.Value,
            PatternString = element.Element("patternString")?.Value,
            TechnicalGrouping = element.Element("technicalGrouping")?.Value,
            LoadIndex = element.Element("loadIndex")?.Value,
            SpeedRating = element.Element("speedRating")?.Value,
            ModifiedTime = element.Element("modifiedtime")?.Value,
            Brand = element.Element("brand")?.Element("description")?.Value,
            Season = element.Element("season")?.Element("description")?.Value,
            Size = element.Element("size")?.Element("description")?.Value,
            
            // Additional detailed properties
            ActivePassiveClassification = element.Element("activePassiveClassification")?.Attribute("code")?.Value,
            ActivePassiveDescription = element.Element("activePassiveClassification")?.Element("description")?.Value,
            BrandCode = element.Element("brand")?.Attribute("code")?.Value,
            NoiseEmission = element.Element("noiseEmission")?.Value?.Trim(),
            NoiseEmissionLevel = element.Element("noiseEmissionLevel")?.Value,
            PcmParentGroupCode = element.Element("pcmParentGroup")?.Attribute("code")?.Value,
            PcmParentGroupDescription = element.Element("pcmParentGroup")?.Element("description")?.Value,
            PcmServiceCode = element.Element("pcmService")?.Attribute("code")?.Value,
            PcmServiceDescription = element.Element("pcmService")?.Element("description")?.Value,
            PositionCode = element.Element("position")?.Attribute("code")?.Value,
            PositionDescription = element.Element("position")?.Element("description")?.Value,
            RollingResistance = element.Element("rollingResistance")?.Value,
            SeasonCode = element.Element("season")?.Attribute("code")?.Value,
            SeasonUrlCode = element.Element("season")?.Element("urlCode")?.Value,
            SizeCode = element.Element("size")?.Attribute("code")?.Value,
            SpecialUsageCode = element.Element("specialUsage")?.Attribute("code")?.Value,
            SpecialUsageDescription = element.Element("specialUsage")?.Element("description")?.Value,
            VehicleClass = element.Element("vehicleClass")?.Value,
            VehicleUse = element.Element("vehicleUse")?.Value,
            VehicleUseDescription = element.Element("vehicleUseDescription")?.Value,
            WetGrip = element.Element("wetGrip")?.Value,
            AspectRatio = element.Element("aspectRatio")?.Value,
            RimDiameter = element.Element("rimDiameter")?.Value,
            SectionWidth = element.Element("sectionWidth")?.Value,
            ThreadPatternCode = element.Element("threadPattern")?.Attribute("code")?.Value,
            ThreadPatternName = element.Element("threadPattern")?.Element("name")?.Value,
            ThreadPatternDetailDescription = element.Element("threadPattern")?.Element("detailDescription")?.Value,
            ThreadPatternSummaryDescription = element.Element("threadPattern")?.Element("summaryDescription")?.Value,
            TsegmentCode = element.Element("tsegment")?.Attribute("code")?.Value,
            TsegmentDescription = element.Element("tsegment")?.Element("description")?.Value,
            UsageSegmentCode = element.Element("usageSegment")?.Attribute("code")?.Value
        };
    }
}