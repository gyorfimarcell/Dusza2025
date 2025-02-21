# A programról

A _Kibirod, kolega?_ csapat Klaszter Kezelő programja összekapcsolt számítógépek, azaz klaszterek kezelésére készült.

A program képes:

- Klaszterek [betöltésére vagy létrehozására](../felhasznaloi/klaszter.md#betoltes).
- A klaszterhez tartozó számítógépek [kezelésére](../felhasznaloi/szamitogep.md).
- A kezelt programok [nyílvántartására](../felhasznaloi/program.md).
- A futó folyamatok [irányítására](../felhasznaloi/folyamat.md).

## Kiemelt funkciók

A mi termékünket számos extra funkció különbözteti meg a versenytársaink progrjamjaitól. Ezek közül emeltünk ki néhányat:

### Letisztult felhasználói felület

A korábbi verzióhoz képest a programunk felülete teljes átalakításra került. A menüpontok és tartalmaik új elrendezést kaptak,
ezzel felhasználóbarátabbá téve a navigációt. A vizuális megjelenést is az alapokról újragondoltuk, ezzel egy letisztult, modern
külsőt adva a programnak. Ezen kívül a program képes világos és sötét megjelenés között
[váltani](../felhasznaloi/beallitasok.md#vilagos-sotet-mod), alkalmazkodva a felhasználó izléséhez.

### Grafikonok

A program több oldalán is grafikonokat helyeztünk el, amelyek segítségével a felhasználó könnyedén átláthatja a klaszter jelenlegi állapotát.

### Többnyelvűség

A program felülete több nyelven is elérhető, amik között a felhasználó szabadon [válthat](../felhasznaloi/beallitasok.md#nyelvvalasztas).
Jelenleg **magyar** és **angol** nyelvre került lefordításra, de későbbi frissítésekben egyszerűen bővíthető további nyelvekkel.

### Automata folyamat kezelés

Lehetőség van egy számítógépet [tehermentesíteni](../felhasznaloi/szamitogep.md#tehermentesites-es-torles), vagy a futó program eloszlását
[optimalizálni](../felhasznaloi/szamitogep.md#optimalizacio). Továbbá a program képes a klaszter hibáit észlelni, és ezeket
[automatikusan kijavítani](../felhasznaloi/klaszter.md#helyreallitas).

### Lista megjelenítés

A kilistázott adatok több szempont alapján is [rendezhetőek, valamint szűrhetőek](../felhasznaloi/folyamat.md#szures-es-rendezes), így a
felhasználó a legnagyobb klaszterekben is könnyedén megtalálja a keresett információt. Ezen kívűl több helyen is lehetőség van CSV formátumban
[exportálni](../felhasznaloi/folyamat.md#exportalas) a megtekintett adatokat.

### Naplózás

A program használata során megannyi bekövetkezhető esemény [naplózásra kerül](../felhasznaloi/naplozas.md), így bármikor visszakövethetőek
a klaszterek változásai. Ezek a naplók bármikor megtekinthetőek a programon belül, szűrés- és keresőfunkciókkal ellátva.
