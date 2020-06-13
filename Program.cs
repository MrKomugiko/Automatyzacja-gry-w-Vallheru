using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ogurowo_planting_automation
{
    public class Program {
        static void Main(string[] args) {
            var driver = new ChromeDriver();

            //TESTOWANIE
            driver.Navigate().GoToUrl("file:///C:/Users/MrKom/Desktop/farma.html");
            string zasiewanie_url = "file:///C:/Users/MrKom/Desktop/farma_sadzenie.html";
            string chatka_ogrodnika_url = "file:///C:/Users/MrKom/Desktop/chatka_ogrodnika.html";
            string bogactwo_url = "file:///C:/Users/MrKom/Desktop/bogactwo.html";
            string statystyki_url = "file:///C:/Users/MrKom/Desktop/statystyki.html";


            ////////// ORGINALNY ZESTAW ODNOSNIKOW NA STRONE
            //string zasiewanie_url = "https://ogurowo.pl/farm.php?step=plantation&action=sow";
            //string chatka_ogrodnika_url = "https://ogurowo.pl/farm.php?step=house";
            //string bogactwo_url ="https://ogurowo.pl/zloto.php";
            //string statystyki_url ="https://ogurowo.pl/stats.php";
            ////////// LOGOWANIE DO KONTA
            //driver.Navigate().GoToUrl("https://ogurowo.pl/");
            //var element_email = driver.FindElementByName("email");
            //element_email.SendKeys("mr.komugiko@gmail.com");
            //var element_password = driver.FindElementByName("pass");
            //element_password.SendKeys("Kamil1995Pl");
            //element_password.SendKeys(Keys.Enter);

            // PRZEJŚCIE NA FARME
            driver.Navigate().GoToUrl("https://ogurowo.pl/farm.php?step=plantation");

            // Znalezienie ilości i linków prowadzących na zbiory posiadanych ziół
            start:
            int counter = 0;
            //var elements_uprawy_link = driver.FindElements(By.LinkText("illani"));

            //  TODO: [DONE] Rozwiązanie problemu innych nazw roślin -> zła kolejność id podczas podsumowania,
            //         co przeklada sie na indeksacje i szukanie włąściwego odnośnika do zbierania
            //         i przypisanej do niego ilości roślin.
            var elements_uprawy_link = driver.FindElementsByXPath("//a[contains(text(), 'illani') or contains(text(), 'dynallca') or contains(text(),'illanias') or contains(text(), 'nutari')]");

            List <Uprawa> Lista_upraw = new List<Uprawa>();
            //Console.WriteLine($"Znaleziono {elements.Count} aktualnie hodowane zioła Ilani.");
            foreach (var item in elements_uprawy_link) {
                // Console.WriteLine(item.GetAttribute("href").ToString());
                Lista_upraw.Add(new Uprawa() {
                    Id = counter,
                    Odnosnik = item.GetAttribute("href").ToString(),
                    Nazwa = item.Text,
                    Ilosc = 0,
                    Wiek = 0,
                });
                counter++;
            }

            // //ul[2]/li[1]
            counter = 0;
            var elements_uprawy_data = driver.FindElements(By.XPath("//ul[2]/li"));
            foreach (var item in elements_uprawy_data) { // wyszukuje "illani [ Ilość: 13 ] [ Wiek: 11 ] [ Młoda roślina ]"
                //Console.WriteLine(item.Text);
                String testing = item.Text;

                //testing.IndexOf("Ilość");
                //Console.WriteLine("index 'ilość' =" + testing.IndexOf("Ilość"));
                //Console.WriteLine("Przycięty tekst =" + testing.Substring(testing.IndexOf("Ilość") + 7));
                //Console.WriteLine("Przycięty tekst z obu stron =" + testing.Substring(testing.IndexOf("Ilość") + 7).Remove(3));
                string ilosc = testing.Substring(testing.IndexOf("Ilość") + 7).Remove(2).Trim();
                int ilosc_int = Convert.ToInt32(ilosc);

                Lista_upraw.Find(id => id.Id == counter).Ilosc = ilosc_int;

                //testing.IndexOf("Wiek");
                //Console.WriteLine("index 'Wiek' =" + testing.IndexOf("Wiek"));
                //Console.WriteLine("Przycięty tekst =" + testing.Substring(testing.IndexOf("Wiek") + 6));
                //Console.WriteLine("Przycięty tekst z obu stron =" + testing.Substring(testing.IndexOf("Wiek") + 6).Remove(3));
                string wiek = testing.Substring(testing.IndexOf("Wiek") + 6).Remove(2).Trim();
                int wiek_int = Convert.ToInt32(wiek);

                Lista_upraw.Find(id => id.Id == counter).Wiek = wiek_int;
                counter++;
            }

            Console.WriteLine("Udało się przejść na farme");
            Console.WriteLine("Posiadasz na niej:"+Lista_upraw.Count+" upraw.");
            foreach (var uprawa in Lista_upraw) {
                Console.WriteLine($"nr. {uprawa.Id+1}, {uprawa.Nazwa}, ilość: {uprawa.Ilosc}, wiek: {uprawa.Wiek}.");
            }

            Console.WriteLine("Wybierz opcje:");
            Console.WriteLine("[1] Zmiana Miasta                        [W.I.P]");
            Console.WriteLine("[2] Sprawdz czy zbiory są możliwe        [OK]");
            Console.WriteLine("[3] Zasiej wszystko                      [OK]");
            Console.WriteLine("[4] Ile mam energi?                      [OK]");
            Console.WriteLine("[5] Zamknij                              [OK]");

            int answer = Convert.ToInt32(Console.ReadLine());
            if (answer == 2) {
                Console.WriteLine("Mozna zebrac nastepujace ziółka");
                foreach (var uprawa in Lista_upraw.Where(wiek => wiek.Wiek > 18)) {
                    Console.WriteLine($"[ID: {uprawa.Id + 1}], {uprawa.Nazwa}, ilość: {uprawa.Ilosc}, wiek: {uprawa.Wiek}.");
                }
                Console.WriteLine("Wprowadz id ziela do zebrania.");
                int collect_by_id = Convert.ToInt32(Console.ReadLine());

                Uprawa ziolo = Lista_upraw.Find(id => id.Id == collect_by_id - 1);
                driver.Navigate().GoToUrl(ziolo.Odnosnik.ToString());
                var element_input_amount = driver.FindElementByName("amount");
                element_input_amount.SendKeys(ziolo.Ilosc.ToString());
                element_input_amount.SendKeys(Keys.Enter);
                Console.WriteLine($"Wykorzystana energia: {ziolo.Ilosc*1.5}");
                Console.WriteLine($"Zebrałeś {ziolo.Ilosc.ToString()} zyskując przytym: EXP/ILOSC LISCI/DOSWIADCZENIE");
                
                goto start;
            } else if (answer == 3) {
                Zasiewanie_pola();
            } else if (answer == 4) {
                Console.WriteLine($"Mam: {OdczytAktualnejEnergii()} energi");
            } 
            else { 
                driver.Quit(); 
            }

        void Zasiewanie_pola() {
            //TODO: Wybieranie rodzaju rosliny 
            //TODO: Sprawdzanie czy posiada się wystarczającą ilość nasion
            //TODO: [DONE] Weryfikowanie czy nie zabraknie energii i sianie jest możliwe

            Console.WriteLine($"Następuje automatyczne zasianie ostatnio uzywanej rosliny");
            driver.Navigate().GoToUrl(zasiewanie_url);
            var element_input_amount = driver.FindElementByName("amount");
            // TODO: [DONE] Ustalenie pojemności farmy w celu jej ponownego zasiania
            var elements_dostepne_pola = driver.FindElements(By.XPath("//td/ul/li/b"));
            element_input_amount.SendKeys(elements_dostepne_pola[1].Text);

            //sprawdzanie czy masz tyle energii
            if((OdczytAktualnejEnergii() - (float.Parse(elements_dostepne_pola[1].Text)* 0.2)) > 0) {
                element_input_amount.SendKeys(Keys.Enter);
                Console.WriteLine($"Zasiano {elements_dostepne_pola[1].Text} ziol.");
                Console.WriteLine($"Zuzyles {Convert.ToInt32(elements_dostepne_pola[1].Text)*0.2}, pozostało {OdczytAktualnejEnergii()} energii.");
                } else {
                    Console.WriteLine("Nie masz wystarczająco energii.");
                }
        }

        float OdczytAktualnejEnergii() {
        // TODO: [DONE] Pobieranie danych tekstowych zawierających ilośc posiadanej energii.
                var element_tabela_z_danymi = driver.FindElement(By.XPath("/html[1]/body[1]/div[1]/table[1]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[1]/td[1]"));
                string surowy_tekst_z_danymi = element_tabela_z_danymi.Text;
        // TODO: [DONE] Wyodrębnienie energii z tekstu.
                string sformatowany_tekst_energia = surowy_tekst_z_danymi.Substring(surowy_tekst_z_danymi.IndexOf("Energia:") + 9).Trim();
                string energia_txt = sformatowany_tekst_energia.Substring(0, sformatowany_tekst_energia.IndexOf("[i]")).Trim();
                float energia = float.Parse(energia_txt.Replace(".",","));
                Console.WriteLine($"TEST ->{energia}<-");
                return energia;
            }
  
        }
        public class Uprawa {

            public int Id { get; set; }
            public string Odnosnik { get; set; }
            public String Nazwa { get; set; }
            public int? Wiek { get; set; }
            public int? Ilosc { get; set; }

        }
    }
}
            /*
             * TODO: suszenie zielska u ogrodnika
             * TODO: sprawdzanie poziomu zielarstwa i notowanie aktualnych zbiorów
             * TODO: wystawianie / edytowanie aukcji na ryneczku lidla
             * TODO: sporządzanie notatek dot.zbiorów
             * TODO: zmiana miasta
             * TODO: zapisywanie o któej mają być zbiory i w którym mieście
             * TODO: pełne zautomatyzowanie :
                    - sprawdzenie enrgi
                    - sprawdzenie wymaganych ziół
                    - ewentualne wysuszenie potrzebnych ziół
                    - wybranie preferowanej rośliny do uprawy
                    - zasadzenie wszytskich dostępnych miejsc farmy
                    - zanotowanie postępów exp i doswiadczenie zielarstwa
                    - zmiana miasta i powtórzenie procesu
                    - sprawdzenie czy widnieje aukcja na rynku
                    - zaktualizowanie aukcji
            */