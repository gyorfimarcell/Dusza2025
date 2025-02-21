# Program működése

## Klaszter állapota

Ezen az oldalon részletesen megtekinthető a klaszter aktuális állapota. Amennyiben a klaszter állapota nem megfelelő – például hiányzó vagy túlzott számú aktív folyamat miatt –, a rendszer jelzi a problémát. Ebben az esetben lehetőség van arra, hogy a program automatikusan javítsa a felmerülő hibákat, biztosítva ezzel a klaszter stabil és zavartalan működését.

## Számítógépek

A rendszerben a számítógépek egy-egy mappával vannak szimbolizálva, erőforrásaikat pedig a `.szamitogep_konfig` fájl határozza meg, amely a memória- és processzorkapacitást tartalmazza. A számítógépek a rendelkezésre álló programokat az erőforrásaik határáig képesek futtatni.

### 1. **Áttekintés és részletek**
A számítógépek listájában megtekinthető azok neve, aktuális terheltsége és kapacitása. Egy adott számítógépre kattintva részletes adatok érhetők el, beleértve a futó programokat és a terheltség grafikonját.

### 2. **Tehermentesítés és törlés**
Egy számítógép törlése előtt tehermentesíteni kell azt, vagyis a rajta futó folyamatokat más gépekre kell áthelyezni. Ha nincs elegendő erőforrás a folyamatok teljes átcsoportosítására, a rendszer csak részleges tehermentesítést végez.

### 3. **Optimalizáció**
Lehetőség van a futó folyamatok elosztásának optimalizálására, hogy a számítógépek terhelése egy meghatározott intervallumon belül maradjon. Ha ez nem lehetséges, a rendszer egyenletes elosztást ajánl fel.

### 4. **Exportálás és szűrés**
A számítógépek adatai CSV formátumban exportálhatók, és lehetőség van különböző szempontok szerinti szűrésre, például processzor- vagy memóriahasználat alapján.

### 5. **Új számítógép hozzáadása és szerkesztése**
Új számítógép létrehozásakor meg kell adni annak nevét, processzorkapacitását és memóriáját. A meglévő gépek kapacitása módosítható, de a beállított értékeknek meg kell haladniuk az aktuális terheltséget.

## Folyamatok

### Folyamat Állapota
- **Aktív:** A folyamat aktívan fut és erőforrást használ.
- **Inaktív:** A folyamat nem fut, így nem használ erőforrást.

### Folyamat Részletek
A `Folyamatok` fülre kattintva megtekinthetjük az aktív és inaktív folyamatokat, grafikonokkal, amelyek segítenek a rendszer terheltségének vizualizálásában. Különböző szűrőeszközökkel gyorsan kereshetünk programtípusok, állapotok vagy nevek alapján.

### Erőforrásigény
Minden folyamat egyedi erőforrásigénnyel rendelkezik, amelyet a fájl harmadik és negyedik sora tartalmaz (processzor és memória használat). A rendszer figyeli a gépek erőforrásait, hogy biztosítsa a folyamatok zavartalan futását.

### Szűrés és Rendezés
A folyamatok listáját szűrhetjük programtípus, állapot vagy név alapján. Emellett rendezhetjük az adatokat processzorhasználat, memóriahasználat és indítási idő szerint.

### Exportálás
A folyamatok adatainak CSV formátumban történő exportálása lehetővé teszi a további elemzést. Az exportálás előtt megadhatjuk a kívánt mentési helyet.

### Új Folyamat Futtatása
Új folyamat indítása a `Folyamatok` fül jobb felső sarkában található `[Új]` gombbal történik. A rendszer ellenőrzi az erőforrást, és biztosítja, hogy a folyamat csak akkor induljon, ha a gazdagépen elegendő erőforrás áll rendelkezésre.

### Leállítás
A folyamatok könnyen leállíthatók a folyamatok mellett található `Kuka(🗑️)` ikonra kattintva. A rendszer automatikusan frissíti a klaszter állapotát a folyamat leállításakor.

### Egyéb információk
- A programok nevei gyakran tartalmazhatnak rejtett utalásokat vagy easter egg-eket.
- Inaktív folyamatok nem használják az erőforrást, de a rendszerben maradhatnak, anélkül, hogy hatással lennének a működésre.

## Programok

A klaszteren futtatható programok a `.klaszter` fájlban vannak definiálva, és tartalmazzák az egyes programok beállításait, például a futtatásukhoz szükséges erőforrásokat. Minden programnak rendelkeznie kell a megfelelő paraméterekkel ahhoz, hogy a rendszer stabilan működjön.

### Elvárt futási igények
- **Program neve**: Egyedi név szükséges a program azonosításához.
- **Futtatandó példányok száma**: Meg kell határozni, hány példányban fusson a program.
- **Processzorigény**: A programnak előre meg kell adnia a szükséges processzor- vagy processzormagot.
- **Memóriaigény**: A program számára lefoglalt memória is előre meghatározandó.

### Részletek
A programok oldalon egy lista és egy grafikon segít a programok és erőforrások átláthatóságában. Az adatok tartalmazzák a program nevét, processzor- és memóriaigényét, valamint az aktív folyamatok számát.

### Létrehozás
Új program létrehozásához meg kell adni a program nevét, aktív példányszámát, processzor- és memóriaigényét.

### Módosítás
A programok paraméterei, mint az aktív folyamatok száma, processzorigény és memóriaigény módosíthatók.

### Törlés
A program törlésével a hozzá kapcsolódó összes folyamat leáll, és a program eltávolításra kerül a rendszerből.

### Rendezés és szűrés
A programok rendezhetők és szűrhetők név, erőforrásigény, futtatott példányszám és státusz szerint.

### Egyéb információk
A programok leállíthatók, akár ha egyetlen példányuk sem fut.

## Klaszter naplók

A rendszer naplózási funkciója rögzíti a fontos eseményeket a klaszter működésének nyomon követésére. Az események típusát, időpontját és egyéb részleteit egy `.log` kiterjesztésű fájlban tárolja, melyek neve az aktuális dátum alapján jön létre (pl. `yyyyMMdd`). Az új események hozzáfűződnek a meglévő fájlokhoz, vagy új fájl készül, ha az adott napra még nem létezik.

### Működése

Minden esemény után automatikusan naplózási folyamat fut, és az események a "Klaszter naplók" oldalon érhetők el. Az oldalon az események részleteit egy kártya lista formájában lehet megtekinteni, melyek kibővítésével további információk válnak elérhetővé.

A rendszer az alábbi eseményeket naplózza:

- Programok megnyitása és bezárása
- Klaszter és számítógép kezelése
- Programok hozzáadása, módosítása, leállítása
- Folyamatok indítása, leállítása és optimalizálása
- Klaszterhibák javítása

### Szűrés

A naplózott események szűrhetők eseménytípus, részletek és keresett érték alapján. A szűrési lehetőségek közé tartozik:

- Eseménytípus szerinti szűrés
- Részletek szerinti szűrés (pl. program vagy számítógép neve)
- Szöveges keresés a naplóbejegyzésekben

### Egyéb információk

- A naplófájlok automatikusan generálódnak és folyamatosan frissülnek.
- A rendszer akár 10.000 soros naplófájlokat is képes kezelni, de nagy fájlok esetén lassulhat a működés.
- Vannak olyan események, amelyek nem tartalmaznak további információkat, így azok nem szűrhetők részletesebben (pl. applikációk megnyitása).



## Beállítások

A Beállítások oldal lehetőséget ad a következő alapvető tulajdonságok módosítására: nyelv és téma.

### Világos - Sötét mód
Alapértelmezés szerint az alkalmazás sötét módban jelenik meg. Azonban választható világos és sötét módok között, így az alkalmazás színei ennek megfelelően változnak.

### Nyelvválasztás
Alapértelmezett nyelv a magyar, de az alkalmazás angol nyelvű használatára is van lehetőség.

