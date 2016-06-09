# ZMiTAD_TwitterAPI_Application
Autorzy: Patryk Rezler, Fabian Sporek

Aplikacja została zaimplementowana w języku C# w środowisku Visual Studio 2015. GUI aplikacji zbudowane jest z Windows Forms.
Program działa w oparciu o Streaming API,na podstawie przerobionego skryptu (https://github.com/swhitley/TwitterStreamClient), 
który to z kolei bazuje na bibliotece OAuth 2.0. Słuzy ona do nawiązywanie połączenie ze pomocą protokołu HTTP. Aplikacja 
skanuje, zbiera i analizuje z globalnej sieci wszystkie tweety zawierające poszukiwane słowo kluczowe. Aby zbudować program należy:
-posiadać najnowszą platformę .NET Framework oraz Visual Studio w wersji 2015
-wygenerować unikalne tokeny dla prywatnego konta na tweeterze(https://dev.twitter.com/overview/api - zakładka
'manage my apps') oraz wrzucić je do pliku App.config 
-zainstalować bibliotekę zewnętrzną FinMATH (https://www.rtmath.net/products/finmath/), jest ona niezbędna do działania
testu sprawdzającego czy dane pochodzą z rozkładu normalnego (test Shapiro Wilk'a, bazuje na bibliotece Accord)

WAŻNE: w repozytorium znajdują się dwa projekty z czego ten właściwy (i oparty na GUI) nosi nazwię ZMiTAD_WinForms

Funkcjonalności:
-wyszukiwanie dowolnego słowa kluczowego
-obliczanie średniej długości statusów (w znakach) oraz średniej ilości słów w statusie
-możliwość podglądu w czasie rzeczywistym treści analizowanych statusów
-możliwość podglądu historii zaanalizowanych statusów oraz ich poszczegolnych: czasów wychwycenia, ilości znaków i słów
oraz tresci
-sprawdzanie czy długości statusów w znakach pochodza z rozkladu normalnego
-badanie ilosci wystepowania poszczegolnych słów oraz czy ich częstotliwośc występowania ma rozklad normalny
-generowanie wykresów w czasie rzeczywistym: histogramu, ilosci statusów i ilosci znaków oraz słów w określonym przedziale czasowym


