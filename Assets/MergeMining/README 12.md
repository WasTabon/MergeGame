# Iteration 15 — Fixes + New Text Tutorial

## 1. Что было добавлено
Фикс ProgressText (0/0) и кнопки START (не пропадала после старта). Новый текстовый туториал из 5 шагов вместо старого с подсветкой.

## 2. Что изменилось с прошлой итерации

### Фиксы:
- `LevelManager.Awake` теперь инициализирует `CurrentLevelNumber` и `CurrentLevel` (раньше было в Start). Это решает race condition когда другие компоненты пытались прочитать данные до Start
- `LevelHud` переписан: подписка на события идёт через `TrySubscribe` который ретраится в Update. FullRefresh правильно обновит phase / ProgressText / startButton visibility
- `OnBlockProgress` теперь сам подхватывает total из LevelManager если параметр пустой
- Update в LevelHud правильно скрывает START кнопку при смене фазы (явная подписка → событие OnPhaseChanged → SetActive(false))

### Новый туториал:
- Создан `TextTutorialPopup` — popup на 5 шагов, каждый с заголовком и описанием
- Кнопка NEXT (на последнем шаге — "LET'S GO!")
- Индикатор шага "1 / 5"
- Появляется только при первом запуске игры **И** только когда CurrentLevelNumber == 1
- Сохраняется флаг `text_tutorial_done` — больше не появляется

### 5 шагов туториала:
1. WELCOME! — короткое intro
2. BUY PICKAXES — про кнопку SHOP и coin budget
3. MERGE PICKAXES — про drag&drop merge
4. START THE BATTLE — про зелёную кнопку START
5. WIN THE LEVEL — про разрушение блоков и звёзды

### Удалено:
- Старый `TutorialOverlayCanvas` если ещё остался (визуально не отображался, но мог быть на сцене)

## 3. Editor скрипт

`Tools / Merge Mining / (Iteration 15) Update Game Scene`

Также для тестирования:
`Tools / Merge Mining / (Iteration 15) DEBUG - Reset Tutorial` — сбросить флаг, чтобы туториал появился снова

## 4. Как тестировать

### Фиксы:
1. Открой Game scene → Play → ProgressText должен показывать "0/4" (или сколько блоков на уровне), не "0/0"
2. Купи кирки → жми START → кнопка START должна **исчезнуть** + текст фазы сменится на "BATTLE!"
3. Кирки бьют блоки → счётчик увеличивается 1/4, 2/4 и т.д.

### Туториал:
4. RESET PROGRESS в Settings → перезапусти сцену MainMenu
5. PLAY → LevelSelect → жми Level 1 → Game scene
6. Должен появиться попап HOW TO PLAY со шагом 1/5: "WELCOME!"
7. NEXT → шаг 2/5: "BUY PICKAXES"
8. Шаги 3, 4 → 5/5 "WIN THE LEVEL" — кнопка теперь "LET'S GO!"
9. Тап → попап закрывается → можно играть
10. Перезапусти сцену → туториал больше не появляется
11. На LevelSelect выбери любой другой уровень (после прохождения первого) → попап не появится

### Если хочешь снова увидеть туториал:
12. `Tools / Merge Mining / (Iteration 15) DEBUG - Reset Tutorial` → перезапусти сцену
13. Или RESET PROGRESS в Settings

## 5. Ожидаемый результат
- ProgressText показывает реальные числа
- Кнопка START скрывается при переходе в Battle
- Туториал появляется один раз на самом первом запуске уровня 1
- Туториал не повторяется на следующих уровнях

## 6. Известные ограничения
- Туториал не имеет анимации появления текста (просто меняется)
- Если игрок будет рестартить уровень 1 — туториал уже не появится (мы пометили его done после первого Continue)
