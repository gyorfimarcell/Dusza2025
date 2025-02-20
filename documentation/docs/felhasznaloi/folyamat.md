# Folyamat

A futó programok egy-egy példányát egy-egy folyamat jelenti. A klaszteren lévő folyamatokat fájlokként tároljuk, melyek különböző mappákban találhatók. Például:

*`*screenshot*`*<br>

Ezek a fájlok a folyamatok futásának helyét mutatják, azaz azt, hogy mely gépeken zajlanak. Fontos, hogy egy adott program több példányban is futtatható egy számítógépen (mappában) a klaszteren belül.<br>
A fájlok neve a következő formátumban jelenik meg: `<programnev> - <random 6 karakter>`, és minden névnek egyedinek kell lennie a klaszteren. Az egyes fájlok tartalmazzák a folyamat aktuális adatait, például:

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Az első sor a folyamat indításának pontos időpontját jelzi, yyyy.mm.dd hh:mm:ss formátumban.

## Erőforrásigény

A folyamatok erőforrásigényei, mint például a processzor- és memóriahasználat, alapvetően meghatározzák a rendszer működésének hatékonyságát. Mivel a klaszteren több gép is dolgozik, minden egyes futó folyamat egyedi fájlban tárolja az általa használt erőforrások adatait. A fájlokban található információk közül az <b>3. és 4. sor</b> adja meg az adott folyamat, processzor és memória szükségletét.

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Jelen esetben 100 millimag processzorra és 100 MB memória erőforrásra van szükége.

## Állapot

Minden egyes folyamat egy fájlban, a <b>2. sorban</b> tárolja az aktuális állapotát, amely lehet `AKTÍV` vagy `INAKTÍV`. Az `AKTÍV` állapot azt jelzi, hogy a folyamat aktívan fut és végrehajtja a feladatát, míg az `INAKTÍV` állapot azt mutatja, hogy a folyamat valamilyen okból nem működik, nem hajt végre műveleteket, vagy leállt. Utóbbi esetben az adott folyamat <b>nem vesz el erőforrást</b> a gazdagéptől.

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Jelen esetben a folyamat éppen `AKTÍV`, erőforrást emészt fel.<br><br>
Az állapot <b>folyamatonként állítható</b>, ezt a folyamat mellett lévő `[Play]/[Pause]` gombbal tehetjük meg.<br>
*`*screenshot*`*<br>
Egy folyamatot azonban csak akkor aktiválható, ha a gazdagépen van elég erőforrás annak aktiválására, egyébként hibát dob.<br>
*`*screenshot*`*<br>

## Szűrés és rendezés

### Szűrés
A folyamatok kilistázásakor lehetőség van különböző szűrők alkalmazására, amelyek segítenek a keresett információk gyorsabb megtalálásában.<br>
*`*screenshot*`*<br>
A szűrés végezhető <b>programtípusok szerint</b>, lehetővé téve, hogy csak a kívánt típusú alkalmazások jelenjenek meg. Ezen kívül az <b>állapot szerinti szűrés</b> is elérhető, így könnyen megtalálhatók az aktív vagy inaktív folyamatok. Egy szövegmező segítségével <b>név alapján is szűrhetjük a listát</b>, így gyorsan rátalálhatunk egy adott folyamatra.<br>

### Rendezés
A lista rendezhető többféle szempont szerint, például programtípus, azonosító (ID), memória- és processzorhasználat, valamint indítási idő szerint, akár növekvő, akár csökkenő sorrendben, hogy a legfontosabb vagy leginkább releváns adatokat könnyen át lehessen tekinteni.<br>
*`*screenshot*`*

## Exportálás

## Új futtatása

## Leállítás

## Egyéb információk