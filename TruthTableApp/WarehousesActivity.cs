using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitedProjectApp.DB;

namespace UnitedProjectApp
{
    [Activity(Label = "WarehousesActivity")]
    public class WarehousesActivity : Activity
    {
        private PhonesDBHelper _dBHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_warehouses);

            Button backButton = FindViewById<Button>(Resource.Id.backHome);

            backButton.Click += (sender, e) =>
            {
                this.Finish();
            };

            InitDB();
            InitWarehousesList();
        }

        private void InitDB()
        {
            _dBHelper = new PhonesDBHelper(this);
            _dBHelper.OnUpgrade(_dBHelper.WritableDatabase, 1, 2);
        }

        private void InitWarehousesList()
        {
            var warehouses = _dBHelper.GetAllWarehouses();
            var list = FindViewById<ListView>(Resource.Id.warehouses_list);
            var arrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.activity_listview, Resource.Id.listtextview, warehouses);
            list.Adapter = arrayAdapter;

            list.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) =>
            {
                var item = (string) args.Parent.GetItemAtPosition(args.Position);
                BuildRouteToWarehouse(item);
            };
        }

        private void BuildRouteToWarehouse(string address)
        {
            var joinedAddress = string.Join('+', address.Split(' '));
            var mapIntent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse("google.navigation:q=" + joinedAddress));
            StartActivity(mapIntent);
        }
    }
}