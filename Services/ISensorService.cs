
using DashboardData.Models;
namespace DashboardData.Services;

public interface ISensorService
{
    Task<List<SensorData>> GetSensorsAsync();
    void AddSensor(SensorData sensor);
    
    
    Task<List<SensorData>> GetCriticalSensorAsync(double threshold);

    Task<int>GetTotalCountAsync();
    Task<double> GetAverageValueAsync();
    Task<double> GetMaxValueAsync();

    Task<List<Location>> GetLocationsAsync();
    Task<SensorData?> GetSensorByIdAsync(int id);
    Task ReloadSensorsAsync(SensorData sensor);
    Task AddSensorAsync(SensorData sensor);
    Task UpdateSensorAsync(SensorData sensor);
    Task DeleteSensorAsync(int id);
    Task<List<LocationStat>> GetAverageValueByLocationAsync();

    Task<List<LocationCountStat>> GetSensorCountByLocationAsync();
    Task<List<SensorData>> SearchSensorsAsync(string? locationName, string? searchText, bool ShowCriticalOnly );
    }