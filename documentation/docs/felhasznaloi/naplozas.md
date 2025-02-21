# Naplózás

A rendszer naplózási funkciója minden fontos eseményt rögzít annak érdekében, hogy a felhasználók és rendszergazdák visszakövethetően monitorozhassák a klaszter működését. A naplófájl tartalmazza az egyes **események típusát**, azok **pontos időpontját**, valamint az esetlegesen kapcsolódó **további információkat**.


Ezeket az adatokat egy `Logs` nevű mappába, egy `.log` kiterjesztésű fájlba menti. A fájlnév mindig az aktuális dátumból jön létre a következő módon: `yyyyMMdd`. Ha már létezik egy naplófájl az aktuális dátummal, akkor az új sor az adott fájlhoz hozzáfűződik, különben új fájlt hoz létre, aminek az első sora az új adat.
![Naplófájl](../img/felhasznaloi/naplozas/naplofajl.png)

## Működése

Minden egyes esemény után automatikusan lefut egy naplózási folyamat a [fentebb](#naplozas) említett módon. Ezeket az adatokat a fájlokból kiolvasva a `Klaszter naplók` fülnél érhetjük el, és tanulmányozhatjuk egy letisztultabb, praktikusabb környezetben.
![Klaszter naplók oldal](../img/felhasznaloi/naplozas/klaszter-naplok-oldal.png)
Itt lehetőségünk van az egyes fájlok tartalmát részleteiben listázni, napokra leosztva. Ha a kártyára kattintunk, lenyílik az adott fájl tartalma, ami egy kártya lista az eseményekről. A kártyák önmagukban csak az események típusát és végrehajtásuk időpontját tartalmazzák, kibontásuk után (ha van rá lehetőség) viszont az esemény további részletei jelennek meg. Ezek eseménytípusonként változnak.
![Esemény részletek](../img/felhasznaloi/naplozas/esemeny-reszletek.png)

A rendszer a következő eseményeket naplózza:

- **OpenProgram:** *Egy program megnyitása*
- **CloseProgram:** *Egy program bezárása*
- **LoadCluster:** *A klaszter betöltése*
- **AddComputer:** *Új számítógép hozzáadása*
- **DeleteComputer:** *Számítógép törlése*
- **ExportCSV:** *Naplófájl exportálása CSV formátumban*
- **AddProgram:** *Új program hozzáadása a klaszterhez*
- **RunProgramInstance:** *Egy program példányának elindítása*
- **ShutdownProgramInstance:** *Egy program példányának leállítása*
- **ModifyProgram:** *Egy program beállításainak módosítása*
- **ShutdownProgram:** *Egy teljes program leállítása*
- **ClearProgramInstances:** *Egy számítógép tehermentesítése*
- **MoveProgramInstance:** *Egy programpéldány áthelyezése másik számítógépre*
- **OptimizeProgramInstances:** *Programpéldányok optimalizálása*
- **ModifyComputer:** *Egy számítógép beállításainak módosítása*
- **SpreadProgramInstances:** *Programok egyeneletesen történő elosztásának a klaszterben*
- **FixIssues:** *Klaszterhibák automatikus javítása*
- **ActivateProgramInstance:** *Egy inaktív program aktiválása*
- **DeactivateProgramInstance:** *Egy aktív program inaktiválása*
- **GenerateCluster:** *Új klaszter generálása*

Minden naplózott esemény a következő formátum szerint kerül rögzítésre:

```
<Esemény típusa> - <Időbélyeg> - <Részletek>
```

Pl.:

```
RunProgramInstance - 2025.02.21. 09:11:54 - word-6r82gx - 2025.02.21. 09:11:54 - False - 150 - 300 - szamitogep17
```

## Szűrés

## Egyéb információk