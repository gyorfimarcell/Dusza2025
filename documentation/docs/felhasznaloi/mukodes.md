# Program működése

## Klaszter állapota
A klaszter aktuális állapota nyomon követhető. Ha probléma merül fel (pl. túlzottan sok vagy kevés aktív folyamat), a rendszer jelzi és automatikusan javítja.

## Számítógépek
A számítógépek `.szamitogep_konfig` fájllal rendelkeznek, amely tartalmazza az erőforrásaikat.

- **Áttekintés**: A számítógépek listáján látható a terheltség és kapacitás.
- **Tehermentesítés**: A számítógép törlése előtt tehermentesíteni kell, ha nincs elég erőforrás, részleges tehermentesítés történik.
- **Optimalizálás**: A rendszer optimalizálja a folyamatok elosztását, ha szükséges.
- **Exportálás és szűrés**: Adatok exportálhatók CSV-ben, szűrhetők processzor- és memóriahasználat alapján.
- **Új számítógép hozzáadása**: Új számítógép adatainak megadása szükséges.

## Folyamatok

- **Állapot**: Aktív (fut) vagy inaktív (nem fut).
- **Részletek**: Az aktív és inaktív folyamatok listázása grafikonokkal.
- **Erőforrásigény**: Minden folyamatnak meghatározott processzor- és memóriaigénye van.
- **Szűrés és rendezés**: A folyamatok szűrhetők típus, állapot, név és erőforrásigény szerint.
- **Új folyamat indítása**: A rendszer ellenőrzi az erőforrást, mielőtt elindítja a folyamatot.
- **Leállítás**: Folyamatok könnyen leállíthatók.

## Programok

- A programok a `.klaszter` fájlban vannak definiálva, beleértve azok futtatási igényeit.
- **Elvárt igények**: Program neve, példányszám, processzor- és memóriaigény.
- **Létrehozás és módosítás**: Programok neve, példányszám, erőforrásigény beállítható.
- **Törlés**: A program törlésével minden hozzá tartozó folyamat leáll.
- **Szűrés és rendezés**: Programok szűrhetők név, erőforrásigény és állapot szerint.

## Klaszter naplók
A naplófájlok `.log` kiterjesztéssel kerülnek tárolásra, az események időpontja alapján.

- **Szűrés**: Eseménytípus, részletek és szöveges keresés szerint.
- **Naplók kezelése**: A rendszer naplózza a programok indítását, leállítását és a klaszter kezelési eseményeit.

## Beállítások

- **Világos - Sötét mód**: Az alkalmazás alapértelmezett sötét módban jelenik meg, de választható világos és sötét mód is.
- **Nyelvválasztás**: Az alkalmazás magyar nyelvű, de angol nyelvre is váltható.
