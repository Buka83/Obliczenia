using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Obliczenia
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public Dictionary<string, Dictionary<string, List<Przekroj_Prad>>> material_sytuacja_przekroj_prad = new Dictionary<string, Dictionary<string, List<Przekroj_Prad>>>();
        public Dictionary<string, Dictionary<int, Dictionary<double, Dictionary<string, double>>>> typ_ilzyl_przekr_sytuacja_idd = new Dictionary<string, Dictionary<int, Dictionary<double, Dictionary<string, double>>>>();
        public string[] kabel_rodzaj_srodowisko = { "3,4,5 ziemia", "1 ziemia TR", "1 ziemia PL", "3,4,5 powietrze", "1 powietrze TR", "1 powietrze PL" }; //6 możliwych sytuacji ułożenia kabla
        public string[] kabel_budowa = { "CU PCV", "CU XLPE", "AL PCV", "AL XLPE", "(N)HXH FE180", "(N)HXCH FE180" }; //6 mozliwych budowy kabli
        public Dictionary<string, Dictionary<int, List<double>>> kable_nazwa_budowa_przekroje = new Dictionary<string, Dictionary<int, List<double>>>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool powodzenie = wczytaj_plik_danych1();
            MessageBox.Show(powodzenie.ToString());
        }


        //
        private void dodaj_do_slownika(string klucz, Przekroj_Prad rekord, ref Dictionary<string, List<Przekroj_Prad>> slownik)
        {
            if (!slownik.ContainsKey(klucz))
                slownik.Add(klucz, new List<Przekroj_Prad>());
            
            slownik[klucz].Add(rekord);
        }

        private void dodaj_do_slownika1(string klucz, Przekroj_Prad rekord, ref Dictionary<string, Dictionary<string, List<Przekroj_Prad>>> slownik)
        {
            string[] kl = klucz.Split((char)09);
            if(!slownik.ContainsKey(kl[1]))
                slownik.Add(kl[1], new Dictionary<string, List<Przekroj_Prad>>());
            if (!slownik[kl[1]].ContainsKey(kl[0]))
                slownik[kl[1]].Add(kl[0], new List<Przekroj_Prad>());
            slownik[kl[1]][kl[0]].Add(rekord);

        }

        public bool wczytaj_plik_danych1()
        {
            material_sytuacja_przekroj_prad.Clear();
            try
            {
                string linia, klucz = "";
                using (StreamReader uchwyt_p = new StreamReader("styuacja_przekroj_prad.txt", Encoding.GetEncoding("windows-1250")))
                {
                    while ((linia = uchwyt_p.ReadLine()) != null)
                    {
                        if (linia.StartsWith(kabel_rodzaj_srodowisko[0]) || linia.StartsWith(kabel_rodzaj_srodowisko[1]) ||
                            linia.StartsWith(kabel_rodzaj_srodowisko[2]) || linia.StartsWith(kabel_rodzaj_srodowisko[3]) ||
                            linia.StartsWith(kabel_rodzaj_srodowisko[4]) || linia.StartsWith(kabel_rodzaj_srodowisko[5]))
                            klucz = linia;
                        else
                        {
                            string[] linia_p = linia.Split((char)09);
                            if (linia_p.Length == 2)
                                dodaj_do_slownika1(klucz, new Przekroj_Prad(Convert.ToDouble(linia_p[0]), Convert.ToDouble(linia_p[1])), ref material_sytuacja_przekroj_prad);
                        }
                    }
                }
                return true;
            }
            catch (Exception blad)
            {
                MessageBox.Show(blad.ToString());
                return false;
            }
        }

        private void dodaj_do_slownika2(string mat, string typ, int il_zyl, double przekr , ref Dictionary<string, Dictionary<int, Dictionary<double, Dictionary<string, double>>>> wy, Dictionary<string, Dictionary<string, List<Przekroj_Prad>>> dane)
        {
            if (!wy.ContainsKey(typ))
                wy.Add(typ, new Dictionary<int, Dictionary<double, Dictionary<string, double>>>());
            if (!wy[typ].ContainsKey(il_zyl))
                wy[typ].Add(il_zyl, new Dictionary<double, Dictionary<string, double>>());
            if (!wy[typ][il_zyl].ContainsKey(przekr))
                wy[typ][il_zyl].Add(przekr, new Dictionary<string, double>());

            if(dane.ContainsKey(mat))
            {

            }
            else
            {
                MessageBox.Show("nie ma takiego klucza w danych");
            }

        }
        public bool wczytaj_plik_konfiguracyjny2()
        {
            try
            {
                typ_ilzyl_przekr_sytuacja_idd.Clear();
                string linia, klucz = "";
                using (StreamReader uchwyt_p = new StreamReader("nazwa_rodzaj_przekroj.txt", Encoding.GetEncoding("windows-1250")))
                {
                    while((linia = uchwyt_p.ReadLine()) != null)
                    {
                        if (linia.EndsWith(kabel_budowa[0]) || linia.EndsWith(kabel_budowa[1]) || linia.EndsWith(kabel_budowa[2]) ||
                            linia.EndsWith(kabel_budowa[3]) || linia.EndsWith(kabel_budowa[4]) || linia.EndsWith(kabel_budowa[5]))
                            klucz = linia;
                        else
                        {
                            string[] kl = klucz.Split((char)09);
                            string[] rekord = linia.Split((char)09);
                            string[] il_przekr = rekord[1].Split((char)78);
                            dodaj_do_slownika2(kl[1], rekord[0], Convert.ToInt32(il_przekr[0]), Convert.ToDouble(il_przekr[1]), ref typ_ilzyl_przekr_sytuacja_idd, material_sytuacja_przekroj_prad);

                        }

                    }
                }
                return true;
            }
            catch (Exception blad)
            {
                MessageBox.Show(blad.ToString());
                return false;
            }

        }
    }

    public class wczytywanie_plikow
    {
        
    }

    public class Przekroj_Prad // klasa do przechowywania rekordu kabel prąd dopuszczalny długotrwale
    {
        public double przekroj_kabla;
        public double prad_Idd;

        public Przekroj_Prad(double przekroj_kabla, double prad_Idd)
        {
            this.przekroj_kabla = przekroj_kabla;
            this.prad_Idd = prad_Idd;
        }
    }
    public class Ulozenie_Prad // klasa do przechowywania rekordu kabel prąd dopuszczalny długotrwale
    {
        public string ulozenie;
        public double prad_Idd;

        public Ulozenie_Prad(string ulozenie, double prad_Idd)
        {
            this.ulozenie = ulozenie;
            this.prad_Idd = prad_Idd;
        }
    }




}
