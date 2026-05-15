using AbcPharmacy.Models;

namespace AbcPharmacy.Services;

/// <summary>
/// In-memory data store acting as the database layer.
/// Replace with EF Core DbContext when switching to a real database.
/// </summary>
public class DataStore
{
    private static int _medicineIdSeed = 1;
    private static int _saleIdSeed     = 1;

    public List<Medicine> Medicines { get; } = new();
    public List<Sale>     Sales     { get; } = new();

    public DataStore()
    {
        SeedMedicines();
        SeedSales();
    }

    // ── ID generators ────────────────────────────────────────────

    public int NextMedicineId() => _medicineIdSeed++;
    public int NextSaleId()     => _saleIdSeed++;

    // ── Seed data ────────────────────────────────────────────────

    private void SeedMedicines()
    {
        var now = DateTime.UtcNow;

        Medicines.AddRange(new List<Medicine>
        {
            // ── Normal medicines ──────────────────────────────────
            new() {
                Id         = NextMedicineId(),
                FullName   = "Paracetamol 500mg",
                Notes      = "For mild to moderate pain relief and fever reduction. Take with water.",
                ExpiryDate = now.AddMonths(18),
                Quantity   = 250,
                Price      = 12.50m,
                Brand      = "GSK Pharma",
                CreatedAt  = now.AddDays(-60),
                UpdatedAt  = now.AddDays(-5)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Amoxicillin 250mg Capsules",
                Notes      = "Broad-spectrum antibiotic. Complete the full course as prescribed.",
                ExpiryDate = now.AddMonths(14),
                Quantity   = 180,
                Price      = 45.00m,
                Brand      = "Cipla",
                CreatedAt  = now.AddDays(-45),
                UpdatedAt  = now.AddDays(-2)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Ibuprofen 400mg",
                Notes      = "Anti-inflammatory and pain reliever. Take after food.",
                ExpiryDate = now.AddMonths(22),
                Quantity   = 320,
                Price      = 18.75m,
                Brand      = "Sun Pharma",
                CreatedAt  = now.AddDays(-30),
                UpdatedAt  = now.AddDays(-1)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Omeprazole 20mg",
                Notes      = "Proton pump inhibitor for acid reflux and gastric ulcers.",
                ExpiryDate = now.AddMonths(20),
                Quantity   = 150,
                Price      = 35.00m,
                Brand      = "AstraZeneca",
                CreatedAt  = now.AddDays(-20),
                UpdatedAt  = now.AddDays(-1)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Cetirizine 10mg",
                Notes      = "Antihistamine for allergy relief. May cause drowsiness.",
                ExpiryDate = now.AddMonths(16),
                Quantity   = 200,
                Price      = 22.00m,
                Brand      = "Zydus Cadila",
                CreatedAt  = now.AddDays(-15),
                UpdatedAt  = now
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Metformin 500mg",
                Notes      = "Type 2 diabetes management. Take with meals to reduce GI side effects.",
                ExpiryDate = now.AddMonths(24),
                Quantity   = 400,
                Price      = 28.50m,
                Brand      = "Lupin",
                CreatedAt  = now.AddDays(-10),
                UpdatedAt  = now
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Atorvastatin 20mg",
                Notes      = "Cholesterol-lowering statin. Take at the same time each day.",
                ExpiryDate = now.AddMonths(30),
                Quantity   = 120,
                Price      = 55.00m,
                Brand      = "Pfizer",
                CreatedAt  = now.AddDays(-8),
                UpdatedAt  = now
            },

            // ── Expiring soon (< 30 days) — RED indicator ────────
            new() {
                Id         = NextMedicineId(),
                FullName   = "Azithromycin 500mg",
                Notes      = "Antibiotic for respiratory and skin infections. 3-day course.",
                ExpiryDate = now.AddDays(20),
                Quantity   = 60,
                Price      = 85.00m,
                Brand      = "Cipla",
                CreatedAt  = now.AddDays(-90),
                UpdatedAt  = now.AddDays(-3)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Pantoprazole 40mg",
                Notes      = "Gastric acid suppressant. Take 30 minutes before meals.",
                ExpiryDate = now.AddDays(12),
                Quantity   = 45,
                Price      = 42.00m,
                Brand      = "Torrent Pharma",
                CreatedAt  = now.AddDays(-100),
                UpdatedAt  = now.AddDays(-7)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Diclofenac Sodium 50mg",
                Notes      = "NSAID for pain and inflammation. Take with food.",
                ExpiryDate = now.AddDays(5),
                Quantity   = 30,
                Price      = 16.00m,
                Brand      = "Novartis",
                CreatedAt  = now.AddDays(-120),
                UpdatedAt  = now.AddDays(-10)
            },

            // ── Low stock (< 10 units) — YELLOW indicator ────────
            new() {
                Id         = NextMedicineId(),
                FullName   = "Insulin Glargine 100IU/ml",
                Notes      = "Long-acting insulin analog. Store in refrigerator. Single use only.",
                ExpiryDate = now.AddMonths(6),
                Quantity   = 7,
                Price      = 890.00m,
                Brand      = "Sanofi",
                CreatedAt  = now.AddDays(-40),
                UpdatedAt  = now.AddDays(-2)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Levothyroxine 50mcg",
                Notes      = "Thyroid hormone replacement. Take on an empty stomach in the morning.",
                ExpiryDate = now.AddMonths(10),
                Quantity   = 5,
                Price      = 120.00m,
                Brand      = "Abbott",
                CreatedAt  = now.AddDays(-35),
                UpdatedAt  = now.AddDays(-1)
            },
            new() {
                Id         = NextMedicineId(),
                FullName   = "Clonazepam 0.5mg",
                Notes      = "Benzodiazepine for anxiety and seizure disorders. Schedule H drug.",
                ExpiryDate = now.AddMonths(8),
                Quantity   = 3,
                Price      = 65.00m,
                Brand      = "Roche",
                CreatedAt  = now.AddDays(-50),
                UpdatedAt  = now.AddDays(-4)
            },

            // ── Both low stock AND expiring soon ─────────────────
            new() {
                Id         = NextMedicineId(),
                FullName   = "Dexamethasone 0.5mg",
                Notes      = "Corticosteroid for inflammatory conditions. Taper dose as directed.",
                ExpiryDate = now.AddDays(18),
                Quantity   = 8,
                Price      = 30.00m,
                Brand      = "Cadila Healthcare",
                CreatedAt  = now.AddDays(-80),
                UpdatedAt  = now.AddDays(-6)
            },

            // ── Already expired (for testing) ─────────────────────
            new() {
                Id         = NextMedicineId(),
                FullName   = "Ranitidine 150mg",
                Notes      = "H2 blocker — recalled in many markets. Do not sell.",
                ExpiryDate = now.AddDays(-10),
                Quantity   = 0,
                Price      = 15.00m,
                Brand      = "GSK Pharma",
                CreatedAt  = now.AddDays(-200),
                UpdatedAt  = now.AddDays(-15)
            }
        });
    }

    private void SeedSales()
    {
        var now = DateTime.UtcNow;

        Sales.AddRange(new List<Sale>
        {
            new() {
                Id            = NextSaleId(),
                MedicineId    = 1,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 1),
                QuantitySold  = 10,
                UnitPrice     = 12.50m,
                TotalAmount   = 125.00m,
                CustomerName  = "Rahul Sharma",
                Notes         = "Regular monthly purchase",
                SoldAt        = now.AddDays(-5)
            },
            new() {
                Id            = NextSaleId(),
                MedicineId    = 2,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 2),
                QuantitySold  = 3,
                UnitPrice     = 45.00m,
                TotalAmount   = 135.00m,
                CustomerName  = "Priya Patel",
                Notes         = "Prescription by Dr. Mehta",
                SoldAt        = now.AddDays(-4)
            },
            new() {
                Id            = NextSaleId(),
                MedicineId    = 3,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 3),
                QuantitySold  = 20,
                UnitPrice     = 18.75m,
                TotalAmount   = 375.00m,
                CustomerName  = "City Hospital",
                Notes         = "Bulk order for ward stock",
                SoldAt        = now.AddDays(-3)
            },
            new() {
                Id            = NextSaleId(),
                MedicineId    = 1,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 1),
                QuantitySold  = 5,
                UnitPrice     = 12.50m,
                TotalAmount   = 62.50m,
                CustomerName  = "Anjali Singh",
                SoldAt        = now.AddDays(-2)
            },
            new() {
                Id            = NextSaleId(),
                MedicineId    = 7,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 7),
                QuantitySold  = 15,
                UnitPrice     = 55.00m,
                TotalAmount   = 825.00m,
                CustomerName  = "Suresh Kumar",
                Notes         = "3-month supply",
                SoldAt        = now.AddDays(-1)
            },
            new() {
                Id            = NextSaleId(),
                MedicineId    = 11,
                Medicine      = Medicines.FirstOrDefault(m => m.Id == 11),
                QuantitySold  = 2,
                UnitPrice     = 890.00m,
                TotalAmount   = 1780.00m,
                CustomerName  = "Meena Devi",
                Notes         = "Insulin — check cold chain",
                SoldAt        = now
            },
        });
    }
}
