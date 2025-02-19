# A klaszter

A feladat során a számítógépeket mappák, a futó folyamatokat pedig fájlok fogják szimbolizálni. Például a klaszter1 mappa tartalma a következő:

```.
├── .klaszter
├── szamitogep2
│ ├── .szamitogep_konfig
│ ├── chrome-asdefg
│ └── word-jklbnm
└── szamitogep1
├── .szamitogep_konfig
└── chrome-qwerty
```

Ebben az esetben a klaszterünkhöz jelenleg 2 darab számítógép tartozik: szamitogep1 és szamitogep2. Az elsőn fut egy chrome és egy word programpéldány (vagyis folyamat), a másodikon pedig csupán egy chrome. A .szamitogep_konfig fájloktól eltekintve tehát a fájlok létezése azt jelenti, hogy egy folyamat (vagyis egy konkrét programból egy példány) éppen fut az adott számítógépen. Egy mappát pedig akkor tartunk a klaszterhez tartozó számítógépnek, amennyiben létezik benne a .szamitogep_konfig fájl.

## Tulajdonságok

A futó folyamat egy konkrét programból futó példányt jelent. A klaszteren fuƩatandó
folyamatokat fájlként reprezentáljuk, melyek a fent említeƩ mappákban helyezkednek el
például az alábbi módon:

```.
├── chrome-asdefg
└── word-jklbnm
```

A fájlok és elhelyezkedésük tehát azt szimbolizálják, hogy az adoƩ folyamatok mely számítógépen futnak. Fontos kiemelni, hogy egy program több példányban is futhat a klaszteren akár 1 számítógépen (mappában) is. Egy folyamat neve programnev>- random 6 karakter> melynek egyedinek kell lennie az egész klaszteren. Az ehhez tartozó fájl tartalmazza a folyamat adatait. Például:

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Az első sorban az adott folyamat indításának pontos ideje található yyyy.mm.dd hh:mm:ss
formátumban. Ezt követően a folyamat állapota, mely az „AKTÍV” és „INAKTÍV” szavak valamelyike
aszerint, hogy a folyamat éppen rendeltetésszerűén működik-e vagy sem. A 3. és 4. sorban a
folyamat által aktuálisan használt processzor és memória erőforrás található.

## Betöltés

A program megynitása után több lehetőságünk van:

1. Egy már jelenlegi klaszter kiválasztása
2. Új klaszter generálása

### Meglévő klaszter kiválasztzása

Klaszter kiválasztása során figyelni kell arra, hogy a kiválasztott mappában szerepeljen a .klaszter file.<br>
Amennyiben ennek nem teszünk eleget, arra a program figyelmeztetni fog minket:

### Új generálása

Van lehetőségünk új klaszter generálására is, ahol a következőket kell megadni:

1. Számítógépek száma
2. Kívánt programok száma
3. Futtandó folyamatok száma
4. Klaszer generálásának helye

Ezután a program a fentiek alapján legenerálja nekünk a megfelelő klasztert és igénybe vehetjük a program többi részét.

## Állapot

A klaszter betöltődése után a klaszter jelenlegi állapotáról kapunk egy képet. Két lehetőség van:

1. A klaszter állapota megfelelő
2. Nem felel meg a klaszter által megkövetelt követelményeknek

## Helyreállítás

Amennyiben a klaszter állapota nem megfelelő, lehetőség van a hibákat a programmal helyreállítani, és az állapot újra megfelelő lesz.

## Egyéb információk

- A program megnyitása során, amennyiben a program a naplózási előzményekben talál egy már megnyitott klasztert, automatikusan azt fogja megnyitni.
- A klaszter nevére kattintva a program megnyitja a fájlkezelőt a klaszter mappájának helyén, amennyiben nincs klaszter megnyitva, akkor csak simán a fájlkezelő nyílik meg.
