# Program szerkezeti felépítése

## Adatmodellek

### Számítógép (`Computer.cs`)

Ez az osztály felel a számítógépek kezeléséért, és a `.szamitogep_konfig` fájlok írásáért és olvasásáért.

### Folyamat (`Process.cs`)

Ez az osztály felel a folyamatok kezeléséért, valamit a folyamatfájlok írásáért és olvasásáért.

### Program (`ProgramType.cs`)

Ez az osztály felel a különböző programok kezeléséért. Továbbá ez az osztály olvassa be a `.klaszter` fájlt.

### Napló (`Log.cs`)

Ez az osztály tartalmazza a különbőző naplózott eseményeket, és kezeli a napló fájlok írását.

## Oldalak (`Pages/`)

### Fő ablak (`MainWindow.xaml(.cs)`)

A program fő ablakja. Tartalmazza az ablak gombjait, és a navigációt. Ez az fájl kezeli a klaszter és a beállítások betöltését is.

### Kezdőlap (`StartPage.xaml(.cs)`)

Ez az oldal jelenik meg a program indításakor, amíg nincs betöltött klaszter.

### Klaszer állapot (`ClusterHealthPage.xaml(.cs)`)

Ez az oldal megjeleníti a klaszterben észlelt hibákat.

### Számítógép

#### Számítógépek (`ComputersPage.xaml(.cs)`)

Ez az oldal megjeleníti a klaszter számítógépeit. A lista szűrhető és rendezhető.

#### Számítógép létrehozás (`AddComputerPage.xaml(.cs)`)

Ezen az oldalon új számítógép hozható létre.

#### Számítógép részletek (`ComputerDetailsPage.xaml(.cs)`)

Ez az oldal egy adott számítógép részleteit, és a rajta futó folyamatokat jeleníti meg.

#### Számítógép szerkesztés (`ModifyComputerPage.xaml(.cs)`)

Ezen az oldalon szerkeszthető egy számítógép kapacítása.

### Folyamatok

#### Folyamatok (`ProcessesPage.xaml(.cs)`)

Ez az oldal megjeleníti a klaszterben futó folyamatokat. A lista rendezhető és szűrhető.

#### Folyamat futtatása (`NewInstancePage.xaml(.cs)`)

Ezen az oldalon új folyamat futtatható.

### Programok

#### Programok (`ProgramsPage.xaml(.cs)`)

Ez az oldal megjeleníti a klaszter programjait. A lista rendezhető és szűrhető.

#### Program létrehozás (`AddNewProgramPage.xaml(.cs)`)

Ezen az oldalon új program hozható létre.

#### Program szerkesztés (`ModifyProgramPage.xaml(.cs)`)

Ezen az oldalon módosítható egy program.

### Napló (`ClusterChangeLogsPage.xaml(.cs)`)

Ez az oldal megjeleníti a program naplóit.

### Beállítások (`SettingsPage.xaml(.cs)`)

Ezen az oldalon kiválasztható a nyelv és téma.

### Klaszter generálás (`GenerateClusterPage.xaml(.cs)`)

Ezen az oldalon generálható egy klaszter, megkönnyítve a program tesztelését.

## UI elemek (`Controls/`)

### Alap kártya (`CustomCard.cs`)

Ez a control a programban megtalálató kártyák alapjául szolgál. Ide tartozik a `Themes/Generic.xaml` fájl is.

### Üres állapot (`EmptyStatus.xaml(.cs)`)

A kezdőképernyőn és az üres listákon megjelenő visszajelzés.

### Optimalizálás ablak (`OptimizeDialog.xaml(.cs)`)

A számítógépek optimalizálása során megjelenő felugró ablak.

### Folyamat kártya (`ProcessCard.xaml(.cs)`)

A folyamatok és a számítógép részletek oldalon megjelenő kártyák.

### Erőforrás-használat sáv (`UsageBar.xaml(.cs)`)

A számítógép oldalakon megjelenő, processzor és memória használatot mutató sáv.

## Grafikon modellek (`ChartModels/`)

Ezek az osztályok az oldalakon mejelenő grafikonokhoz szolgáltatják az adatokat.

## I18N

### Fordítás kezelő (`TranslationSource.cs`)

Ez az osztály segédmetódusokat tartalmaz az aktív nyelv változtatásához, és a fordított szövegek lekéréséhez.

### Fordítás fájlok (`Resources/`)

Ebben a mappában találhatóak a lefordított szövegek. A `Strings.resx` fájl az angol szövegeket, a `Strings.hu-HU.resx` fájl
pedig a magyar fordítást tartalmazza. Egy új `.resx` fájl hozzáadásával a program további nyelvekkel bővíthető.

###

## Egyéb fájlok

### Klaszter ellenőrzés (`ClusterHealth.cs`)

Ez az osztály metódusokat tartalmaz a klaszter hibáinak észleléséhez és javításához.

### Egyedi oldal (`CustomPage.cs`)

Ez az osztály a legtöbb oldal ősosztályaként szolgál. Néhány segédmetódust tartalmaz, amire több oldalnak is szüksége van.

### Validáció (`Validate.cs`)

Ez az osztály segédmetódusokat tartalmaz a különböző adatszerkezetek és fájlok ellenőrzéséhez.

### Ikon (`images/icon.ico`)

A program által használt ikon. Ez az Icons8 ingyenesen elérhető ikoncsomagjából származik.
