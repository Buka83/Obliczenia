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
        public Dictionary<string, List<Przekroj_Prad>> typ_sytuacji_przekroj_prad = new Dictionary<string, List<Przekroj_Prad>>(); // przechowywanie prądów Idd kabla w odniesieniu do przekroju i sposobu ułożenia kabla
        public string[] kabel_rodzaj_srodowisko = { "3,4,5 ziemia", "1 ziemia", "3,4,5 powietrze", "1 powietrze" }; //4 możliwe sytuacje ułożenie kabla
        public string[] kabel_budowa = { "CU PCV", "CU XLPE", "AL PCV", "AL XLPE", "(N)HXH FE180", "(N)HXCH FE180" }; //6 mozliwych budowy kabli
        public Dictionary<string, Dictionary<int, List<double>>> kable_nazwa_budowa_przekroje = new Dictionary<string, Dictionary<int, List<double>>>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool powodzenie = wczytaj_plik_konfiguracyjny1();
            MessageBox.Show(powodzenie.ToString());
        }


        //
        private void dodaj_do_slownika(string klucz, Przekroj_Prad rekord, ref Dictionary<string, List<Przekroj_Prad>> slownik)
        {
            if (!slownik.ContainsKey(klucz))
                slownik.Add(klucz, new List<Przekroj_Prad>());
            
            slownik[klucz].Add(rekord);
        }

        //wczytywanie_plikow pliku ilośc zył_budowa kabla_przekrój zył roboczych_ prąd długotrwały
        public bool wczytaj_plik_konfiguracyjny1()
        {
            typ_sytuacji_przekroj_prad.Clear();
            try
            {
                string linia, klucz = "";
                using (StreamReader uchwyt_p = new StreamReader("styuacja_przekroj_prad.txt", Encoding.GetEncoding("windows-1250")))
                {
                    while ((linia = uchwyt_p.ReadLine()) != null)
                    {
                        if (linia.StartsWith(kabel_rodzaj_srodowisko[0]) || linia.StartsWith(kabel_rodzaj_srodowisko[1]) ||
                            linia.StartsWith(kabel_rodzaj_srodowisko[2]) || linia.StartsWith(kabel_rodzaj_srodowisko[3]))
                            klucz = linia;
                        else
                        {
                            string[] linia_p = linia.Split((char)09);
                            if (linia_p.Length == 2)
                                dodaj_do_slownika(klucz, new Przekroj_Prad(Convert.ToDouble(linia_p[0]), Convert.ToDouble(linia_p[1])), ref typ_sytuacji_przekroj_prad);
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

        private void dodaj_do_slownika2(string klucz, int klucz2,  ref Dictionary<string, Dictionary<int, List<double>>> slownik)
        {

        }
        public bool wczytaj_plik_konfiguracyjny2()
        {
            try
            {
                kable_nazwa_budowa_przekroje.Clear();
                string linia, klucz1 = "", klucz2 = "";
                using (StreamReader uchwyt_p = new StreamReader("nazwa_rodzaj_przekroj.txt", Encoding.GetEncoding("windows-1250")))
                {
                    while((linia = uchwyt_p.ReadLine()) != null)
                    {
                        if (linia.EndsWith(kabel_budowa[0]) || linia.EndsWith(kabel_budowa[1]) || linia.EndsWith(kabel_budowa[2]) ||
                            linia.EndsWith(kabel_budowa[3]) || linia.EndsWith(kabel_budowa[4]) || linia.EndsWith(kabel_budowa[5]))
                            klucz1 = linia;
                        else
                        {
                            string[] linia_p = linia.Split((char)09);
                            if (linia_p.Length == 2)
                            {

                            }
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

    
    

}
