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

namespace UnitedProjectApp
{
    [Activity(Label = "AppHelpActivity")]
    public class AppHelpActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var contentView = Resource.Layout.activity_app_help;
            var bundle = Intent.Extras;

            switch (bundle.GetInt("page"))
            {
                case Resource.Id.truth_main: contentView = Resource.Layout.truth_main; break;
                case Resource.Id.truth_export: contentView = Resource.Layout.truth_export; break;
                case Resource.Id.truth_import: contentView = Resource.Layout.truth_import; break;
                case Resource.Id.truth_table: contentView = Resource.Layout.truth_table; break;
                case Resource.Id.phones_contacts: contentView = Resource.Layout.phones_contacts; break;
                case Resource.Id.phones_main: contentView = Resource.Layout.phones_main; break;
                case Resource.Id.phones_warehouses: contentView = Resource.Layout.phones_warehouses; break;
            }
            
            SetContentView(contentView);

            Button backButton = FindViewById<Button>(Resource.Id.backHome);

            backButton.Click += (sender, e) =>
            {
                this.Finish();
            };
        }
    }
}