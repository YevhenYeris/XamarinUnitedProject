using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitedProjectApp.DB;

namespace UnitedProjectApp
{
    [Activity(Label = "PhonesActivity")]
    public class PhonesActivity : Activity
    {
        private PhonesDBHelper _dBHelper;
        private bool _filterApplied = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.phones_layout);
            
            Button backButton = FindViewById<Button>(Resource.Id.backHome);

            backButton.Click += (sender, e) =>
            {
                this.Finish();
            };

            InitDB();
            DisplayAllPhones();
            DisplayAverageDiagonalSize();
            InitContacts();
            InitWarehouses();
        }

        private void InitDB()
        {
            _dBHelper = new PhonesDBHelper(this);
        }

        private void InitWarehouses()
        {
            var warehousesButton = FindViewById<Button>(Resource.Id.warehousesButton);
            warehousesButton.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(WarehousesActivity));
                StartActivity(nextActivity);
            };
        }

        private void InitContacts()
        {
            var contactsButton = FindViewById<Button>(Resource.Id.contactsButton);
            contactsButton.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(ContactsActivity));
                StartActivity(nextActivity);
            };
        }

        private void DisplayAllPhones()
        {
            var filterButton = FindViewById<Button>(Resource.Id.filterPhones);
            filterButton.Click += (object sender, EventArgs e) =>
            {
                filterButton.Text = _filterApplied ? "Показати усі моделі" : "Показати за фільтром";
                var phones = _filterApplied ? _dBHelper.GetPhonesByMinimalDiagonalSize("Motorola", 5) : _dBHelper.GetAllPhones();
                var list = FindViewById<ListView>(Resource.Id.mobile_list);
                var arrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.activity_listview, Resource.Id.listtextview, phones.Select(p => p.Manufacturer + " " + p.Model + " " + p.DiagonalSize).ToArray());
                list.Adapter = arrayAdapter;
                _filterApplied = !_filterApplied;
            };
        }

        private void DisplayAverageDiagonalSize()
        {
            var diagonalSizeText = FindViewById<TextView>(Resource.Id.averageDiagonalSize);
            diagonalSizeText.Text = "Середня довжина діагоналі становить " + _dBHelper.GetAverageDiagonalSize();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}