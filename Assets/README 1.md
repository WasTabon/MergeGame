# Iteration 10 — Final pass: Daily Reward + Achievements

## 1. Что было добавлено
Ежедневная награда при первом заходе за день (Daily Reward popup). 10 achievements с разными условиями (разрушено блоков, куплено кирок, max level кирки, открыто зон, открыто сундуков, использовано бустеров). Achievement toast — небольшая всплывающая плашка сверху которая показывает unlocked achievement и автоматически закрывается через 3 сек. Это финальная итерация — игра feature-complete.

## 2. Что изменилось с прошлой итерации
- В Resources созданы `DailyRewardConfig.asset` (7 дней цикл) и `AchievementConfig.asset` (10 achievements)
- На Canvas добавлены `DailyRewardManager` и `AchievementManager` (singletons)
- Появились новые UI: `DailyRewardPopupCanvas` (sortingOrder=250), `AchievementToastCanvas` (260, без backdrop)
- `ChestManager.OpenChest` теперь инкрементит счётчик открытых сундуков
- `BoosterManager.TryActivate` инкрементит счётчик использованных бустеров
- `BlocksRowManager.OnBlockDestroyed` и `PickaxeGridManager.DoMerge` дёргают `AchievementManager.CheckAll()` после прогресса

## 3. Editor скрипты

`Tools / Merge Mining / (Iteration 10) Update Game Scene`

## 4. Как тестировать

### Daily Reward:
1. Открой Game scene → если PlayerPrefs ключ `daily_last_claim_date` отсутствует или ≠ сегодняшняя дата → попап DAILY REWARD появится сразу
2. День 1 → +100 coins
3. CLAIM → coins начисляются, попап закрывается
4. Если перезапустить сцену → попап **не появится** (уже клеймнул сегодня)
5. На следующий день → попап появится со словом "DAY 2" и +200 coins
6. Цикл 7 дней: Coins → Coins → Gems → Coins → Pickaxe Lv3 → Gems → Gems (большая) → снова с Day 1 (но прогресс растёт)
7. **Тест без ожидания суток:** удали ключ `daily_last_claim_date` через RESET PROGRESS (стирает ВСЁ) или вручную через Window/Analysis/Player Settings PlayerPrefs editor

### Achievements:
8. Разрушай блоки → как только наберётся 10 → появится toast сверху "UNLOCKED: BEGINNER MINER" + "+2" gems
9. Toast 3 секунды показан → fade out → если параллельно открылся ещё один — встаёт в очередь
10. Покупай кирки → "STOCKING UP" на 5-й покупке
11. Мержь → "SKILLED MERGER" на lvl 5
12. Открой 10 сундуков → "TREASURE HUNTER"
13. Используй 5 бустеров → "POWER USER"

### Achievement список (всего 10):
- Beginner Miner — 10 блоков → +2 gems
- Persistent Miner — 100 блоков → +5
- Block Crusher — 500 блоков → +10
- Stocking Up — 5 кирок → +2
- Big Spender — 50 кирок → +10
- Skilled Merger — lvl 5 → +3
- Merge Master — lvl 10 → +10
- Explorer — 3 зоны → +5
- Treasure Hunter — 10 сундуков → +5
- Power User — 5 бустеров → +3

## 5. Ожидаемый результат
- Daily Reward появляется ровно один раз в сутки (UTC dates)
- Achievement Toast не блокирует ввод (CanvasGroup blocksRaycasts=false) → игрок может играть пока toast висит
- Несколько достижений в одну секунду → очередь (например 100 блоков + lvl 10 одновременно — покажут по очереди)
- Достижения сохраняются (PlayerPrefs `achievement_claimed_<id>`)
- Reset Progress сбрасывает всё включая daily и achievements

## 6. Известные ограничения
- Achievement отображается только тостом — нет отдельного экрана со списком всех достижений. Это нормально для прототипа MVP
- Daily reward цикл — простой 7-дневный, без streak бонусов и stamping calendar UI
- Pickaxe-награда в Daily если сетка занята → конвертируется в coins (level × 50)

## 7. Game is feature-complete!
Все 10 итераций по плану выполнены:
1. Foundation: каркас, менеджеры, базовые попапы
2. Pickaxes/Merge/Shop: drag&drop, merge, магазин
3. Blocks/Mining: атаки, добыча, награды
4. Pause/Settings/Popups: основные попапы
5. Zones: 5 зон с прогрессией
6. Chests: 3 типа сундуков
7. Boosters: 4 бустера
8. Tutorial/Offline: туториал + offline rewards
9. Polish: летающие currency
10. Final: daily + achievements

Что можно докрутить в "post-launch":
- Реальные арт-ассеты (сейчас всё — placeholder shapes из Unity UI)
- Реальные звуковые клипы для SoundManager
- Save state сетки кирок при перезапуске
- iOS Taptic Engine plugin для лучшей вибрации
- Шейдерный градиент фона зон
- Local notifications когда Free Chest готов / daily reward доступен
- Реклама за x2 Claim в Offline Rewards
- Список достижений в отдельном попапе/экране
