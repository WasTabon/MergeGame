# Iteration 14 — Cleanup + Level Select Scene

## 1. Что было добавлено
Новая сцена LevelSelect с линейным путём из 15 уровней (Candy Crush style — узлы с волнообразным расположением). Узлы показывают номер, иконку замка для заблокированных, 3 звезды для пройденных. Звёздочки в LevelVictoryPopup теперь Image вместо TMP — можно подменить на свой sprite. Большая чистка Game scene от старых idle-систем (zones / chests / boosters / offline / tutorial / achievement toasts / daily reward).

## 2. Что изменилось с прошлой итерации

### Новое:
- Создана сцена `LevelSelect.unity` со ScrollRect и узлами уровней
- `LevelSelectController` — генерирует 15 узлов с прогрессом из PlayerPrefs
- `LevelSelectNodeView` — view одного узла (круг + цифра, или замок, плюс 3 звезды снизу)
- `LevelVictoryPopup.starXIcon` теперь `Image` (а не TMP). `starSpriteOverride` — поле SerializeField для подмены своей иконки. Placeholder: круг Knob.psd + ★ символ TMP поверх как indicator

### Изменено:
- `MainMenu.PLAY` → ведёт в `LevelSelect` (а не сразу в `Game`)
- `LevelSelectController` → выбор уровня → `Game`
- `LevelVictoryPopup.OnMenu` → возвращает в `LevelSelect` (а не в `MainMenu`)
- `PausePopup.OnMainMenu` → ConfirmPopup → `LevelSelect`
- `LevelManager.GoToLevelSelect()` — публичный метод для перехода

### Удалено из Game scene:
- `ChestOpeningPopupCanvas`, `ChestHud`
- `BoosterHud`, `BoosterEdgeGlow`
- `ZoneHud`, `ZoneCompletePopupCanvas`
- `OfflineRewardsPopupCanvas`, `TutorialOverlayCanvas`
- `AchievementToastCanvas`, `DailyRewardPopupCanvas`
- Компоненты с Canvas объекта: ChestManager, BoosterManager, ZoneManager, TutorialManager, OfflineRewardsManager, AchievementManager, DailyRewardManager

Прежний код всех этих систем остался в `Scripts/` — если в будущем понадобится, можно поднять. Просто на сцене больше не используется.

## 3. Editor скрипты

`Tools / Merge Mining / (Iteration 14) Update ALL` — выполняет всё:
- создаёт `LevelSelect.unity`
- чистит Game scene
- переделывает звёздочки в LevelVictoryPopup

Альтернативно по отдельности:
- `(Iteration 14) Create LevelSelect Scene`
- `(Iteration 14) Cleanup Game Scene`
- `(Iteration 14) Update Victory Popup Stars`

## 4. Как тестировать

1. Settings → RESET PROGRESS (чтобы начать с уровня 1)
2. На MainMenu → жми PLAY
3. Должна открыться **LevelSelect** scene с 15 узлами:
   - Уровень 1 — желтый круг с цифрой 1 (текущий, метка-кружок над ним)
   - Уровни 2-15 — серые с замком (locked)
   - Внизу под каждым — 3 пустые звёздочки
4. Тап на уровень 1 → переход в Game scene на уровень 1
5. Пройди уровень → Victory popup со звёздами (теперь это Image с круглым placeholder фоном + ★)
6. NEXT LEVEL → переход на уровень 2
7. Назад в LevelSelect через Pause → MAIN MENU → confirm QUIT
8. На LevelSelect: уровень 1 теперь зелёный (passed) с N звёздами, уровень 2 жёлтый (current), 3-15 заблокированы
9. Можно ткнуть на любой пройденный — переиграть его (для лучшего score)

### Подмена звёздочки на свою:
10. В Inspector LevelVictoryPopup найди `Star Sprite Override` → перетащи туда свой Sprite → во время Play звёзды будут использовать твой sprite (placeholder отключится при наличии override)

## 5. Ожидаемый результат
- LevelSelect выглядит как путь — узлы зигзагом
- ScrollRect автоматически скроллится к текущему уровню
- Прогресс между сценами сохраняется (current_level, max_passed_level, level_stars_N)
- Game scene теперь чистая — только то, что нужно для уровней

## 6. Известные ограничения
- Линии между узлами пути не нарисованы (можно добавить как Image-bar между точками, но это polish)
- Замок и кружок-маркер — без своих иконок, просто placeholder shapes
- Старые скрипты (ZoneManager, ChestManager, BoosterManager и т.д.) остались в проекте, но не используются — можно удалить файлы из `Scripts/` если хочешь полную чистоту проекта
- LevelSelect использует Knob.psd как placeholder для всех графических элементов — нужны иконки артиста

## 7. Game is now level-based
- 15 уровней с прогрессией сложности
- Setup → Battle → Victory loop
- Сохраняется прогресс по уровням и звёзды
- Level Select с unlocking
- Все idle-элементы убраны
