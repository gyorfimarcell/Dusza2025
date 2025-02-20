# Program

A klaszteren futtatható programok a .klaszter fileban vannak definiálva.

## Elvárt futási igények

Egy programnak rendelkezni kell az alábbi információkkal:

1. Program neve
2. Futtandó példányok száma a klaszteren. Amennyiben nem annyi példány fut a programból (akár aktív, akár inaktív), a [klaszter állapota](klaszter.md) sérülni fog.
3. Processzorigény
4. Memóriaigény

## Létrehozás

Az "Új" gombra kattintva lehetőségünk nyílik új program létrehozására. Itt az alábbiakat kell megadni:

1. Program neve (egyedinek kell lennie)
2. Aktív folyamatok száma: Mennyi példányban kell a program fusson
3. Processzorigény
4. Memóriaigény

## Módosítás

Módosítás során az alábbiakat lehet szerkeszteni.

1. Aktív folyamatok száma
2. Processzorigény
3. Memóriaigény

## Törlés

Lehetőség van egy program törlésére (leállítására) is. Ebben az esetben a program rákérdez arra, hogy biztos vagyunk-e a szándékunkban. 

Amennyiben jóváhagyjuk a program törlését, akkor először minden hozzátartozó folyamatot le fog a gépről állítani, és utána fog a program eltávolításra kerülni. 

## Rendezés és szűrés

A programok között lehetőség van szűrésre és rendezésre az alábbi szempontok szerint:

## Egyéb információk

