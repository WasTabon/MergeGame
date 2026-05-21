# Iteration 5 — Zones + Progression

## 1. Что было добавлено
5 зон с разными визуалами и сложностью. Прогресс по достижению уровня кирки. Zone Complete popup теперь показывает имя завершённой и следующей зоны. Реальные мультипликаторы HP/reward по зонам. Появился Lava тип блока.

## 2. Что изменилось с прошлой итерации
- В Resources создан `ZoneConfig.asset` с 5 зонами:
  - **Stone Cave** (1+) — Stone + Iron, ×1.0 HP, +1 gem за прохождение
  - **Iron Mine** (lvl 3+) — Iron + Stone + Gold, ×1.2 HP, +2 gems
  - **Gold Mine** (lvl 6+) — Gold + Iron, ×1.5 HP, +3 gems
  - **Crystal Cave** (lvl 9+) — Crystal + Gold, ×1.8 HP, +5 gems
  - **Lava Core** (lvl 12+) — Lava + Crystal, ×2.2 HP, +10 gems
- `BlockData` теперь поддерживает override sequence по зоне и zone multiplier'ы
- В `BlockConfig.asset` добавлен 5-й тип — **Lava** (×4 HP, ×7 reward)
- На Game scene появился новый ZoneBackground (двухцветный градиент top/bottom), старый одноцветный Background отключён
- На Game scene появился ZoneHud — небольшая полоска под топ-HUD с именем зоны и индикатором "ZONE N/5"
- При merge'е через `PickaxeGridManager` вызывается `ZoneManager.CheckUnlock(highestLevel)`. Если уровень кирки достиг порога новой зоны → ZoneCompletePopup с +N gems → автоматически переключается зона → фон плавно меняет цвет → блоки текущего ряда уничтожаются и спавнятся новые из последовательности новой зоны
- `ZoneCompletePopup` теперь показывает "X COMPLETED" + "NEXT: Y" + "+N" gem'ов

## 3. Editor скрипты

В Unity:
1. `Tools / Merge Mining / (Iteration 5) Update Game Scene`

Создаст ZoneConfig.asset, обновит BlockConfig (если нужно — добавит Lava), добавит зон-фон, ZoneHud и привяжет ZoneManager.

## 4. Как тестировать

### Старт:
1. Открой MainMenu → Play → Game scene
2. Должна быть **Zone 1 / 5: STONE CAVE** в небольшой полоске под HUD
3. Фон тёмно-синий с градиентом сверху→снизу
4. Блоки — Stone (серые) + Iron (бронзовые)

### Прогресс зоны:
5. Мержь кирки чтобы получить lvl 3 (1+1=2, 2+2=3)
6. Как только появится кирка lvl 3 → должен сразу же сработать ZoneComplete popup: "STONE CAVE COMPLETED" / "+2" / "NEXT: IRON MINE"
7. Continue → фон меняется на бронзово-коричневый, ZoneHud показывает "ZONE 2/5: IRON MINE", блоки заменяются на новый ряд из последовательности Iron Mine
8. Продолжай мержить — на lvl 6 откроется Gold Mine, lvl 9 — Crystal Cave (появится Crystal блок!), lvl 12 — Lava Core

### Проверка сохранения:
9. Открой Pause → Main Menu → возвращайся → текущая зона должна остаться той же
10. RESET PROGRESS в MainMenu → возвращает к Stone Cave (zone index = 0)

### Сложность:
11. В Lava Core HP блоков уже умножен на 2.2 от формулы (плюс сам Lava имеет ×4 от HP-мультипликатора типа). Без серьёзных кирок (lvl 12+) блок будет ломаться долго — это специально для долгого endgame

## 5. Ожидаемый результат
- Переключение зоны выглядит юзабельно: попап → continue → фон плавно красится за 0.8s → блоки перерождаются как новый ряд
- В каждой зоне свой характер цвета и набор блоков
- Highest reached pickaxe level сохраняется → unlocked zone тоже фиксируется (после reset progress начинается заново)

## 6. Известные ограничения
- Сам PickaxeData (15 уровней) не имеет визуальной привязки к зонам — это просто цвет уровня
- Звуковые ассеты пока не подключены
- Нет анимации "разблокированной зоны" в UI (показывается через попап, но без бирки "unlocked")
- Crystal блок есть, но gem-выпадение из него — иттер 6/7
- Bg-градиент сейчас не настоящий градиент (две полоски — верх и низ), для prototype'а достаточно. В polish-итерации можно добавить настоящий vertex-color градиент через шейдер

## 7. Что будет в следующей итерации
Сундуки (Free Chest по таймеру 5 мин + Block Chest каждые 20 блоков), Chest opening попап с выпадением кирок/coins/gems, Premium Chest за gems.
