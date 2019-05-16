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
        public Dictionary<string, Dictionary<string, Dictionary<double, double>>> material_sytuacja_przekroj_prad = new Dictionary<string, Dictionary<string, Dictionary <double, double>>>();
        public Dictionary<string, Dictionary<int, Dictionary<przek_ROB_PE, Dictionary<string, double>>>> typ_ilzyl_przekr_sytuacja_idd = new Dictionary<string, Dictionary<int, Dictionary<przek_ROB_PE, Dictionary<string, double>>>>();
        public string[] kabel_rodzaj_srodowisko = { "3,4,5 ziemia", "1 ziemia TR", "1 ziemia PL", "3,4,5 powietrze", "1 powietrze TR", "1 powietrze PL" }; //6 możliwych sytuacji ułożenia kabla
        public string[] kabel_budowa = { "CU PCV", "CU XLPE", "AL PCV", "AL XLPE", "(N)HXH FE180", "(N)HXCH FE180" }; //6 mozliwych budowy kabli
        public Dictionary<string, Dictionary<int, List<double>>> kable_nazwa_budowa_przekroje = new Dictionary<string, Dictionary<int, List<double>>>();


        
        private void dodaj_do_slownika(string klucz, Przekroj_Prad rekord, ref Dictionary<string, List<Przekroj_Prad>> slownik)
        {
            if (!slownik.ContainsKey(klucz))
                slownik.Add(klucz, new List<Przekroj_Prad>());
            
            slownik[klucz].Add(rekord);
        }

        private void dodaj_do_slownika1(string klucz, Przekroj_Prad rekord, ref Dictionary<string, Dictionary<string, Dictionary<double, double>>> slownik)
        {
            string[] kl = klucz.Split((char)09);
            if(!slownik.ContainsKey(kl[1]))
                slownik.Add(kl[1], new Dictionary<string, Dictionary<double, double>>());
            if (!slownik[kl[1]].ContainsKey(kl[0]))
                slownik[kl[1]].Add(kl[0], new Dictionary<double, double>());
            if (!slownik[kl[1]][kl[0]].ContainsKey(rekord.przekroj_kabla))
                slownik[kl[1]][kl[0]].Add(rekord.przekroj_kabla, rekord.prad_Idd);

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

        private void dodaj_do_slownika2(string mat, string typ, int il_zyl, przek_ROB_PE przekr , ref Dictionary<string, Dictionary<int, Dictionary<przek_ROB_PE, Dictionary<string, double>>>> wy, Dictionary<string, Dictionary<string, Dictionary<double, double>>> dane)
        {
            if (!wy.ContainsKey(typ))
                wy.Add(typ, new Dictionary<int, Dictionary<przek_ROB_PE, Dictionary<string, double>>>());
            if (!wy[typ].ContainsKey(il_zyl))
                wy[typ].Add(il_zyl, new Dictionary<przek_ROB_PE, Dictionary<string, double>>());
            

            if(dane.ContainsKey(mat))              
              foreach(string syt in dane[mat].Keys)
                if(dane[mat][syt].ContainsKey(przekr.przekr_ROB))
                    {
                        if (!wy[typ][il_zyl].ContainsKey(przekr))
                            wy[typ][il_zyl].Add(przekr, new Dictionary<string, double>());
                        wy[typ][il_zyl][przekr].Add(syt, dane[mat][syt][przekr.przekr_ROB]);
                    }
        }
        public bool wczytaj_plik_danych2()
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
                            string[] il_przekr = rekord[1].Split((char)120);
                            string[] rob_pe = il_przekr[1].Split((char)47);
                            if(rob_pe.Length < 2)
                                dodaj_do_slownika2(kl[1], rekord[0], Convert.ToInt32(il_przekr[0]), new przek_ROB_PE(Convert.ToDouble(rob_pe[0]), 0), ref typ_ilzyl_przekr_sytuacja_idd, material_sytuacja_przekroj_prad);
                            else
                                dodaj_do_slownika2(kl[1], rekord[0], Convert.ToInt32(il_przekr[0]), new przek_ROB_PE(Convert.ToDouble(rob_pe[0]), Convert.ToDouble(rob_pe[1])), ref typ_ilzyl_przekr_sytuacja_idd, material_sytuacja_przekroj_prad);

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wczytaj_plik_danych1();
            wczytaj_plik_danych2();
            lista_typow.Items.Clear(); lista_il_zyl.Items.Clear(); lista_przekroje.Items.Clear();
            foreach (string typ in typ_ilzyl_przekr_sytuacja_idd.Keys)
                lista_typow.Items.Add(typ);
        }

        private void Lista_typow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lista_typow.SelectedIndex != -1)
            {
                string typ = lista_typow.SelectedItem.ToString();
                lista_il_zyl.Items.Clear(); lista_przekroje.Items.Clear();
                foreach (int il_zyl in typ_ilzyl_przekr_sytuacja_idd[typ].Keys)
                    lista_il_zyl.Items.Add(il_zyl);
            }
        }

        private void Lista_il_zyl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lista_typow.SelectedIndex != -1 && lista_il_zyl.SelectedIndex != -1)
            {
                string typ = lista_typow.SelectedItem.ToString();
                int il_zyl = (int)lista_il_zyl.SelectedItem;
                lista_przekroje.Items.Clear();
                foreach (przek_ROB_PE przek_ROB_PE in typ_ilzyl_przekr_sytuacja_idd[typ][il_zyl].Keys)
                {
                    if (przek_ROB_PE.przekr_PE == 0)
                        lista_przekroje.Items.Add(przek_ROB_PE.przekr_ROB.ToString());
                    else
                        lista_przekroje.Items.Add(przek_ROB_PE.przekr_ROB.ToString() + "/" + przek_ROB_PE.przekr_PE.ToString());
                }
            }
        }

        private void Lista_przekroje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lista_typow.SelectedIndex != -1 && lista_il_zyl.SelectedIndex != -1 && lista_przekroje.SelectedIndex != -1)
            {
                string typ = lista_typow.SelectedItem.ToString();
                int il_zyl = (int)lista_il_zyl.SelectedItem;
                string[] s_przek_ROB_PE = lista_przekroje.SelectedItem.ToString().Split((char)47);
                przek_ROB_PE przek_ROB_PE = typ_ilzyl_przekr_sytuacja_idd[typ][il_zyl].ElementAt(lista_przekroje.SelectedIndex).Key;
                lista_sytuacja.Items.Clear();
                foreach (string syt in typ_ilzyl_przekr_sytuacja_idd[typ][il_zyl][przek_ROB_PE].Keys)
                    lista_sytuacja.Items.Add(syt);
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
    public class przek_ROB_PE : IEquatable<przek_ROB_PE>// klasa do przechowywania rekordu kabel prąd dopuszczalny długotrwale
    {
        public double przekr_ROB;
        public double przekr_PE;

        public przek_ROB_PE(double przekr_ROB, double przekr_PE)
        {
            this.przekr_ROB = przekr_ROB;
            this.przekr_PE = przekr_PE;
        }

        public bool Equals(przek_ROB_PE we)
        {
            return ((this.przekr_ROB == we.przekr_ROB) && (this.przekr_PE == we.przekr_PE));
        }
    }




}
