# Számítógép

A számítógépek egy-egy mappával vannak szimbolizálva, míg a számítógép erőforrásait a mappán belül található .szamitogep_konfig fájl írja le. Ezek az erőforrások jelen esetben a számítógép memória- és processzorkapacitását jelentik. A számítógépek képesek a rendelkezésre álló programok közül valamennyi példányt futtatni az erőforrás megfelelő kapacitásáig.

## Erőforrások

Például egy .szamitogep_konfig fájl tartalma a következő lehet:

```
2800
12000
```

Az első sor a processzor kapacitását jelöli egy egész szám formájában, amely a processzormagok darabszámának ezred részét, azaz millimagot ad meg. Például a 2800 érték azt jelenti, hogy a szamitogep2 számítógépen 2,8 processzormag áll rendelkezésre.

A második sor szintén egy egész szám, amely az elérhető memóriakapacitást mutatja MB-ban. Például a 12000 érték azt jelzi, hogy a szamitogep2 számítógép 12 GB memóriát használhat.

## Részletek

A számítógéplistát megnyitva gyors áttekintést kaphatunk a jelenlegi számítógépekről és azok leterheltségéről.

![Számítógép főoldal](../img/felhasznaloi/szamitogep/computer-landing-page.png)<br>

## Tehermentesítés és törlés

Számítógép törlése esetén két opció léphet fel. Az egyik, hogy a kitörlendő számítógépen még vannak futó programok, a másik meg, hogy már nincsen terhelés alatt.

1. Amennyiben egy számítógépen már nincsen futó program, a számítógép minden akármi plusz lépés nélkül törölhető.
   ![Számítógép törlés sikeres](../img/felhasznaloi/szamitogep/computer-deleted-success.png)<br>

2. Ha egy számítógépet futó programokkal akarunk eltávolítani, akkor előtte **tehermentesítenünk** kell, erre két lehetőségünk van:
    - a számítógépet tehermentesítjük és utána abban a pillanatban töröljük
      ![Számítógép törlés](../img/felhasznaloi/szamitogep/are-you-sure-delete.png)
    - a számítógépet csak tehermentesítjük (ebben az esetben az történik, hogy a számítógép nem kerül eltávolításra, hanem minden folyamat alól felszabadul)
      ![Számítógép folyamat kiszervezés](../img/felhasznaloi/szamitogep/are-you-sure-outsource.png)

**Tehermentesítés**: Az a lépés, amikor egy számítógépről minden folyamatot átcsoportosítunk egy másik gépre.

**Kivétel**: Szélsőséges esetben történhet olyan, hogy a többi rendelkezsére álló számítógépek magas kihasználtsága miatt nincs lehetőség teljes tehermentesítésre. Erre a program figyelmeztet minket, és csak annyi folyamatot fog áthelyezni a többi számítógépre, amennyit csak tud, a többit meg rajta hagyja a jelenlegi gépen. Ebben az esetben akár a törlés gombra, akár a kiszervezés gombra kattintva, csak annyi program fog kiszerveződni, amennyi csak tud, a többi pedig a gépen marad.

![Számítógép nem törölhető](../img/felhasznaloi/szamitogep/computer-outsource.png)

## Rendezés és szűrés

A számítógépek között lehetőség van szűrésre és rendezésre az alábbi szempontok szerint:

![Számítógép nem törölhető](../img/felhasznaloi/szamitogep/computer-filter.png)

## Exportálás

Lehetőségünk van a számítógépek CSV exportálására is a következő adatokkal:

    - Számítógép neve
    - Processzor kapacitása
    - Processzor jelenlegi terheltsége
    - Memória kapacitása
    - Memória jelenlegi terheltsége

![Számítógép exportálás](../img/felhasznaloi/szamitogep/computer-export.png)

## Új hozzáadása

Lehetőség van számítógép új hozzáadására is, ahol az alábbi adatokat kell megadnunk:

1. Számítógép neve (egyedi kell legyen)
2. Processzor kapacitása
3. Memória kapacitása

![Számítógép exportálás](../img/felhasznaloi/szamitogep/new-computer.png)

## Optimalizáció

Lehetőség van a gépeken futó folyamatok eloszlásának optimalizálására is.

![Számítógép optimalizálás gomb](../img/felhasznaloi/szamitogep/computer-optimize-button.png)

Ehhez meg kell adnunk azt a minimum és maximum százalékot, amik között a számítógép terhelésének lennie kell.

![Számítógép optimalizálás felugró ablak](../img/felhasznaloi/szamitogep/computer-optimize-popup.png)

Amennyiben lehetséges ez az elosztás, akkor a gépek terheltsége a megadott intervallumon belül lesz.

![Számítógép optimalizálás sikeres](../img/felhasznaloi/szamitogep/computer-optimize-success.png)

Ez amennyiben nem lehetséges a program felajánlja azt a lehetőséget, hogy minden gép egyenlő mértékben legyen leterhelt.

![Számítógép optimalizálás szórás](../img/felhasznaloi/szamitogep/computer-optimize-spread.png)

![Számítógép optimalizálás szórás sikeres](../img/felhasznaloi/szamitogep/computer-optimize-spread-success.png)

## Részletek

A számítógépek részletesebb információinak megtekintéséhez rá kell kattintani a megfelelő kártyára. Ezután látható a számítógépen futó folyamatok száma (aktív és inaktív), a számítógép leterheltsége, valamint különféle grafikonok. A grafikonok azt mutatják meg, hogy milyen eloszlásban mennyi program fut, valamint hogy az egyes programok milyen eloszlásban terhelik processzor és memória szintjén egyaránt a számítógépeket.

![Számítógép részletek](../img/felhasznaloi/szamitogep/computer-details.png)

Továbbiakban lehetőség van itt is a számítógép törlésére, amire ugyan az érvényes, mint a [fent](#tehermentesites-es-torles) leírtakban.

![Számítógép részletek törlés](../img/felhasznaloi/szamitogep/computer-details-delete.png)

Lehet még a számítógépeket szerkeszteni is, ahol a processzor és a memória kapacitását lehet állítani. Fontos megjegyeznivaló, hogy mind a processzor és mind a memóriakapacitásnak nagyobbnak kell lennie, mint az akutális leterheltség, ha ennek nem teszünk eleget, a program nem fogja engedélyezni a szerkesztést.

![Számítógép részletek szerkesztés](../img/felhasznaloi/szamitogep/computer-details-edit.png)

![Számítógép részletek szerkesztés űrlap](../img/felhasznaloi/szamitogep/computer-details-edit-form.png)

Végső soron lehet még a már számítógépen futó folyamatokat leállítani, vagy pedig aktívvá/inaktívvá tenni.

![Számítógép részletek program állapot](../img/felhasznaloi/szamitogep/computer-details-program-state.png)

## Egyéb információk

- Mind a listában, és mind a részletek oldalon megtekinthető a gép aktuális terheltsége százalékos értékben is.
- Ha sok folyamatot futtatunk egy számítógépen (nem túlterhelve annak erőforrásait), akkor lassulás észlelhető. Ezt le is teszteltük több, mint 10.000 folyamattal. Ezek betöltése együttesen kb. 14 GB memóriát igényelt.

![Egyszerre sok folyamat futtatása](../img/felhasznaloi/szamitogep/extrem-ertekek.png)