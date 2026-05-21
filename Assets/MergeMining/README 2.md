# Iteration 4 — Win/Lose/Pause + Popups

## 1. Что было добавлено
Pause-попап, отдельный Settings-попап на game scene, Zone Complete демо-попап (каждые 10 разрушенных блоков), универсальный ConfirmPopup, защита Reset Progress подтверждением.

## 2. Что изменилось с прошлой итерации
- `BasePopup` теперь поддерживает `pausesGame` флаг — попап с этим флагом ставит `Time.timeScale=0` на показе и обратно `=1` на скрытии
- Все анимации внутри попапов используют `.SetUpdate(true)` (unscaled time) — попапы работают на паузе
- `BlocksRowManager` получил поля `zoneCompletePopup` и `blocksPerZone` — каждые N блоков (10) показывает Zone Complete попап с +1 gem
- `SettingsPopup` в MainMenu теперь использует ConfirmPopup перед Reset Progress
- На game scene добавлены 4 новых canvas-попапа:
  - `PausePopup` (sortingOrder=200, pausesGame=true)
  - `GameSettingsPopup` (210)
  - `ZoneCompletePopup` (220, pausesGame=true)
  - `ConfirmPopup` (230)
- На MainMenu добавлен ConfirmPopup (canvas)
- Кнопка Pause в HUD теперь работает (PauseButtonHandler на Canvas)
- Создан вспомогательный `UIColors` (статические цвета) — runtime версия `UIBuildUtils` цветов

## 3. Editor скрипты — порядок запуска

В Unity:
1. `Tools / Merge Mining / (Iteration 4) Update ALL Scenes`

Альтернативно по отдельности:
- `Tools / Merge Mining / (Iteration 4) Update Game Scene`
- `Tools / Merge Mining / (Iteration 4) Update MainMenu Scene`

## 4. Как тестировать

### Pause flow:
1. Открой Game scene, жми Play
2. Тапни кнопку Pause (правый верх, иконка "II") → должен появиться большой попап PAUSED с тремя кнопками
3. Игра замирает: кирки не атакуют, блоки не дёргаются, coin burst анимации тоже на паузе
4. RESUME → попап исчезает, игра продолжается
5. Снова Pause → SETTINGS → откроется попап настроек поверх паузы. Игра ОСТАЁТСЯ на паузе
6. В Settings меняй тогглы/слайдеры → значения сохраняются
7. CLOSE Settings → возврат к Pause-попапу (игра всё ещё на паузе)
8. MAIN MENU → ConfirmPopup "QUIT TO MENU?" с красной кнопкой QUIT
9. CANCEL → возврат к Pause. QUIT → fade в MainMenu

### Zone Complete (демо):
1. На Game scene разрушай блоки (можно тапать или ждать пока кирки сами разобьют)
2. После каждого 10-го разрушенного блока через ~0.8s появляется большой попап ZONE COMPLETE с +1 GEM
3. Gem icon вращается, появляется с пружинкой
4. CONTINUE → попап закрывается, игра продолжается, +1 gem прилетает в счётчик
5. **Важно:** счётчик `total_blocks_destroyed` хранится в PlayerPrefs накопительно. Если у тебя уже за прошлые итерации разрушено 7 блоков — следующее Zone Complete будет на 10-м (т.е. через 3 блока), потом 20-м и т.д.

### Reset Progress confirmation:
1. На MainMenu → Settings → RESET PROGRESS → теперь появляется ConfirmPopup "RESET PROGRESS?" с красной кнопкой RESET
2. CANCEL → закрытие, прогресс цел
3. RESET → прогресс стирается, сцена перезагружается

## 5. Ожидаемый результат
- Все попапы анимируются (scale 0→1 с OutBack, backdrop fade), на паузе анимации работают через unscaled time
- timeScale корректно ставится на 0/1 при показе/скрытии паузо-попапов
- При переходе через несколько попапов timeScale не "зависает" в неправильном состоянии
- Цена SHOP и счётчики продолжают работать после возврата из пауза
- Кирки во время drag над попапом не таскаются (drag-handler даёт raycast только на свой Image, попап перекрывает их сверху)

## 6. Известные ограничения текущей итерации
- Zone Complete пока всегда даёт +1 gem (в иттер 5 будет привязка к реальным зонам с разной наградой)
- Нет анимации монет/gems прилёта при Zone Complete (только сам попап с гемом) — это можно добавить в иттер 9 polish
- Музыки нет — поэтому пауза музыки не нужна (но логика готова)
- Pause не блокирует ввод физически — это работает за счёт того что попап перекрывает экран полностью своим backdrop'ом

## 7. Что будет в следующей итерации
Реальные 5 зон с разными визуалами, прогрессом разблокировки и Zone Complete привязанный к зонам с растущей gem-наградой.
