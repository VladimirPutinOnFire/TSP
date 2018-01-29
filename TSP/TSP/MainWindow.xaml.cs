﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TSP 
{
    /*
     * TODO: 
     * -Labels unter den Städten anbringen
     * -Label um Koordinaten ergänzen
     * -DataBinding
     * -Distanz zwischen den einzelnen Städten berechnen
     * -Distanz zwischen den Städten grafisch anzeigen
     * -Neu generierte Städte mit moveTo animieren
     * 
     * */
    public partial class MainWindow : Window
    {
        int CityCount = 5;
        Dictionary<int, City> Städte = new Dictionary<int, City>();
        private List<UIElement> KontrollElementeCity = new List<UIElement>();
        private List<UIElement> KontrollElementeLabel = new List<UIElement>();

        public MainWindow()
        {
            this.Title = "TSP";
            InitializeComponent();
            DataContext = this;
        }

        private void Generieren_Click(object sender, RoutedEventArgs e)
        {
            Städte.Clear();
            Random rnd = new Random();

            for (int i = 0; i < CityCount; i++)
            {
                int Cityindex = i + 1;
                String Cityname = "City" + (Cityindex).ToString();
                
                //Koordinaten um die Größe des Icons anpassen
                int X = rnd.Next(100, 500)-12;
                int Y = rnd.Next(100, 500)-24;

                //TODO: X- und Y-Position random implementieren
                Städte.Add(Cityindex, new City(Cityname, Cityindex, X, Y));
            }
            
            //Label der ersten Stadt mit dem richtigen Namen versehen
            City1Label.Content = Städte[1].Name;

            //Die Zugriffsliste für die UI-Elemente initialisieren
            //Die UI-Elemente an die ausgewürfelten Positionen setzen
            InitControlList();
            Position_Anpassen();
        }

        public void Position_Anpassen()
        {
            //erstmal statisch an die XAML Elemente binden. TODO: dynamisch generieren
            //note: ist dynamisch, solange die Listenelemente (UIElement) dynamisch dazukommen)
            for (int i = 0; i < CityCount; i++)
            {
                Canvas.SetTop(KontrollElementeCity[i], Städte[i+1].Y);
                Canvas.SetLeft(KontrollElementeCity[i], Städte[i+1].X);
            }
        }

        private void InitControlList()
        {
            //TODO: Kann dieser Vorgang dynamisch gemacht werden?
            KontrollElementeCity.Add(City1);
            KontrollElementeCity.Add(City2);
            KontrollElementeCity.Add(City3);
            KontrollElementeCity.Add(City4);
            KontrollElementeCity.Add(City5);

            KontrollElementeLabel.Add(City1Label);
            //KontrollElemente.FirstOrDefault(x => x.  )
            /*
            for (int i = 0; i < Städte.Count(); i++)
            {
               
            }
            */
        }

        public class City
        {
            String name = "Bielefeld";
            int number = 66;
            int x = 300;
            int y = 300;

            public City(String name, int number, int x, int y)
            {
                this.Name = name;
                this.Number = number;
                this.X = x;
                this.Y = y;
            }

            public int Number { get => number; set => number = value; }
            public string Name { get => name; set => name = value; }
            public int X { get => x; set => x = value; }
            public int Y { get => y; set => y = value; }

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tschüss!","Anwendung schließen");
            this.Close();
        }
    }

    public static class HilfsMethoden
    {
        //https://stackoverflow.com/questions/4214155/wpf-easiest-way-to-move-image-to-x-y-programmatically
        //coole Funktione für Animation, noch ungenutzt

        public static void MoveTo(this Image target, double newX, double newY)
        {
            var top = Canvas.GetTop(target);
            var left = Canvas.GetLeft(target);
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(10));
            DoubleAnimation anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(10));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }
    }
}