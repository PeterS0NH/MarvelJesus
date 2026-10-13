Marvel Jézus
Deadpool keresi a megfelelő Rozsomák variánst a különböző új világokban. A TVA viszont végig
követi és az egyes világok látogatása után felmérik a károkat. A TVA-ban egy json formátumot
készítenek ezekről, hogy későbbiekben ezeket könnyű legyen feldolgozni.
Feladat: A hallgató készítsen egy programot, ahol ezeket az adatokat fel tudjuk dolgozni.
A program legyen képes beolvasni a felhasználó által megadott fáljt és annak adatait mentse el egy
adatbázisba. Alakítsa ki a megfelelő adatbázis sémát és a tábla kapcsolatokat.
Legyen egy fő mappa “Deadpool” néven. Az almappák pedig az egyes világok. Minden egyes új
beolvasnál ellenőrizze le, hogy létezik a megfelelő világ almappája, ha nem, akkor hozza létre.
Legyen lehetőség ne csak fáljból hanem a programból is felvenni egy új világot és legalább egy
káreseményt. Tegye lehetővé, hogy utólag is szerkeszthetőek legyenek.
Ha egy új káresemény eléri az 1 millió dollárt, akkor egy eseményen keresztül értesítsük a fő
programot, hogy melyik világban milyen káresemény történt a pontos összeggel együtt.
Riportok készítése:
A készítsen egy kimutatást ahol, összesített kárértéket lehet megtekinteni világszinten.
Felhasználói igény szerint lehessen elmenteni a főkönyvtárba egy txt fáljba. Fálj neve legyen:
Summary_{aktuálisdátum}.txt pl : Summary_20241010.txt
A fő programban lehessen lekérni a károk helyreállításának időtartalmát . A lekérdezésben
szerepelnie kell hány incidenst érint a javítási idő típúsa és ezeknek mennyi az összköltsége. A
megfelelő almappákkba mentsük el xml-ként.
Earth-616
Hours: 1, $130
Days: 2, $50,000
Legyen lehetőség megjeleníteni a károkat számosságát kategóriák szerint költséggel, illetve a
károkat biztosítottság szerint szintén össz költséggel.
Legyen lehetőség listázni egy válaszott világ káreseményeit. Ügyeljen arra, hogy egyszerre csak 5
darabot jelenítsen meg és jelezze a felhasználónak, hogy egyébként összesen hány darab van. Az
oda és visszalapozást valósítsa meg 2 billentyűvel.
