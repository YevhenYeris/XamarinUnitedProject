using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Android.Text;
using UnitedProjectApp.TruthTableBuilder;
using System.Linq;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Android.Views.TextClassifiers;
using static System.Net.Mime.MediaTypeNames;
using Android.Content;
using Android.Graphics;
using System.Text;
using Java.IO;
using System.Runtime.Remoting.Contexts;
using Java.Util;
using Scanner = UnitedProjectApp.TruthTableBuilder.Scanner;
using Xamarin.Essentials;

namespace UnitedProjectApp
{
    public class FormulaCalculatedEventArgs: EventArgs
    {
        public bool IsSuccessful { get; set; }

        public TruthTable TruthTable { get; set; }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public event EventHandler<FormulaCalculatedEventArgs> FormulaCalculated;
        private readonly string Nothing = "Обрати формулу";
        private readonly string formulasFileName = "formulas";

        public static TruthTable TruthTable { get; set; }

        private void Init()
        {
            RequestPermissions(new String[]{"android.permission.WRITE_EXTERNAL_STORAGE","android.permission.READ_EXTERNAL_STORAGE"}, 1);

            FormulaCalculated += CheckError;
            FormulaCalculated += ShowResults;
            FormulaCalculated += DisplayShowTableButton;

            var calculator = new TruthTableCalculator();
            var scanner = new Scanner();
            var parser = new Parser(scanner);

            var phonesBtn = FindViewById<Button>(Resource.Id.btn_phones);
            phonesBtn.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(PhonesActivity));
                StartActivity(nextActivity);
            };

            var showTableBtn = FindViewById<Button>(Resource.Id.btn_showtable);
            showTableBtn.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(ShowTableActivity));
                StartActivity(nextActivity);
            };

            var entry = FindViewById<EditText>(Resource.Id.entry);
            entry.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                try
                {
                    var parserResult = parser.Parse(string.Concat(e.Text));
                    TruthTable = calculator.GenerateTruthTable(parserResult);

                    OnFormulaCalculated(new FormulaCalculatedEventArgs()
                    {
                        IsSuccessful = true,
                        TruthTable = TruthTable
                    });
                }
                catch(Exception ex)
                {
                    OnFormulaCalculated(new FormulaCalculatedEventArgs()
                    {
                        IsSuccessful = false,
                        TruthTable = null
                    });
                }
            };

            var spinner = FindViewById<Spinner>(Resource.Id.spinner_formulas);
            var btnClear = FindViewById<Button>(Resource.Id.btn_clear);
            btnClear.Click += (sender, e) => ClearFormulas();
            var importBtn = FindViewById<Button>(Resource.Id.btn_import);
            importBtn.Click += (object sender, EventArgs e) =>
            {
                var formulas = new List<string>() { Nothing };
                formulas.AddRange(ReadFormulas());
                ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, formulas.ToArray());
                spinner.Adapter = arrayAdapter;
                spinner.Visibility = ViewStates.Visible;
                //btnClear.Visibility = ViewStates.Visible;
            };

            spinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                var itemText = ((Spinner)sender).SelectedItem.ToString();
                var entry = FindViewById<EditText>(Resource.Id.entry);

                if (itemText == Nothing)
                {
                    return;
                }

                entry.Text = itemText; 
                spinner.Visibility = ViewStates.Gone;
                btnClear.Visibility = ViewStates.Gone;
            };

            var exportBtn = FindViewById<Button>(Resource.Id.btn_export);
            exportBtn.Click += (object sender, EventArgs e) => SaveFormula(entry.Text);

            //ContextWrapper contextWrapper = new ContextWrapper(this);
            //var directory = contextWrapper.GetDir(FilesDir.Name, FileCreationMode.MultiProcess);
            //var file =  new Java.IO.File(directory, "formulas");
            //String data = "TEST DATA";
            //FileOutputStream fos = new FileOutputStream(file, true); // save
            //fos.Write(System.Text.Encoding.UTF8.GetBytes(data));
            //fos.Close();
        }

        private void ShowResults(object sender, FormulaCalculatedEventArgs eventArgs)
        {
            var textExec = FindViewById<TextView>(Resource.Id.text_exec);
            var textEq = FindViewById<TextView>(Resource.Id.text_eq);

            if (eventArgs.TruthTable == null)
            {
                textExec.Visibility = ViewStates.Invisible;
                textEq.Visibility = ViewStates.Invisible;
                return;
            }

            textExec.Visibility = ViewStates.Visible;
            textEq.Visibility = ViewStates.Visible;

            textExec.Text = eventArgs.TruthTable.IsExecutable ?
                Resources.GetString(Resource.String.exec) :
                Resources.GetString(Resource.String.not_exec);
            textEq.Text = eventArgs.TruthTable.IsEquality ?
                Resources.GetString(Resource.String.eq) :
                Resources.GetString(Resource.String.not_eq);
        }

        private void CheckError(object sender, FormulaCalculatedEventArgs eventArgs)
        {
            var textError = FindViewById<TextView>(Resource.Id.text_error);
            textError.Visibility = eventArgs.IsSuccessful ?
                                     ViewStates.Invisible :
                                     ViewStates.Visible;
        }

        private void DisplayShowTableButton(object sender, FormulaCalculatedEventArgs eventArgs)
        {
            var button = FindViewById<Button>(Resource.Id.btn_showtable);

            if (eventArgs.TruthTable == null)
            {
                button.Visibility = ViewStates.Invisible;
                return;
            }

            button.Visibility = ViewStates.Visible;
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Init();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.action_info)
            {
                Intent nextActivity = new Intent(this, typeof(AuthorInfoActivity));
                StartActivity(nextActivity);
            }
            
            if (id == Resource.Id.truth_main ||
                id == Resource.Id.truth_export ||
                id == Resource.Id.truth_import ||
                id == Resource.Id.truth_table ||
                id == Resource.Id.phones_contacts ||
                id == Resource.Id.phones_main ||
                id == Resource.Id.phones_warehouses)
            {
                var nextActivity = new Intent(this, typeof(AppHelpActivity));
                var bundle = new Bundle();
                bundle.PutInt("page", id);
                nextActivity.PutExtras(bundle);
                StartActivity(nextActivity, bundle);
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Intent nextActivity = new Intent(this, typeof(HelpActivity));
            StartActivity(nextActivity);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private bool SaveFormula(string formula)
        {
            if (formula == null || !formula.Trim().Any())
            {
                return false;
            }

            formula = formula.Trim();
            
            try
            {
                var oldText = readFileFromInternalStorage();
                var formulas = oldText.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                if (formulas.Contains(formula))
                {
                    View viewerr = FindViewById<Spinner>(Resource.Id.spinner_formulas);
                    Snackbar.Make(viewerr, "Формулу уже додано", Snackbar.LengthLong)
                        .SetAction("Action", (View.IOnClickListener)null).Show();
                    return false;
                }

                writeFileOnInternalStorage(oldText + formula);
            }
            catch (Exception ex)
            {
                return false;
            }

             View view = FindViewById<Spinner>(Resource.Id.spinner_formulas);
                    Snackbar.Make(view, "Формулу збережено", Snackbar.LengthLong)
                        .SetAction("Action", (View.IOnClickListener)null).Show();

            return true;
        }

        private IEnumerable<string> ReadFormulas()
        {
            var formulasString = readFileFromInternalStorage();

            if (string.IsNullOrEmpty(formulasString))
            {
                return Enumerable.Empty<string>();
            }

            return formulasString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        private void ClearFormulas()
        {
            writeFileOnInternalStorage(string.Empty);
        }

        private void OnFormulaCalculated(FormulaCalculatedEventArgs eventArgs)
        {
            if (FormulaCalculated == null)
            {
                return;
            }

            FormulaCalculated(this, eventArgs);
        }

        private void writeFileOnInternalStorage(string sBody)
        {
            var basePath = System.IO.Path.Combine(DataDir.AbsolutePath, formulasFileName);
            if (string.IsNullOrEmpty(basePath))
            {
                throw new Exception("No valid storage for the app.");
            }
            using (var writer = System.IO.File.CreateText(basePath))
            {
                writer.WriteLine(sBody);
            }
        }

        private string readFileFromInternalStorage()
        {
            var basePath = System.IO.Path.Combine(DataDir.AbsolutePath, formulasFileName);

            if (!System.IO.File.Exists(basePath))
            {
                writeFileOnInternalStorage(string.Empty);
            }

            if (string.IsNullOrEmpty(basePath) || !System.IO.File.Exists(basePath))
            {
                throw new Exception("File \"" + formulasFileName + "\" is not exist.");
            }

            string res = "";

            using(var reader = new StreamReader(basePath, true))
            {
                res = reader.ReadToEnd();
            }

            return res;
        }
    }
}
