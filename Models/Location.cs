using System.ComponentModel.DataAnnotations;
namespace DashboardData.Models;
public class Location
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Building { get; set; }
    
    public ICollection<SensorData> Sensors { get; set; }=new List<SensorData>();
}