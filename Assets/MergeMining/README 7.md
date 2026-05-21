# Iteration 9 — Polish Pass

## 1. Что было добавлено
Помощник анимации летящих currency (coins/gems) от любой точки экрана к HUD-счётчикам — `FlyEffectSpawner`. Используется в Zone Complete попапе: при нажатии Continue гемы летят к иконке gems в HUD с анимацией. Готовый компонент `DOCounterText` для анимации числовых счётчиков. Фикс TutorialOverlay (правильное позиционирование через RectTransformUtility).

## 2. Что изменилось с прошлой итерации
- `TutorialOverlay` теперь корректно позиционирует ring/hand/text через `RectTransformUtility.WorldToScreenPoint` + `ScreenPointToLocalPointInRectangle`. Используется `anchoredPosition` вместо `position`. Ring имеет clamp 180×120 .. 700×380 — больше не "режет глаз" гигантским кругом
- Добавлен `FlyEffectSpawner` (singleton на Canvas) — методы `FlyCoins(worldPos, amount, count)` и `FlyGems(worldPos, amount, count)`. Спавнит N маленьких иконок которые летят по дуге к HUD-целям, начисляют currency на финале с lite-haptic
- На сцене созданы `CoinFlyTemplate` и `GemFlyTemplate` (скрытые шаблоны)
- `ZoneCompletePopup` теперь не добавляет gems сразу. Gems добавляются по нажатию Continue через `FlyEffectSpawner.FlyGems` — игрок видит как гемы летят из попапа к счётчику
- Создан `DOCounterText` — компонент на TMP, методы SetImmediate(value) и AnimateTo(value). Можно использовать в будущем для smooth изменения цифр прогресс-баров

## 3. Editor скрипты

`Tools / Merge Mining / (Iteration 9) Update Game Scene`

## 4. Как тестировать

### Tutorial fix:
1. Settings → RESET PROGRESS → перезапусти сцену
2. Туториал теперь корректно подсвечивает целевой объект — ring точно по центру кнопки SHOP / сетки / блока, не вылазит за границы
3. Размер ring адекватный — не закрывает половину экрана

### Letящие гемы при Zone Complete:
4. Мержь кирки до lvl 3 → Zone Complete popup
5. Жми Continue → 4 голубых гема летят из позиции gem-иконки в попапе к HUD-счётчику gems
6. Счётчик в HUD анимированно увеличивается (как уже было раньше)
7. При прилёте каждого гема — лёгкая вибрация

### Coin burst при разрушении блока (как раньше):
8. Разруши блок → 6 монеток летят к HUD-coins → счётчик растёт

## 5. Ожидаемый результат
- Tutorial highlight ring корректно подсвечивает целевые объекты
- Zone Complete становится более "сочным" — игрок видит как гемы реально летят в его инвентарь
- FlyEffectSpawner можно использовать в будущих попапах (Offline Rewards Claim, Chest Opening) при необходимости

## 6. Известные ограничения
- Реальные летящие гемы при Zone Complete — только в Continue handler. Если игрок закроет попап через backdrop/ESC — гемы добавятся мгновенно без анимации
- DOCounterText создан но пока не используется в существующих счётчиках — это для будущего polish

## 7. Что будет в следующей итерации
Final pass: Daily reward popup при первом заходе за день, achievements (10 шт), общий проход по всем экранам на полировку, баланс цен/HP/наград.
