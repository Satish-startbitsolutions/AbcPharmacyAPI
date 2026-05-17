using AbcPharmacy.Models;
using System.Text.Json;

namespace AbcPharmacy.Services;

public class DataStore
{
    private readonly string _dataFolder;
    private readonly string _medicineFile;
    private readonly string _saleFile;

    private static int _medicineIdSeed = 1;
    private static int _saleIdSeed = 1;

    public List<Medicine> Medicines { get; private set; } = new();
    public List<Sale> Sales { get; private set; } = new();

    public DataStore()
    {
        _dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        _medicineFile = Path.Combine(_dataFolder, "medicines.json");
        _saleFile = Path.Combine(_dataFolder, "sales.json");

        Directory.CreateDirectory(_dataFolder);

        Medicines = LoadFromFile<Medicine>(_medicineFile);
        Sales = LoadFromFile<Sale>(_saleFile);

        SetNextIds();
        AttachMedicineNavigation();
    }

    public int NextMedicineId() => _medicineIdSeed++;
    public int NextSaleId() => _saleIdSeed++;

    public void SaveMedicines()
    {
        SaveToFile(_medicineFile, Medicines);
    }

    public void SaveSales()
    {
        SaveToFile(_saleFile, Sales);
    }

    public void SaveAll()
    {
        SaveMedicines();
        SaveSales();
    }

    private List<T> LoadFromFile<T>(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "[]");
            return new List<T>();
        }

        var json = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<T>();
    }

    private void SaveToFile<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    private void SetNextIds()
    {
        _medicineIdSeed = Medicines.Any()
            ? Medicines.Max(m => m.Id) + 1
            : 1;

        _saleIdSeed = Sales.Any()
            ? Sales.Max(s => s.Id) + 1
            : 1;
    }

    private void AttachMedicineNavigation()
    {
        foreach (var sale in Sales)
        {
            sale.Medicine = Medicines.FirstOrDefault(m => m.Id == sale.MedicineId);
        }
    }
}