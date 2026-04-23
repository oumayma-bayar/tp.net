using System.ComponentModel.DataAnnotations;
namespace DashboardData.Models;
public class Tag
{
    [Key]
    public int Id { get; set; }
    public string   Label { get; set; }
    public ICollection<SensorData> Sensors { get; set; }=new List<SensorData>(); 
    
}