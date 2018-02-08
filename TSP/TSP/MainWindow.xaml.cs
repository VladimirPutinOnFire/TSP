using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TSP 
{
    /*
     * TODO: 
     * -Distanz zwischen den Städten grafisch anzeigen
     * -Neu generierte Städte mit moveTo animieren (on hold)
     * -DataBinding Part 2, Die Binding Klasse
     * -Anzahl der generierten Städte dynamisieren:
     *      -XAML dynamisch generieren
     *      -Kontrollelemente dynmaisch hinzufügen
     * 
     * */
    public partial class MainWindow : Window
    {
        int CityCount = 5;
        
        bool FirstGenerated = true;
        Dictionary<int, City> Städte = new Dictionary<int, City>();
        private List<UIElement> KontrollElementeCity = new List<UIElement>();
        private List<TextBlock> KontrollElementeTextBlock = new List<TextBlock>();
        private Dictionary<String, double> Distanzen = new Dictionary<String, double>();
        //Alle Städte ausgenommen der ersten. Diese sind relevant für die Permutationen
        private List<String> CityListe = new List<String>();
        //Eine vollständige Rundtrip-Permutation mit einem Stadtnamen als String pro Index
        private List<String> Permutation = new List<String>();
        //Sammlung aller Permutationen (City 2..City N) als generischer Typ für die Permutationsmethode
        private IEnumerable<IEnumerable<String>> Liste;
        //Sammlung aller Permutationen (City 2..City N)
        private List<IEnumerable<String>> list;

        //Das Source Objekt für das Databinding
        Data daten = new Data();

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        { 
            TestBlock.Text = String.Format("Gesamtdistanz City 1 bis 5: {0:0}{1}", Permutation_Gesamtdistanz_Berechnen(Permutation), Environment.NewLine );
            TestBlock.Text += String.Format("Die Strecke von City 2 bis City 3 ist: {0:0}", Konkrete_Distanz_Berechnen(Städte[2], Städte[3]));
        }

        public MainWindow()
        {
            this.Title = "TSP";
            InitializeComponent();
            DataContext = daten;
            Generieren();
            //TODO: Binding-Klasse angehen -> Testbox.SetBinding(TextBlock.TextProperty, dieBindung);
        }

        private void Generieren_Click(object sender, RoutedEventArgs e)
        {
            Generieren();
        }

        //Animation ist bugged
        private void Generieren()
        {
            InitControlList();
            if (FirstGenerated)
            {
                for (int i = 0; i < CityCount; i++)
                {
                    int Cityindex = i + 1;
                    String Cityname = "City" + (Cityindex).ToString();

                    //Koordinaten um die Größe des Icons anpassen
                    double X = Canvas.GetTop(KontrollElementeCity[i]);
                    double Y = Canvas.GetLeft(KontrollElementeCity[i]);

                    //TODO: X- und Y-Position random implementieren
                    Städte.Add(Cityindex, new City(Cityname, Cityindex, X, Y));
                }
                FirstGenerated = false;
            }
            else
            {
                //Ohne Animation
                if (!AnimationCheckBox.IsChecked.Value)
                {
                    Städte.Clear();
                    Random rnd = new Random();

                    for (int i = 0; i < CityCount; i++)
                    {
                        int Cityindex = i + 1;
                        String Cityname = "City" + (Cityindex).ToString();

                        //Koordinaten um die Größe des Icons anpassen
                        int X = rnd.Next(100, 500) - 12;
                        int Y = rnd.Next(100, 500) - 24;

                        //TODO: X- und Y-Position random implementieren
                        Städte.Add(Cityindex, new City(Cityname, Cityindex, X, Y));
                    }

                    //Die Zugriffsliste für die UI-Elemente initialisieren
                    //Die UI-Elemente an die ausgewürfelten Positionen setzen

                    Position_Anpassen();

                }
                //Mit Animation
                if (AnimationCheckBox.IsChecked.Value)
                {
                    Random rnd = new Random();
                    for (int i = 0; i < CityCount; i++)
                    {
                        int Cityindex = i + 1;
                        double oldX = Canvas.GetTop(KontrollElementeCity[i]);
                        double oldY = Canvas.GetLeft(KontrollElementeCity[i]);
                        double newX = rnd.Next(100, 500) - 12;
                        double newY = rnd.Next(100, 500) - 24;
                        Städte[Cityindex].X = newX;
                        Städte[Cityindex].Y = newY;
                        HilfsMethoden.MoveTo(KontrollElementeCity[i], oldX, oldY, newX, newY);
                    }
                    Position_Anpassen_Animation();
                }
            }
            Distanzen_Berechnen();
        }

        private void Position_Anpassen()
        {
            
            //erstmal statisch an die XAML Elemente binden. TODO: dynamisch generieren
            //note: ist dynamisch, solange die Listenelemente (UIElement) dynamisch dazukommen)
            for (int i = 0; i < CityCount; i++)
            {
                //Position des City Images
                Canvas.SetTop(KontrollElementeCity[i], Städte[i + 1].Y);
                Canvas.SetLeft(KontrollElementeCity[i], Städte[i + 1].X);
                //Position des zugehörigen TextBlocks
                Canvas.SetTop(KontrollElementeTextBlock[i], Städte[i + 1].Y-6);
                Canvas.SetLeft(KontrollElementeTextBlock[i], Städte[i + 1].X+25);
                //Name der Stadt in den TextBlock schreiben
                KontrollElementeTextBlock[i].Text = String.Format("{0}  ({1},{2})", 
                    Städte[i + 1].Name, Städte[i + 1].X, Städte[i + 1].Y);
            }
        }

        private void Position_Anpassen_Animation()
        {

            //erstmal statisch an die XAML Elemente binden. TODO: dynamisch generieren
            //note: ist dynamisch, solange die Listenelemente (UIElement) dynamisch dazukommen)
            for (int i = 0; i < CityCount; i++)
            {
                //Position des City Images
                //Canvas.SetTop(KontrollElementeCity[i], Städte[i + 1].Y);
                //Canvas.SetLeft(KontrollElementeCity[i], Städte[i + 1].X);
                //Position des zugehörigen TextBlocks
                Canvas.SetTop(KontrollElementeTextBlock[i], Städte[i + 1].Y - 6);
                Canvas.SetLeft(KontrollElementeTextBlock[i], Städte[i + 1].X + 25);
                //Name der Stadt in den TextBlock schreiben
                KontrollElementeTextBlock[i].Text = String.Format("{0}  ({1},{2})",
                    Städte[i + 1].Name, Städte[i + 1].X, Städte[i + 1].Y);
            }
        }


        private void Distanzen_Berechnen()
        {
            Distanzen.Clear();
            Permutation.Clear();
            for (int i = 0; i < CityCount; i++)
            {
                //Startpermutation eintragen
                Permutation.Add(Städte[i + 1].Name);

                for (int i2 = i + 1; i2 < CityCount; i2++)
                {
                    //Routen-Strings werden im Format <AnfangsStadtNr, EndStadtNr> angegeben
                    //String Route1 = (Städte[i + 1].Number).ToString() + (Städte[i2 + 1].Number).ToString();
                    //String Route2 = (Städte[i2 + 1].Number).ToString() + (Städte[i + 1].Number).ToString();

                    //Routen-Strings werden im Format <AnfangsStadtName -> EndStadtName> angegeben
                    String Route1 = Städte[i + 1].Name + " -> " + Städte[i2 + 1].Name;
                    String Route2 = Städte[i2 + 1].Name + " -> " + Städte[i + 1].Name;

                    //Satz des Pythagoras yo
                    double Distanz = 0;
                    Distanz = Math.Sqrt((((Städte[i + 1].X - Städte[i2 + 1].X) * (Städte[i + 1].X - Städte[i2 + 1].X))
                                       + ((Städte[i + 1].Y - Städte[i2 + 1].Y) * (Städte[i + 1].Y - Städte[i2 + 1].Y))));
                    Math.Round(Distanz);

                    //Beide Routen Richtungen werden hinzugefügt, damit später der Algorithmus 
                    //nicht mehr selbst rechnen muss, sondern direkt beide Varianten abfragen kann
                    Distanzen.Add(Route1, Distanz);
                    Distanzen.Add(Route2, Distanz);
                }
            }
            //Erste Stadt erneut einfügen um Rundtrip anzuzeigen
            Permutation.Add(Städte[1].Name);
            //permutation = new Permutation(PermutationFolge);
        }

        private void Distanzen_Anzeigen()
        {
            daten.AusgabeText += "Die Distanzen zwischen den Städten sind:" + Environment.NewLine;
            foreach (KeyValuePair<String, double> entry in Distanzen)
            {
                daten.AusgabeText += (String.Format("Strecke: {0}     Distanz: {1:0}{2}", entry.Key, entry.Value, Environment.NewLine));
            }
        }

        private void Distanzen_Anzeigen_Click(object sender, RoutedEventArgs e)
        {
            Distanzen_Anzeigen();
        }

        private void InitControlList()
        {
            //TODO: Kann dieser Vorgang dynamisch gemacht werden?
            KontrollElementeCity.Add(City1);
            KontrollElementeCity.Add(City2);
            KontrollElementeCity.Add(City3);
            KontrollElementeCity.Add(City4);
            KontrollElementeCity.Add(City5);

            KontrollElementeTextBlock.Add(City1TextBlock);
            KontrollElementeTextBlock.Add(City2TextBlock);
            KontrollElementeTextBlock.Add(City3TextBlock);
            KontrollElementeTextBlock.Add(City4TextBlock);
            KontrollElementeTextBlock.Add(City5TextBlock);
            //KontrollElemente.FirstOrDefault(x => x.  )
            /*
            for (int i = 0; i < Städte.Count(); i++)
            {
               
            }
            */
        }

        //Hilfsfunktion welche Distanz zwischen zwei Städten berechnet
        //könnte später von Algorithmen aufgerufen werden. Das wäre jedoch 
        //im Bezug auf Rechenzeit ineffizient, daher die Distanzen Collection nutzen
        public static double Konkrete_Distanz_Berechnen(City city1, City city2)
        {
            double Distanz = Math.Sqrt((((city1.X - city2.X) * (city1.X - city2.X))
                               + ((city1.Y - city2.Y) * (city1.Y - city2.Y))));
            return Math.Round(Distanz);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tschüss!","Anwendung schließen");
            this.Close();
        }

        private void BtnAusgabeClear_Click(object sender, RoutedEventArgs e)
        {
            daten.AusgabeText = "";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        //Erste Lösungsimplementation - Alle Permutationen durchrechnen
        private void Permutationen_Rechnen()
        {
            Double Kürzeste_Gesamtdistanz = Permutation_Gesamtdistanz_Berechnen(Permutation);
            String Beste_Route = " 1 2 3 4 5 1";
            daten.AusgabeText += "Die Reihenfolge der Städte ist:" + Environment.NewLine;

            //Dies erstellt eine Liste mit allen Permutationen.
            //Bei den Permutationen fehlt die erste Stadt als Start und Endposition.
            //Sie muss daher oben und unten angefügt werden in der Schleife.
            PermutationsTeilListe_Erstellen();

            for (int i = 0; i < Liste.Count(); i++)
            {
                List<String> PermListe = list[i].ToList();

                Permutation.Clear();
                Permutation.Add(Städte[1].Name);
                //Listen verketten
                Permutation.AddRange(PermListe);
                Permutation.Add(Städte[1].Name);

                //Eine Permutation mit Reihenfolge und Distanz anzeigen
                Double Gesamtdistanz = Permutation_Gesamtdistanz_Berechnen(Permutation);
                String Route = "";
                for (int i2 = 0; i2 < CityCount; i2++)
                {
                    Route += Permutation[i2] + " -> ";
                }
                Route += String.Format("{0}    Die Gesamtdistanz ist: {1:0}{2}", Permutation[CityCount], Gesamtdistanz, Environment.NewLine);
                daten.AusgabeText += Route;
                if (Gesamtdistanz < Kürzeste_Gesamtdistanz)
                {
                    Kürzeste_Gesamtdistanz = Gesamtdistanz;
                    Beste_Route = Route;
                }
            }
            //Das beste Ergebnis ausgeben
            daten.AusgabeText += String.Format("Die Beste Route ist:" + Environment.NewLine + Beste_Route + Environment.NewLine);
        }

        private double Permutation_Gesamtdistanz_Berechnen(List<String> Liste)
        {
            Double Gesamtdistanz = 0;
            for (int i = 0; i < CityCount; i++)
            {
                String Strecke = Liste[i] + " -> " + Liste[i + 1];
                Gesamtdistanz += Distanzen[Strecke];
            }
            return Gesamtdistanz;
        }

        //Stadt 2 bis Stadt N
        private void PermutationsTeilListe_Erstellen()
        {
            CityListe.Clear();
            for (int i = 0; i < CityCount - 1; i++)
            {
            //permutation.Add(Städte[i + 2].Name);
            CityListe.Add(Städte[i + 2].Name);
                //PermutationenListe.Add(Städte[i+2].Name);
            }

            Liste = GetPermutations<String>(CityListe , CityCount - 1);
            list = Liste.ToList();
        }

        //Funktion für die rekursive Rückgabe von Listen
        private static IEnumerable<IEnumerable<T>>
        GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private void BtnSolve_Click(object sender, RoutedEventArgs e)
        {
            Permutationen_Rechnen();
        }

        public class City
        {
            String name = "Bielefeld";
            double number = 66;
            double x = 300;
            double y = 300;

            public City(String name, double number, double x, double y)
            {
                this.Name = name;
                this.Number = number;
                this.X = x;
                this.Y = y;
            }

            public double Number { get => number; set => number = value; }
            public string Name { get => name; set => name = value; }
            public double X { get => x; set => x = value; }
            public double Y { get => y; set => y = value; }
        }
    }





    public static class HilfsMethoden
    {
        //https://stackoverflow.com/questions/4214155/wpf-easiest-way-to-move-image-to-x-y-programmatically
        //Funktion für Animation, immer noch buggy, images haben am Ende vertauschte X und Y Werte
        //Es wird außerdem immer der Wert als Ursprung übertragen, der beim setzen des Hakens gegeben war

        public static void MoveTo(this UIElement target,double oldX, double oldY, double newX, double newY)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(oldY, newY - oldY, TimeSpan.FromSeconds(3));
            DoubleAnimation anim2 = new DoubleAnimation(oldX, newX - oldX, TimeSpan.FromSeconds(3));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        //ursprünglicher Code
        /*
            var top = oldY;
            var left = oldX;
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(3));
            DoubleAnimation anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(3));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        */

    }

    public class Data : INotifyPropertyChanged
    {
        private String testString = "Jetzt funktioniert Databinding";
        public String TestString
        {
            get { return testString; }
            set { testString = value; NotifyPropertyChanged("TestString"); }
        }

        private String ausgabeText = "Lorem Ipsum Dolores";
        public String AusgabeText
        {
            get { return ausgabeText; }
            set { ausgabeText = value; NotifyPropertyChanged("AusgabeText"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
