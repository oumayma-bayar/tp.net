using System.ComponentModel.DataAnnotations;

namespace DashboardData.Models;

public class SensorData
{
    [Key]
    public int Id { get; set; }
     [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom doit faire entre 3 et 50 caractères.")]
    public string Name{get;set;}
    public string Type {get;set;}="Temperature";
    public DateTime LastUpdate {get;set;}=DateTime.Now;

    [Range(-50.0, 150.0, ErrorMessage = "La valeur doit être comprise entre -50.0 et 150.0")]
    public double Value {get;set;}
    
[Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un lieu valide.")]
    public int LocationId { get; set; }
    public Location Location { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public ICollection<SensorValueHistory> Values { get; set; } = new List<SensorValueHistory>();
}
public class LocationStat
{
    public string LocationName { get; set; }
    public double AverageValue { get; set; }
}
public class LocationCountStat
{
    public string LocationName { get; set; }
    public int Count { get; set; }
}