# Iteration 17 — Block Types with Immunities

## 1. Що додано
П'ять типів блоків з різною поведінкою:
- **Stone** (сірий) — звичайний блок
- **Iron** (темно-синій) — отримує damage тільки від кирок lvl ≥ 3. Інакше — імунітет з візуальною feedback (блок здригається, спалахує білим)
- **Diamond** (блакитний) — отримує 50% damage завжди. HP трохи менше але reward 2x
- **Explosive** (червоний) — при руйнуванні наносить damage кільком найближчим киркам (зменшує їх durability на 5 одиниць)
- **Healer** (зелений) — кожні 2 секунди лікує сусідні блоки в радіусі 250px на 8 HP

Спуск блоків працює з ур.1 (8 px/sec, до 45 на ур.15).

## 2. Що змінилось у файлах
- `BlockTypeData` — нові поля: `behavior`, `minPickaxeLevel`, `damageMultiplier`, `explosionDamage`, `healPerSecond`, `healInterval`
- Enum `BlockBehavior`: Normal / Iron / Diamond / Explosive / Healer
- `Block.TakeDamage` — приймає sourcePickaxeLevel, перевіряє Iron immunity, застосовує damageMultiplier (Diamond)
- `Block.Update` — Healer тикає кожен healInterval і лікує сусідів
- `Block.Die` — Explosive викликає TriggerExplosion → зменшує durability сусідніх кирок
- `Block.HealedBy` + ShowHealFlash — візуальний feedback для лікованого блоку
- `Block.ShowImmuneFlash` — фідбек коли Iron не пробивається
- `MiningAttack.Launch` — приймає pickaxeLevel, передає у TakeDamage
- `PickaxeMiningBehaviour` — передає `pickaxe.Level` у Launch
- Editor оновлює BlockConfig (5 типів) і LevelConfig (descend з ур.1, різноманітні sequences з різними типами блоків на кожному рівні)

## 3. Editor скрипт

`Tools / Merge Mining / (Iteration 17) Update ALL`

## 4. Розкидка типів по рівнях
- **Ур.1-2** (maxPickaxe=2): Stone, Diamond, Explosive, Healer. Без Iron (не зможеш пробити)
- **Ур.3-5**: + Iron
- **Ур.6+**: Складніші мікси з більшою кількістю Iron і Healer
- **Ур.10+**: Майже немає Stone — суцільні Iron / Diamond / Healer / Explosive

## 5. Як тестувати

### Iron immunity:
1. Запусти Level 3 → у послідовності буде Iron-блок
2. Купи 1 кирку lvl1 → START → атаки по Iron-блоку = нічого не відбувається, блок здригається з білим спалахом
3. RETRY → купи 2 кирки → merge до lvl2 → атаки по Iron також не пробивають
4. Тільки lvl3+ кирка пробиває Iron

### Diamond:
5. На Level 1 буде Diamond блок (індекс 2 у sequence). HP трохи нижче, але damage 50% → треба більше атак

### Explosive:
6. На Level 2+ є Explosive (індекс 3). Коли його зломаєш → 3 найближчі кирки втрачають по 5 durability. Камера сильно трясеться

### Healer:
7. На Level 3+ є Healer (індекс 4). Зелений блок. Кожні 2 секунди він лікує сусідні блоки → їхні HP-бари ростуть зеленим спалахом
8. **Стратегія:** Healer треба вбивати першим, інакше навколо нього блоки регенерують швидше ніж їх руйнуєш

### Descend з ур.1:
9. Тепер навіть на Level 1 блоки повільно опускаються (8 px/sec)

## 6. Очікуваний результат
- Гравець мусить мержити щоб пробивати Iron
- Healer змушує думати про порядок руйнування
- Explosive змушує тримати запасних кирок
- Гра набагато складніша і потребує стратегії

## 7. Відомі обмеження
- Колір блоків — placeholder. Реальний artwork знадобиться
- Iron immunity не показує тексту "REQUIRES LVL 3" — просто здригання. Можна додати tooltip у наступних іттер
- Explosive завжди б'є 3 найближчі кирки — не залежить від кількості реально близьких
