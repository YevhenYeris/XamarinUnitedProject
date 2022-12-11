using Android.Content;
using Android.Database.Sqlite;
using Android.Provider;
using Java.Nio.Channels;
using Java.Util;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnitedProjectApp.DB;
using static Android.Database.Sqlite.SQLiteDatabase;

namespace UnitedProjectApp.DB
{
    public class PhonesDBHelper : SQLiteOpenHelper
    {
        private const int _databaseVersion = 1;
        private const string _databaseName = "Phones.db";
        private const string _tableCreationScript = "CREATE TABLE " + PhonesDBContract.Smartphone.TableName + " (" +
            IBaseColumns.Id + " INTEGER PRIMARY KEY, " +
            PhonesDBContract.Smartphone.Model + " TEXT, " +
            PhonesDBContract.Smartphone.Manufacturer + " TEXT, " +
            PhonesDBContract.Smartphone.DiagonalSize + " REAL, " +
            PhonesDBContract.Smartphone.Warehouse + " TEXT)";
        private const string _tableDeletionScript = "DROP TABLE IF EXISTS " + PhonesDBContract.Smartphone.TableName;
        private const string _getAverageDiagonalSizeScript = "SELECT AVG(" + PhonesDBContract.Smartphone.DiagonalSize + ") AS AvgDiagonalSize FROM " + PhonesDBContract.Smartphone.TableName;
        public PhonesDBHelper(Context context)
            : base(context, _databaseName, null, _databaseVersion)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(_tableCreationScript);

            foreach (var phone in PhonesDBContract.Smartphone.Data)
            {
                var values = new ContentValues();
                values.Put(PhonesDBContract.Smartphone.Model, phone.Model);
                values.Put(PhonesDBContract.Smartphone.Manufacturer, phone.Manufacturer);
                values.Put(PhonesDBContract.Smartphone.DiagonalSize, phone.DiagonalSize);
                values.Put(PhonesDBContract.Smartphone.Warehouse, phone.WarehouseAddress);
                db.Insert(PhonesDBContract.Smartphone.TableName, null, values);
            }
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(_tableDeletionScript);
            OnCreate(db);
        }

        public override void OnDowngrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            OnUpgrade(db, oldVersion, newVersion);
        }

        public List<SmartphoneEntity> GetPhonesByMinimalDiagonalSize(string manufacturer, double diagonalSize)
        {
            var projection = new string[]
            {
                IBaseColumns.Id,
                PhonesDBContract.Smartphone.Model,
                PhonesDBContract.Smartphone.Manufacturer,
                PhonesDBContract.Smartphone.DiagonalSize,
                PhonesDBContract.Smartphone.Warehouse,
            };

            var selection = PhonesDBContract.Smartphone.Manufacturer + " = ? AND " +
                        PhonesDBContract.Smartphone.DiagonalSize + " > ?";
            var selectionArgs = new string[]
            {
                manufacturer,
                diagonalSize.ToString(),
            };

            var sortOrder = PhonesDBContract.Smartphone.DiagonalSize + " DESC";
            var cursor = ReadableDatabase.Query(
                PhonesDBContract.Smartphone.TableName,
                projection,
                selection,
                selectionArgs,
                null,
                null,
                sortOrder);
            var phoneList = new List<SmartphoneEntity>();

            while (cursor.MoveToNext())
            {
                phoneList.Add(new SmartphoneEntity
                {
                    Id = cursor.GetInt(cursor.GetColumnIndexOrThrow(IBaseColumns.Id)),
                    Manufacturer = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Manufacturer)),
                    Model = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Model)),
                    DiagonalSize = cursor.GetDouble(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.DiagonalSize)),
                    WarehouseAddress = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Warehouse)),
                });
            }
            cursor.Close();
            return phoneList;
        }

        public List<SmartphoneEntity> GetAllPhones()
        {
            var projection = new string[]
            {
                IBaseColumns.Id,
                PhonesDBContract.Smartphone.Model,
                PhonesDBContract.Smartphone.Manufacturer,
                PhonesDBContract.Smartphone.DiagonalSize,
                PhonesDBContract.Smartphone.Warehouse,
            };

            var sortOrder = PhonesDBContract.Smartphone.DiagonalSize + " DESC";
            var cursor = ReadableDatabase.Query(
                PhonesDBContract.Smartphone.TableName,
                projection,
                null,
                null,
                null,
                null,
                sortOrder);
            var phoneList = new List<SmartphoneEntity>();

            while (cursor.MoveToNext())
            {
                phoneList.Add(new SmartphoneEntity
                {
                    Id = cursor.GetInt(cursor.GetColumnIndexOrThrow(IBaseColumns.Id)),
                    Manufacturer = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Manufacturer)),
                    Model = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Model)),
                    DiagonalSize = cursor.GetDouble(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.DiagonalSize)),
                    WarehouseAddress = cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Warehouse)),
                });
            }
            cursor.Close();
            return phoneList;
        }

        public List<string> GetAllWarehouses()
        {
            var projection = new string[]
            {
                PhonesDBContract.Smartphone.Warehouse,
            };

            var cursor = ReadableDatabase.Query(
                true,
                PhonesDBContract.Smartphone.TableName,
                projection,
                null,
                null,
                null,
                null,
                null,
                null);
            var warehousesList = new List<string>();

            while (cursor.MoveToNext())
            {
                warehousesList.Add(cursor.GetString(cursor.GetColumnIndexOrThrow(PhonesDBContract.Smartphone.Warehouse)));
            }
            cursor.Close();
            return warehousesList;
        }

        public double GetAverageDiagonalSize()
        {
            var cursor = ReadableDatabase.RawQuery(_getAverageDiagonalSizeScript, null);
            var averageDiagonalSize = .0;

            while (cursor.MoveToNext())
            {
                averageDiagonalSize = cursor.GetDouble(cursor.GetColumnIndexOrThrow("AvgDiagonalSize"));
            }

            cursor.Close();
            return averageDiagonalSize;
        }
    }
}