using DashboardData.Models;
namespace DashboardData.Services;
using DashboardData.Data;
using Microsoft.EntityFrameworkCore;

public class SensorService : ISensorService
{
  

    private readonly AppDbContext _context;
    public SensorService(AppDbContext context)
    {
        _context = context;
    }
    
      public async Task ReloadSensorsAsync(SensorData sensor)
    {
        await _context.Entry(sensor).ReloadAsync();
    }

    private List<SensorData> _sensors = new List<SensorData>
    {
        new SensorData { Name = "Temp_Salon", Value = 22.5 },
        new SensorData { Name = "Hum_cuisine", Value = 45.0 },
        new SensorData { Name = "CO2_Bureau", Value =800 },
        new SensorData { Name = "Temp_Bureau", Value =24.0 },
        new SensorData { Name = "Temp_Exit", Value =12.0 }
    };

    

    public async Task<List<SensorData>> GetSensorsAsync()
    {
        return await _context.Sensors
            .Include(s => s.Location)
            .ToListAsync(); 
    }
    public void AddSensor(SensorData sensor)
    {
        _sensors.Add(sensor);
    }
    public async Task AddSensorAsync(SensorData sensor)
    {
        _context.Sensors.Add(sensor);
        await _context.SaveChangesAsync();
    }

    public async Task<List<SensorData>> GetCriticalSensorAsync(double threshold)
    {
        return await _context.Sensors
            .Include(s => s.Location)
            .Where(s => s.Value > threshold)
            .OrderByDescending(s => s.Value)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        if (!await _context.Sensors.AnyAsync())
            return 0;
        return await _context.Sensors.CountAsync();
    }
    public async Task<double> GetAverageValueAsync()
    {
        if (!await _context.Sensors.AnyAsync())
            return 0;
        return await _context.Sensors.AverageAsync(s => s.Value);
    }
    public async Task<double> GetMaxValueAsync()
    {
        if (!await _context.Sensors.AnyAsync())
            return 0;
        return await _context.Sensors.MaxAsync(s => s.Value);
    }

    public async Task<List<Location>> GetLocationsAsync()
    {
        return await _context.Locations.ToListAsync();
    }
    public async Task<SensorData?> GetSensorByIdAsync(int id)
    {
        return await _context.Sensors.FindAsync(id);
            
            
    }
    public async Task UpdateSensorAsync(SensorData sensor)
    {
        sensor.LastUpdate=DateTime.Now;

        sensor.Values.Add(new SensorValueHistory
        {
            Value = sensor.Value,
            Timestamp = DateTime.Now
        });

        _context.Sensors.Update(sensor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSensorAsync(int id)
    {
        var sensor = await _context.Sensors.FindAsync(id);
        if (sensor != null)
        {
            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();
        }
    }

   public async Task<List<LocationStat>> GetAverageValueByLocationAsync()
{
    // EF Core traduit ceci en : SELECT Location, AVG(Value) FROM Sensors GROUP BY Location
    return await _context.Sensors
        .Include(s => s.Location)
        .GroupBy(s => s.Location.Name)
        .Select(g => new LocationStat 
        { 
            LocationName = g.Key ?? "Inconnu", 
            AverageValue = g.Average(s => s.Value) 
        })
        .ToListAsync();
}
public async Task<List<LocationCountStat>> GetSensorCountByLocationAsync()
{
    return await _context.Sensors
        .Include(s => s.Location)  
        .GroupBy(s => s.Location.Name)
        .Select(g => new LocationCountStat
        {
            LocationName = g.Key,
            Count = g.Count()
        })
        .ToListAsync();
}
public async Task<List<SensorData>> SearchSensorsAsync(string? locationName, string? searchText, bool ShowCriticalOnly)
{
    // AsQueryable() prépare une requête sans l'exécuter
    IQueryable<SensorData> query = _context.Sensors.Include(s => s.Location).AsQueryable();

    // Si un lieu est fourni, on ajoute un WHERE au SQL
    if (!string.IsNullOrEmpty(locationName))
    {
        query = query.Where(s => s.Location.Name == locationName);
    }

    // Si un texte est fourni, on ajoute un autre WHERE (LIKE) au SQL
    if (!string.IsNullOrEmpty(searchText))
    {
        query = query.Where(s => s.Name.Contains(searchText));
    }

    if (ShowCriticalOnly)
    {
        query = query.Where(s => s.Value > 30.0);
    }
    // L'exécution SQL (SELECT ...) se fait uniquement ici, avec ToListAsync() !
    return await query.ToListAsync();
}
}