using Android.Provider;

namespace UnitedProjectApp.DB
{
    public class PhonesDBContract
    {
        public class Smartphone : IBaseColumns
        {
            public const string TableName = "Phone";

            public const string Model = "Model";

            public const string Manufacturer = "Manufacturer";

            public const string DiagonalSize = "DiagonalSize";

            public const string Warehouse = "WarehouseAddress";

            public static readonly SmartphoneEntity[] Data = new SmartphoneEntity[]
            {
                new SmartphoneEntity
                {
                    Manufacturer = "Motorola",
                    Model = "G32 6/128GB Satin Maroon",
                    DiagonalSize = 6.5,
                    WarehouseAddress = "525 S Winchester Blvd, San Jose, CA 95128, Сполучені Штати",
                },
                new SmartphoneEntity
                {
                    Manufacturer = "Motorola",
                    Model = "E20 2/32 GB Graphite",
                    DiagonalSize = 5.5,
                    WarehouseAddress = "1199 Jacklin Rd, Milpitas, CA 95035, Сполучені Штати",
                },
                new SmartphoneEntity
                {
                    Manufacturer = "Motorola",
                    Model = "E40 4/64GB Carbon Gray",
                    DiagonalSize = 4,
                    WarehouseAddress = "вулиця Стрийська, 85а, Львів, Львівська область, 79000"
                },
                new SmartphoneEntity
                {
                    Manufacturer = "Samsung",
                    Model = "Galaxy S22 Ultra 12/512 GB Phantom Black",
                    DiagonalSize = 6.8,
                    WarehouseAddress = "просп. Юрія Гагаріна, 8а (ТК Нагорка)"
                },
                new SmartphoneEntity
                {
                    Manufacturer = "Apple",
                    Model = "iPhone 14 128GB Blue",
                    DiagonalSize = 6.1,
                    WarehouseAddress = "просп. Дмитра Яворницького (Карла Маркса), 100"
                },
                new SmartphoneEntity
                {
                    Manufacturer = "Xiaomi",
                    Model = "Redmi Note 11 4/128 GB Twilight Blue",
                    DiagonalSize = 6.43,
                    WarehouseAddress = "просп. Дмитра Яворницького (Карла Маркса), 100"
                },
            };
        }
    }
}