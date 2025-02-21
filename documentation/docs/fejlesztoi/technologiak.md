# Használt technológiák

## Program

### C\#

A program elkészítésére a .NET keretrendszert választottuk. A C# erős típusú nyelv, ami csökkenti a fejlesztés során
keletkező hibákat. A nyelv objektum-orientált felépítése lehetővé teszi az adatszerkezetek rendezett kezelését,
valamint megkönnyíti a program részegységekre bontását. Ezen kívül a LINQ eszközökkel az adatainkat hatékonyan szűrhetjük
és alakíthatjuk, megkönnyítve a különböző oldalak elkészítését.

### Windows Presentation Foundation

A grafikus felület a Windows Presentation Foundation keretrendszer (rövidebb nevén WPF) segítségével készült.
A WPF különböző elrendezési lehetőségeket kínál, például tábla elrendezést vagy rács elrendezést, amelyek segítségével
könnyen kialakítható az alkalmazás felhasználói felülete. Erős adatkötési funkciókkal rendelkezik, amelyek lehetővé
teszik az adatok dinamikus megjelenítését és frissítését a felhasználói felületen. Továbbá lehetővé teszi a stílusok és
sablonok alkalmazását, amelyek segítségével egyszerűen testre szabható az alkalmazás megjelenése.

### WPF UI

A program megjelenésének alapját a `WPF-UI` package adja. Ez a Microsoft által kifejlesztett Fluent 2 design nyelvet
valósítja meg WPF-ben. Számos Windows rendszeralkalmazás követi ezeket az irányelveket. A Fluent 2 észtétikailag vonzóbbá
és felhasználóbarátabbá teszi az alkalmazást. Ezen kívül rengeteg, egységes stílusú ikont is tartalmaz.

A kiegészítő az MIT licenc feltételei szerint használható.

### LiveCharts 2

A `LiveCharts` egy több keretrendszerhez is elérhető library, ami különböző grafikonok megjelenítését teszi lehetővé.
Számos grafikon típust tartalmaz, beleértve az általunk használt kör- és oszlopdiagrammokat is. Mindezt egyszerűen testreszabható,
esztétikus, és teljesítmény-kímélő módon biztosítja.

A kiegészítő az MIT licenc feltételei szerint használható.

## Fejlesztői eszközök

### Git

A számos verziókezelő rendszer közül a Git-et választottuk, mivel ez a legelterjedtebb megoldás, és mindannyian ismertük.
A Git lehetővé teszi, hogy minden változtatást nyomon kövessünk és dokumentáljunk. A különböző ágak használata megkönnyítette
a párhuzamos munkavégzést, és segített elkerülni, vagy szükség esetén elhárítani a konfliktusokat.  
A [Fejlesztés menete](fejlesztes.md) oldalon részletesebben leírjuk, hogy hogyan használtunk verziókezelést a projekt során.

### Visual Studio

A fejlesztés főként a Visual Studio fejlesztői környezetben történt. A visual studio számos eszközt kínál, amely megkönnyíti
a C# kód írását. A beépített Designer ablak gyorsabbá teszi a felhasználói felület elkészítését. Ezen kívül lehetővé teszi a
hibák keresését, és a program teljesítményének tesztelését.

## Dokumentáció

### MkDocs

A dokumentáció elkészítéséhez az MkDocs statikus weboldal generátort használtuk. Ez a program Markdown fájlokból készít weboldalt.
A számos generátor közül az MkDocs-ra esett a választás, mivel könnyen kezelhető, a HTML sablonok egyszerűen testreszabhatóak,
valamint több, dokumentáció specifikus funkciót tartalmaz.

### Material for MkDocs

Ez egy MkDocs sablon, ami egy modern, reszponzív megjelenést biztosít a dokumentációnknak. Ezen kívül további funkciókkal is kibővíti
a weboldal generátort, mint például a témaváltás, vagy az offline csomag generálás.

### Github Pages

A dokumentáció a Github Pages szolgáltatás segítségével válik elérhetővé. Ingyenes, a verziókezelő rendszer alapján automatikusan
frissülő weboldal készítését teszi lehetővé.  
A teljes folyamat leírása megtalálható a [Dokumentáció](dokumentacio.md) oldalon.
