# MiniFactory — Roadmapa

Factorio-like top-down hra v MonoGame. Tenhle dokument sleduje postup od úplného začátku
(prázdný projekt) až po funkční demo, a slouží i jako mapa toho, jak by hra měla nakonec
vypadat. Věci označené **[TBD]** ještě nemají domluvený konkrétní návrh — jen víme, že tam
mají být, ne přesně jak budou fungovat.

Legenda: `[x]` hotovo, `[~]` rozděláno/částečně, `[ ]` nezačato.

---

## Vize hry

- Top-down, volný pohyb hráče, stavění funguje v gridu
- Vektorová grafika (později — teď placeholder textury/barvy)
- Progrese: kámen → pec → uhlí/železo → tavení → crafting komponent/strojů → stroje (těžiče, pásy, craftery) → automatizace
- Crafting: ruční (hráč v menu) i automatický (craftery jako stroje na páse)
- Ekonomika: nákup/prodej
- Ruční těžba s dosahem a progress barem, než se odemknou automatické těžiče

---

## Základy / architektura

- [x] Herní smyčka, `MiniFactoryGame` (Update/Draw), platformové projekty (DesktopGL/Android/iOS)
- [x] Souřadnicový systém: `World.PixelToGrid` / `World.GridToPixel`, `TileSize` konstanta
  - [~] `TileSize` je teď testovací hodnota (64px) — finální velikost/škálování se bude řešit spolu s kamerou
- [~] `World` — `Terrain`/`Water` (2D pole), `Buildings`/`Resources` (dictionary), `TryGetResourceAt`
  - `Terrain`/`Water` už se generují a vykreslují (viz sekce "Svět / mapa" níže); `Buildings` je pořád jen deklarovaný dictionary, nic ho nenaplňuje ani nevykresluje
- [x] `.gitignore`, oprava umístění git repozitáře a `.sln`
- [x] `.sln` se všemi projekty (Core, DesktopGL, Android, iOS)

## Svět / mapa ✅

- [x] `TerrainType` enum (zatím jen `Grass`) + `World.Terrain`/`World.Water` jako husté 2D pole (`[,]`, pevná velikost mapy) — `Resources`/`Buildings` zůstávají `Dictionary<Point, T>`, protože jsou řídká (většina buněk nic nemá)
- [x] Skutečné dlaždice — pixel art ze "Sprout Lands" (Cup Nooble, credit nutný při zveřejnění hry), 47-dílná "blob" autotile sada (`BlobTileset`, bitmaska sousedů v `World.ComputeBitmask`)
- [x] Voda jako oddělená vrstva (`World.Water`) — funguje i jako "moře" automaticky za hranicí mapy (`World.IsWater` s bounds-checkem, žádná data navíc potřeba)
- [x] Ořezávání vykreslování podle viditelné oblasti kamery (`MiniFactoryGame.GetVisibleGridBounds`) — nekreslí se celá mapa, jen to, co je vidět (otestováno na 1000×1000 bez propadu výkonu)
- [ ] **[TBD do budoucna]**: mapa je zatím jen plochý obdélník/čtverec — promyslet tvar mapy (např. ostrov s nepravidelným pobřežím místo pravoúhlé hranice), případně jestli/jak generovat procedurálně
- Poznámka: `TerrainType` je připravený na další typy terénu (písek, cesta...) — vlastnosti podle typu (rychlost pohybu apod.) půjdou přes budoucí `TerrainDatabase`, stejný vzor jako `ItemDatabase` (per-type, ne per-instance)
- Poznámka: podlahy zvyšující rychlost pohybu (beton apod., jako ve Factoriu) budou samostatná vrstva `Floors` (blíž `Buildings` — hráčem stavěná infrastruktura — než `Terrain`), zatím neimplementováno

## Hráč

- [x] `Player` — pozice (`Vector2`), pohyb WASD, `_speed`
- [x] `MiningRange`, `MiningSpeed` jako vlastnosti hráče
- [ ] Krumpáč / nástroje ovlivňující těžbu **[TBD]** — zatím těžba nezávisí na vybaveném nástroji

## Ruční těžba

- [x] `ResourceTile` — `ItemType`, `Remaining` (clampnuté na 0), `MiningDuration`
- [x] `OreDeposit` — mřížka `ResourceTile`, `RegisterInWorld`, `TotalRemaining`
- [x] Detekce "co je pod myší" (`TryGetResourceAt`)
- [x] Kontrola dosahu (`Vector2.Distance` hráč ↔ střed políčka)
- [x] Vizuální zvýraznění (ohraničení) políčka v dosahu
- [x] Držení tlačítka → progress (`miningProgress`), reset při puštění/změně cíle
- [x] Vizuální progress bar u kurzoru
- [x] Dokončení těžby → snížení `Remaining`, vyčerpání → odstranění z `World.Resources`
- [x] Propojení s inventářem (`player.Inventory.TryAddItem`)
- [ ] Rozdílná doba těžby podle suroviny — `MiningDuration` je teď natvrdo `5f` pro **všechny** typy (kámen, uhlí, železo, měď těží stejně rychle). Potřeba upravit, aby každý `ItemType`/surovina měla vlastní čas těžby, ne jednu sdílenou hodnotu.

**Drobné chybky k opravě (zatím nehoří, ale je o nich vědět):**
- [ ] Těžba funguje i "skrz" otevřené UI (např. inventář) — když je myš nad UI panelem, těžba by neměla fungovat. Pozor, neřešit to jako "těžba nefunguje, když je inventář otevřený" (to by bylo zbytečně omezující) — jde jen o to, že myš nesmí být zrovna nad nějakým UI prvkem, jinak by mělo jít normálně těžit i s otevřeným inventářem

## Itemy

- [x] `ItemType` enum (rezervované rozsahy: suroviny 0–299, stavitelné 300–599)
- [x] `ItemDefinition` (Name, Description, Weight, Price, ItemColor, ItemTexture)
- [x] `ItemDatabase` — `static class`, `LoadContent(ContentManager)`, `Get(ItemType)`
- [x] Skutečné textury pro suroviny (stone/coal/iron_ore/copper_ore), mipmapy zapnuté
- [ ] Sjednotit `ResourceTile.GetColor()` (natvrdo switch) s `ItemDatabase` — teď je barva definovaná na dvou místech **(otevřený technický dluh, ne bug)**

## Inventář / Menu (Tab)

Tab zatím otevírá jen inventářový panel — do budoucna se má rozšířit na **obecné menu**,
kde bude inventář jen jedna část. Domluvené UI rozložení **[TBD — implementace]**:

- [ ] Otevření přes Tab ukáže celé menu, ne jen inventář (inventář zůstává jako teď, dole)
- [ ] V **pravém horním rohu** — crafting menu, nalepené na roh
  - [ ] Po levé straně crafting menu — sloupec s crafting queue (viz sekce Crafting výše, každý item trvá nějakou dobu vyrobit)
- [ ] V **levém horním rohu** — malý panel s penězi (počet peněz hráče, viz Ekonomika níže)

Hotovo zatím (samotný inventářový panel):

- [x] `Inventory` (`Slots[rows, columns]`), `InventorySlot` (`Type`, `Count`)
- [x] Stackování v `TryAddItem`, plnění po řádcích
- [x] UI panel — Tab přepínání, plynulá animace vysunutí/zasunutí zespodu (`inventorySlide`, `Lerp`)
- [x] Grid slotů, vycentrovaný v panelu, zaoblené rohy (9-slice technika, `SpriteBatchExtensions`)
- [x] Barevná paleta (`panelBackground`, `slotColor`, `accent`, ...)
- [x] Zobrazení itemu ve slotu (textura + počet kusů)
- [x] Hover tooltip nad slotem (název itemu)
- [x] Vlastní fonty ve 3 velikostech (`fontSmall`/`fontMedium`/`fontLarge`) místo runtime `scale`
- [ ] Tooltip — cena/popis (teď jen název) **[TBD]** — chceme zobrazit i `Description`/`Price`?
- [x] Přesouvání itemů mezi sloty (drag & drop) — swap i slučování stejného typu s ohledem na `StackSize`
  - [x] Tažení jde začít jen z neprázdného slotu (`slot.Count > 0`)
- [ ] Co se stane, když je inventář plný a nejde přidat další item **[TBD]**

**Drobné chybky k opravě (zatím nehoří, ale je o nich vědět):**
- [ ] Hover tooltip (název itemu) by se neměl zobrazovat, když zrovna probíhá drag & drop — teď se ukáže i nad itemem, co zrovna táhneš/nad slotem, přes který s ním projíždíš

## Refactor `MiniFactoryGame.cs` ✅

Hotovo (větev `refactor`). `MiniFactoryGame.cs` kleslo z ~345 na ~180 řádků, `Update`/`Draw`
jsou teď čistý orchestrátor:

- [x] Mining logika (hover/dosah/progress/dokončení) přesunuta na `Player` (`Player.Update`, `Player.DrawMiningUI`), vyčerpání ložiska teď řeší `World.RemoveIfDepleted(gridPos)` místo přímého sahání do `World.Resources` zvenku
- [x] Inventářové UI (panel, sloty, drag & drop, tooltip, animace) přesunuto do nové třídy `InventoryUI` (`UI/InventoryUI.cs`) — záměrně **oddělené** od datové třídy `Inventory` (data vs. prezentace, ať jde stejná data časem zobrazit i jinak — obchodník, truhly...)
- [x] Hit-testing/rozhodování je teď v `Update`, `Draw` jen čte hotový stav a kreslí
- [x] Prozkoumali jsme architektonické vzory (MonoGame `GameComponent`/`Services`, ECS, prosté explicitní OOP) — vědomě zůstáváme u explicitního předávání závislostí jako parametrů, ne `Game.Services`/service locator (čitelnější pro učení, žádný skrytý stav)
- [ ] Menší budoucí refactor: `LoadContent`/`Initialize` zatím nejsou takhle roztříděné (zatím toho tam je málo, takže nehoří) — až přibude víc assetů/inicializace, zvážit rozdělení do vlastních metod/tříd stejným způsobem
- Konvence pro **nově psaný kód** (ať se tenhle refactor neopakuje) jsou zapsané v [`CLAUDE.md`](CLAUDE.md) v kořeni repozitáře

## Kamera ✅

- [x] `Camera` třída (`Position`, `Zoom`, `GetTransformMatrix`) — `Matrix.CreateTranslation` + `CreateScale`, žije v `MiniFactoryGame` (ne v `Player` — kamera je sdílený "view" koncept, potřebuje ji jak `World.Draw`, tak `Player.Draw`, ne jen hráč sám)
- [x] Kamera sleduje hráče (`camera.Position = player.Position` v `Update`)
- [x] `Draw` rozdělený na dva `SpriteBatch.Begin`/`End` bloky — jeden s `transformMatrix:` (svět, hráč), druhý bez (UI — inventář, HUD text, progress bar)
- [x] Inverzní transformace pozice myši (`Matrix.Invert` + `Vector2.Transform`) — `worldMousePos`/`currentMousePosGrid` se počítají **jednou** ze světové pozice a používají konzistentně všude (těžba, hover, border) — `screenMousePos` zůstává jen pro UI hit-testing
- [ ] Plynulé sledování hráče kamerou (teď "teleportuje" okamžitě na `player.Position` — chtělo by to `Lerp`/podobné vyhlazení)
- [ ] Ovládání přiblížení/oddálení (zoom) — zatím napevno `1f`, žádný vstup od hráče (kolečko myši?)
- [ ] Převod velikosti `TileSize` z testovací (64px) na finální (s vektorovým artem) — závisí na doladění kamery/zoomu

## Stavění **[TBD — kompletně nenavrženo, jen zmíněno]**

- [ ] Jak přesně funguje pokládání budov do gridu (kolize s ložisky/terénem, orientace budov, náklady v itemech)
- [ ] `Building.cs`/`Tile.cs` — zatím prázdné kostry tříd, bez chování

## Zpracování surovin (pec/tavení) **[TBD]**

- [ ] Pec — recept (uhlí + železná ruda → železo), doba tavení, jak hráč pec obsluhuje (ručně vkládá suroviny? automaticky?)
- `ItemType.FurnaceTierOne` existuje jen jako hodnota enumu, žádná logika za tím

## Crafting

Domluvený koncept, detaily implementace ještě **[TBD]**:

- [ ] **Ruční crafting** — hráč si v menu (viz UI níže) může sám vyrobit komponenty/stroje z itemů v inventáři
- [ ] **Automatické craftery** (stroj) — mají nastavený recept (např. "vyráběj ozubená kolečka"); přes conveyor belt do nich přijde surovina (železo), craft proběhne po nějaké době, hotový item se pošle pryč (dál po pásu / do dalšího stroje)
- [ ] Crafting queue — fronta rozpracovaných/naplánovaných craftů, každý item trvá vyrobit nějakou dobu (viz UI)
- [ ] Recepty — co přesně z čeho jde vyrobit, jaké suroviny/komponenty existují mezi surovinou a hotovým strojem **[TBD]**

## Stroje / automatizace **[TBD]**

- [ ] Těžiče (automatická těžba bez hráče)
- [ ] Pásy (conveyor belt) — přesun itemů mezi stroji/craftery
- [ ] Automatické craftery jako konkrétní typ stroje (viz sekce Crafting výše)
- `ItemType.MinerTierOne`, `ItemType.BeltTier` existují jen jako hodnoty enumu

## Napájení (elektřina) **[TBD]**

- [ ] Stroje potřebují elektřinu, aby fungovaly — bez napájení stojí
- [ ] Elektrické vedení — propojování zdrojů energie se stroji (podobně jako conveyor belt propojuje suroviny)
- [ ] Zdroje energie: solární panely, uhelné elektrárny (a časem asi další tiery)
- [ ] Day/night cyklus — ovlivňuje hlavně solární panely (v noci nevyrábí/vyrábí míň), takže hráč potřebuje kombinaci zdrojů nebo skladování energie
- Inspirace: Mindustry řeší napájení přes "Power Node" (uzly, co propojují zdroje/spotřebiče v okolním rádiusu) a solární panely tam mají výkon závislý na okolním osvětlení/dennní době — velmi blízké tomu, co popisuješ (viz sekce Grafická inspirace níže)

## Ekonomika **[TBD]**

Domluvený koncept — kombinace víc způsobů prodeje, detaily implementace ještě otevřené:

- [ ] **Lokální obchodník** (na ostrově) — vykupuje omezené a **rotující** množství surovin podle dne (např. den 1 vykupuje dřevo, den 2 kámen), navíc s denním stropem množství (např. max 100ks daný den). Pravděpodobně provázané s day/night cyklem výše (nový den = nová nabídka obchodníka).
- [ ] **Export lodí** — pro větší množství/pozdější fázi hry. Hráč naloží zboží na loď, zaplatí za export, loď odjede, zboží se prodá (asi s prodlevou, ne okamžitě).
- [ ] Panel s penězi v UI (levý horní roh menu, viz sekce Inventář/Menu výše)
- [ ] **Využití peněz — potřeba vymyslet, na co si hráč peníze utratí** (zatím jen nápady, nic domluvené):
  - Odemykání dalších tierů strojů (těžiče, craftery, pásy vyšší úrovně) — tvůj nápad
  - Blueprinty/recepty za peníze (odemčení nového craftu)
  - Rozšíření mapy / koupě dalšího pozemku pod stavby
  - Najímání NPC dělníků (pokud by hra měla i jinou pracovní sílu než hráče)
  - Nákup surovin, které se na ostrově nedají těžit (opačný směr k obchodníkovi/exportu)
  - Kosmetické věci (vzhled staveb, hráče)
- `ItemDefinition.Price` existuje jako pole, ale nikde se zatím nepoužívá

## Vizuál / assety

- [x] Placeholder textury pro suroviny (Inkscape) + 9-slice textura s zaobleným rohem pro UI
- [ ] Finální vektorový art (plánováno na později, celý projekt je zatím "placeholder-first")
- [x] Známé, akceptované omezení: mírné rozmazání textu/ikon na macOS Retina v okenním režimu — jde o limitaci MonoGame DesktopGL, ne chybu v projektu (viz GitHub issue [MonoGame#4802](https://github.com/MonoGame/MonoGame/issues/4802))

---

## Grafická inspirace

Podle popisu (top-down, čistý/plochý vektorový styl, factory automation) — pár her na
inspiraci vzhledu i mechanik. Jsou to odkazy na oficiální stránky her, ne přímo obrázky —
mrkni se tam na screenshoty/trailery přímo.

- **[shapez](https://store.steampowered.com/app/1318690/shapez/)** — nejblíž tomu, co popisuješ vizuálně: čistě 2D top-down, minimalistický plochý vektorový styl, žádné zbytečné detaily. Dobrá inspirace hlavně pro celkový "look" (barvy, jednoduché geometrické tvary strojů/pásů).
- **[Mindustry](https://store.steampowered.com/app/1127400/Mindustry/)** — top-down, plošší sprite styl (ne tak čistě vektorový jako shapez, ale blízko). Hlavně zajímavé pro **mechaniky** — má prakticky přesně to schéma napájení, co popisuješ (uzly propojující zdroje energie se stroji, solární panely závislé na denní době/osvětlení).
- **[Factorio](https://factorio.com/)** — samotný genre-defining vzor pro celou hru (progrese surovina → zpracování → automatizace → stroje), i když vizuálně je spíš detailnější/malovaný, ne čistě vektorový. Dobré na inspiraci UI (inventář, crafting menu) a celkovou strukturu progrese.

---

## Poznámky k údržbě dokumentu

Až něco rozpracujeme/dokončíme, aktualizuj checkbox a případně dopiš krátkou poznámku
(podobně jako u `TileSize`/barev výše) — cíl je, aby šlo kdykoliv na první pohled poznat,
kde přesně projekt je, bez nutnosti procházet celou historii konverzace.
