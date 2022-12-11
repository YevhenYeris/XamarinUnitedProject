using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitedProjectApp
{
    [Activity(Label = "ContactsActivity")]
    public class ContactsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_contacts);

            Button backButton = FindViewById<Button>(Resource.Id.backHome);

            backButton.Click += (sender, e) =>
            {
                this.Finish();
            };

            CheckPermission("android.permission.READ_CONTACTS", 100);

            GetContacts();
        }

        private void GetContacts()
        {
            var whereQuery = "LENGTH(" + ContactsContract.Contacts.InterfaceConsts.DisplayName + ")" + " > 10";
            var cursor = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, whereQuery, null, null);
            StartManagingCursor(cursor);
            // ContactsContract.CommonDataKinds.StructuredName.FamilyName
            String[] data = { ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName, ContactsContract.CommonDataKinds.Phone.Number };
            int[] to = { Resource.Id.listtextview, Resource.Id.listtextviewSecond };
            SimpleCursorAdapter adapter = new SimpleCursorAdapter(this, Resource.Layout.activity_listview, cursor, data, to, CursorAdapterFlags.None);
            var listView = FindViewById<ListView>(Resource.Id.contacts_list);
            listView.Adapter = adapter;
            listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;
        }

        public void CheckPermission(String permission, int requestCode)
        {
            if (ContextCompat.CheckSelfPermission(this, permission) == Android.Content.PM.Permission.Denied) {
                ActivityCompat.RequestPermissions(this, new String[] { permission }, requestCode);
            }
            else {
                Toast.MakeText(this, "Permission already granted", ToastLength.Short).Show();
            }
        }
    }
}