# Folyamat

A futó programok egy-egy példányát egy-egy folyamat jelenti. A klaszteren lévő folyamatokat fájlokként tároljuk, melyek különböző mappákban találhatók. Például:

```
.
├── chrome-asdefg
└── word-jklbnm
```

Ezek a fájlok a folyamatok futásának helyét mutatják, azaz azt, hogy mely gépeken zajlanak. Fontos, hogy egy adott program több példányban is futtatható egy számítógépen (mappában) a klaszteren belül. A fájlok neve a következő formátumban jelenik meg: `<programnev> - <random 6 karakter>`, és minden névnek egyedinek kell lennie a klaszteren. Az egyes fájlok tartalmazzák a folyamat aktuális adatait, például:

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Az első sor a folyamat indításának pontos időpontját jelzi, yyyy.mm.dd hh:mm:ss formátumban.

## Erőforrásigény

A folyamatok erőforrásigényei, mint például a processzor- és memóriahasználat, alapvetően meghatározzák a rendszer működésének hatékonyságát. Mivel a klaszteren több gép is dolgozik, minden egyes futó folyamat egyedi fájlban tárolja az általa használt erőforrások adatait. A fájlokban található információk közül az 3. és 4. sor adja meg az adott folyamat, processzor és memória szükségletét.

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Jelen esetben 100 millimag processzorra és 100 MB memória erőforrásra van szükége.

## Állapot

Minden egyes folyamat egy fájlban, a 2. sorban tárolja az aktuális állapotát, amely lehet „AKTÍV” vagy „INAKTÍV”. Az „AKTÍV” állapot azt jelzi, hogy a folyamat aktívan fut és végrehajtja a feladatát, míg az „INAKTÍV” állapot azt mutatja, hogy a folyamat valamilyen okból nem működik, nem hajt végre műveleteket, vagy leállt. Utóbbi esetben az adott folyamat nem vesz el erőforrást a gazdagéptől.

```
2024-10-27 07:15:45
AKTÍV
100
100
```

Jelen esetben a folyamat éppen AKTÍV, erőforrást emészt fel.<br><br>
Az állapot <b>folyamatonként állítható</b>, ezt a folyamat mellett lévő `[Play]/[Pause]` gombbal tehetjük meg.<br>
*`*screenshot*`*<br>
Egy folyamatot azonban csak akkor aktiválható, ha a gazdagépen van elég erőforrás annak aktiválására.


## Rendezés és szűrés

## Exportálás

## Új futtatása

## Leállítás

## Egyéb információk