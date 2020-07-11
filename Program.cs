using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ogurowo_planting_automation
{
    public class Program {
        static void Main(string[] args) {
            string nasionko = "dynallca";
            var driver = new ChromeDriver();

            //////TESTOWANIE
            ////driver.Navigate().GoToUrl("file:///C:/Users/MrKom/Desktop/farma.html");
            ////string zasiewanie_url = "file:///C:/Users/MrKom/Desktop/farma_sadzenie.html";
            ////string chatka_ogrodnika_url = "file:///C:/Users/MrKom/Desktop/chatka_ogrodnika.html";
            ////string bogactwo_url = "file:///C:/Users/MrKom/Desktop/bogactwo.html";
            ////string statystyki_url = "file:///C:/Users/MrKom/Desktop/statystyki.html";


            //////// ORGINALNY ZESTAW ODNOSNIKOW NA STRONE
            string zasiewanie_url = "https://ogurowo.pl/farm.php?step=plantation&action=sow";
            string chatka_ogrodnika_url = "https://ogurowo.pl/farm.php?step=house";
            string bogactwo_url = "https://ogurowo.pl/zloto.php";
            string statystyki_url = "https://ogurowo.pl/stats.php";
            //////// LOGOWANIE DO KONTA
            driver.Navigate().GoToUrl("https://ogurowo.pl/");
            var element_email = driver.FindElementByName("email");
            element_email.SendKeys("mr.komugiko@gmail.com");
            var element_password = driver.FindElementByName("pass");
            element_password.SendKeys("*****************");
            element_password.SendKeys(Keys.Enter);

            // PRZEJŚCIE NA FARME
            driver.Navigate().GoToUrl("https://ogurowo.pl/farm.php?step=plantation");

            // Znalezienie ilości i linków prowadzących na zbiory posiadanych ziół
            start:
            Console.WriteLine("Udało się przejść na farme");
            var Lista_upraw = PobierzListeUpraw();
            Console.WriteLine("Posiadasz na niej:" + Lista_upraw.Count + " upraw.");
            foreach (var uprawa in Lista_upraw) {
                Console.WriteLine($"nr. {uprawa.Id + 1}, {uprawa.Nazwa}, ilość: {uprawa.Ilosc}, wiek: {uprawa.Wiek}.");
            }
            menuswitch:
            Console.WriteLine("Wybierz opcje:");
            Console.WriteLine("[1] Zmiana Miasta                        [W.I.P]");
            Console.WriteLine("[2] Sprawdz czy zbiory są możliwe        [OK]");
            Console.WriteLine("[3] Zasiej wszystko                      [OK]");
            Console.WriteLine("[4] Ile mam energi?                      [OK]");
            Console.WriteLine("[5] Zamknij                              [OK]");
            int answer = Convert.ToInt32(Console.ReadLine());
            switch (answer) {
                case 1:
                    //TUTAJ ZMIENISZ MIASTO
                    Console.WriteLine("Case 1");
                    break;
                case 2:
                    mozliwe_do_zbioru();
                    goto start;
                //break;
                case 3:
                    Zasiewanie_pola();
                    goto start;
                //break;
                case 4:
                    Console.WriteLine($"Mam: {OdczytAktualnejEnergii()} energi");
                    goto menuswitch;

                case 5:
                    driver.Quit();
                    break;
                default:
                    Console.WriteLine("niepoprawny wybór opcji 1-5.");
                    goto menuswitch;

            }

            
            List<Uprawa> PobierzListeUpraw() {
                int counter = 0;
                var elements_uprawy_link = driver.FindElementsByXPath("//a[contains(text(), 'illani') or contains(text(), 'dynallca') or contains(text(),'illanias') or contains(text(), 'nutari')]");

                List<Uprawa> Lista_upraw = new List<Uprawa>();
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
                //var elements_uprawy_link = driver.FindElements(By.LinkText("illani"));

                //  TODO: [DONE] Rozwiązanie problemu innych nazw roślin -> zła kolejność id podczas podsumowania,
                //         co przeklada sie na indeksacje i szukanie włąściwego odnośnika do zbierania
                //         i przypisanej do niego ilości roślin.

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
                return Lista_upraw;
            }

            void mozliwe_do_zbioru() {
                Console.WriteLine("Mozna zebrac nastepujace ziółka");
                foreach (var uprawa in Lista_upraw.Where(wiek => wiek.Wiek >=23)) {
                    Console.WriteLine($"[ID: {uprawa.Id + 1}], {uprawa.Nazwa}, ilość: {uprawa.Ilosc}, wiek: {uprawa.Wiek}.");
                }
                Console.WriteLine("Wprowadz id ziela do zebrania.[Wybierz \"0\" aby wrócić]");
                int collect_by_id = Convert.ToInt32(Console.ReadLine());
                if (collect_by_id != 0) {
                    Uprawa ziolo = Lista_upraw.Find(id => id.Id == collect_by_id - 1);
                    driver.Navigate().GoToUrl(ziolo.Odnosnik.ToString());
                    var element_input_amount = driver.FindElementByName("amount");

                        int polaDoZebrania = 0;
                        // Decyzja czy zbieramy wszystko czy tylko kilka sztuk:
                        Console.WriteLine("Chcesz zebrać wszystkie posadzone uprawy ? t/n");
                        var answer = Console.ReadLine();
                        if(answer.ToUpper() == "T") {
                            element_input_amount.SendKeys(ziolo.Ilosc.ToString());
                        } else {
                            Console.WriteLine("Podaj pożądaną liczbe pól któe chcesz zebrac:");
                            polaDoZebrania = Convert.ToInt32(Console.ReadLine());
                            element_input_amount.SendKeys(polaDoZebrania.ToString());
                        }

                    element_input_amount.SendKeys(Keys.Enter);


                    // Odczytanie wiadomości o powoodzeniu zbierania tj. liczba nasion exp i doswiadczenie zielarstwa
                    var liczbaZebranychZiol = driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr[2]/td[2]/table/tbody/tr[2]/td/div/b[1]")).Text;
                    var zdobytaUmiejetnoscZielarstwa = driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr[2]/td[2]/table/tbody/tr[2]/td/div/b[2]")).Text;
                    var zdobytePunktyDoswiadczenia = driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr[2]/td[2]/table/tbody/tr[2]/td/div/b[3]")).Text;

                    Console.WriteLine($"Wykorzystana energia: {polaDoZebrania * 1.5}");
                    Console.WriteLine($"Zebrałeś {ziolo.Ilosc.ToString()} zyskując przytym: " +
                        $"\n\t\t\t{liczbaZebranychZiol} ziół {ziolo.Nazwa}," +
                        $"\n\t\t\t{zdobytaUmiejetnoscZielarstwa} umiejetnosci zielarstwa," +
                        $"\n\t\t\t{zdobytePunktyDoswiadczenia} punktów doświadczenia. ");
                }
            }
            void Zasiewanie_pola() {
                //TODO: Wybieranie rodzaju rosliny aktualnie ustawiona sztywna wartosc - Dynallca
                //wybranie konkretnego ziola: np Dynallca

                //TODO: [DONE] Sprawdzanie czy posiada się wystarczającą ilość nasion

                //TODO: [DONE] Weryfikowanie czy nie zabraknie energii i sianie jest możliwe

                Console.WriteLine($"Rozpoczęty proces zasiewania");
                driver.Navigate().GoToUrl(zasiewanie_url);
                checkSeedsAmout:
                // TODO: [DONE] Ustalenie pojemności farmy w celu jej ponownego zasiania
                var element_input_amount = driver.FindElementByName("amount");
                var elements_dostepne_pola = driver.FindElements(By.XPath("//td/ul/li/b"));
                element_input_amount.SendKeys(elements_dostepne_pola[1].Text);

                //wybieranie z listy rozwijanej wybranego rodzaju nasion do zasadzenia
                //wybranie konkretnego ziola: np Dynallca
                 nasionko = nasionko;
                var select_seed_element = driver.FindElement(By.XPath($"//select/option[@value='{nasionko}_seeds']"));
                select_seed_element.Click();
                var posiadaneNasionka = select_seed_element.Text.Substring(select_seed_element.Text.IndexOf("(") + 1);
                posiadaneNasionka = posiadaneNasionka.Substring(0, posiadaneNasionka.IndexOf(")"));
                int posiadaneNasionka_int = Convert.ToInt32(posiadaneNasionka);
                Console.WriteLine($"Posiadasz aktualnie {posiadaneNasionka} nasion {nasionko}");
                int brakujacaLiczbaNasion = Convert.ToInt32(elements_dostepne_pola[1].Text) - posiadaneNasionka_int;
                if (Convert.ToInt32(elements_dostepne_pola[1].Text) > posiadaneNasionka_int) {
                    Console.WriteLine($"Niewystarczająca liczba nasionek. Brakuje {brakujacaLiczbaNasion}szt");
                    suszenie_na_nasiona(brakujacaLiczbaNasion,nasionko);
                    goto checkSeedsAmout;
                } else 
            //sprawdzanie czy masz tyle energii
                if ((OdczytAktualnejEnergii() - (float.Parse(elements_dostepne_pola[1].Text)* 0.2)) > 0) {
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
                return energia;
            }

            void suszenie_na_nasiona(int liczba_nasion,string nazwa_rosliny) {
                //Przejscie do ogrodnika
                driver.Navigate().GoToUrl(chatka_ogrodnika_url);
                //wybranie roslin do wysuszenia
                var select_seed_element = driver.FindElement(By.XPath($"//select/option[@value='{nazwa_rosliny.ToLower()}']"));
                select_seed_element.Click();
                //wprowadzenie ilosc nasion do pozyskania z założeniem 25% zwiększonego nakładu z racji niepowodzenia podczas suszenia
                var element_input_amount = driver.FindElementByName("amount");
                element_input_amount.SendKeys(Math.Round(liczba_nasion * 1.25).ToString());
                //sprawdzenie czy posiadamy wystarczajaca ilosc nasion ( 1 nasionko =  10nasion )
                //pobranie wartosci posiadanych ziol z wczesniejw ybranej listy
                var posiadaneZiola = select_seed_element.Text.Substring(select_seed_element.Text.IndexOf("(")+1);
                posiadaneZiola = posiadaneZiola.Substring(0,posiadaneZiola.IndexOf(")"));
                int posiadaneZiola_int = Convert.ToInt32(posiadaneZiola);
                //porownanie z wartoscia ktora chcemy uzyskac
                if (Math.Round(liczba_nasion * 1.25) * 10 > posiadaneZiola_int) {
                    Console.WriteLine("Posiadasz za mało ziół zeby uzyskać z nich wystarczającą ilośc nasion.");
                    Console.WriteLine("Chcesz spróbowac zasiac inne ziolka ? T/N");
                    var answer = Console.ReadLine().ToLower();
                    if (answer == "t") {
                        Console.WriteLine("Wybierz rosline ktora chcesz posadzic: \n1. Illani\n2. Illanias\n3. Nutari\n4. Dynallca.");
                        int answer2 = Convert.ToInt32(Console.ReadLine());
                        switch (answer2) {
                            case 1:
                                nasionko = "illani";
                                Console.WriteLine($"Postanowiłeś zasadzić {nasionko}. ");
                                suszenie_na_nasiona(liczba_nasion, nasionko);
                                break;
                            case 2:
                                nasionko = "illanias";
                                Console.WriteLine($"Postanowiłeś zasadzić {nasionko}. ");
                                suszenie_na_nasiona(liczba_nasion, nasionko);
                                break;
                            case 3:
                                nasionko = "nutari";
                                Console.WriteLine($"Postanowiłeś zasadzić {nasionko}. ");
                                suszenie_na_nasiona(liczba_nasion, nasionko);
                                break;
                            case 4:
                                nasionko = "dynallca";
                                Console.WriteLine($"Postanowiłeś zasadzić {nasionko}. ");
                                suszenie_na_nasiona(liczba_nasion, nasionko);
                                break;

                            default:
                                Console.WriteLine("niepoprawny wybór opcji 1-4.");
                                break;
                        }

                    }
                    // Tutaj przekieruje na strone farmy z pytaniem któe ziolka chce sie sadzic
                    driver.Navigate().GoToUrl(zasiewanie_url);
                } else {
                    //zatwierdzenie przyciskiem wysusz
                    var element_btn_wysusz = driver.FindElementByXPath("//input[@value='Wysusz']");
                    element_btn_wysusz.Click();
                    //suszenie zielska do czasu uzykania wymaganej ( podanej w parametrze) liczby nasion
                    driver.Navigate().GoToUrl(zasiewanie_url);
                }
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
             * TODO: [DONE] suszenie zielska u ogrodnika
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