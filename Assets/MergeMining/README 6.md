# Iteration 8 — Tutorial + Offline Rewards

## 1. Что было добавлено
Туториал на 3 шага для новых игроков (Buy → Merge → Tap Block) с подсветкой целей и рукой-указателем. Offline Rewards: при возвращении в игру после паузы/выхода показывается попап "WELCOME BACK!" с подсчётом заработанных coins за время отсутствия.

## 2. Что изменилось с прошлой итерации
- На Canvas добавлены `TutorialManager` и `OfflineRewardsManager` (singletons)
- Появились новые попапы: `OfflineRewardsPopup` (sortingOrder=240), `TutorialOverlay` (245, не классический BasePopup, а свой CanvasGroup overlay)
- `ShopController` теперь вызывает `TutorialManager.NotifyShopPurchase()` после удачной покупки
- `Block.OnPointerClick` вызывает `TutorialManager.NotifyBlockTapped()` после тапа
- `PickaxeGridManager.OnMerged` событие используется TutorialManager автоматически для перехода с шага DragMerge на TapBlock
- На каждый MonoBehaviour создан отдельный файл (BoosterButtonView ранее был в общем файле — ошибка, поправлено в иттер 7 патчем)

## 3. Editor скрипты

`Tools / Merge Mining / (Iteration 8) Update Game Scene`

## 4. Как тестировать

### Tutorial (новый игрок):
1. Открой Game scene → Settings → RESET PROGRESS (или удали PlayerPrefs ключ `tutorial_done`)
2. Перезапусти сцену
3. После загрузки сцены должен появиться overlay с тёмной подложкой
4. На кнопке SHOP горит белый "halo" (highlight ring пульсирует), над ней рука-указатель прыгает вверх-вниз, снизу надпись "TAP TO BUY A PICKAXE"
5. Тапни SHOP → купи кирку → overlay переходит на сетку PickaxeGrid
6. Надпись "DRAG TWO PICKAXES TO MERGE", рука анимированно тащит из одной точки в другую (символизирует drag)
7. Сделай merge → переход на BlocksRow
8. Надпись "TAP THE BLOCK TO MINE"
9. Тапни 3 раза по блоку → туториал завершается, overlay плавно исчезает
10. После завершения PlayerPrefs `tutorial_done = 1` → при следующих запусках туториал не появляется
11. Можно скипнуть через кнопку SKIP в правом нижнем углу overlay

### Offline Rewards:
12. Поиграй немного (наработай хоть одну кирку) → выйди из игры (закрой приложение или останови Play в редакторе)
13. Подожди минимум 60 секунд
14. Запусти снова → должен появиться попап "WELCOME BACK!" с временем "AWAY FOR 1m" (или сколько прошло) и подсчётом заработанных coins
15. Внутри попапа крутится монета-иконка, "+N" coins большим шрифтом
16. Кнопка CLAIM → +N coins, попап закрывается
17. Кнопка x2 CLAIM → +2N coins (заглушка под ad-reward, можно вмонтировать AdMob в продакшен)
18. Если был офлайн меньше 60 секунд — попап не показывается

### Расчёт offline coins:
- Берётся DPS всех кирок на сетке (sum of damage × miningSpeed)
- Высчитывается сколько блоков можно разрушить за time-away (DPS / averageHP)
- Каждый блок даёт averageReward coins
- Множитель 0.5 (50% эффективности от полной активной игры) — балансово, чтобы не было дисбаланса
- Cap: максимум 4 часа

## 5. Ожидаемый результат
- Tutorial появляется ровно один раз при первом запуске игры
- Skip всегда доступен
- Highlight ring точно подстраивается под размер целевого элемента (Shop, Grid, BlocksRow)
- Offline rewards честно считаются от DPS на сетке
- Если игрок никогда не покупал кирок → DPS = 0 → offline reward = 0 → попап не показывается

## 6. Известные ограничения
- Tutorial overlay сам не блокирует ввод (CanvasGroup blocksRaycasts=false) — игрок может тапать сквозь подсветку. Это специально: иначе он не сможет выполнить шаг туториала
- Dimmer изначально alpha=0, без затемнения экрана. Если хочется реального dim — поднять alpha до 0.4 в editor скрипте
- Tutorial не сохраняет промежуточное состояние — если игрок выйдет в середине туториала, при возвращении он начнётся с шага TapShop заново

## 7. Что будет в следующей итерации
Полишер: screen shake/haptics везде, анимация прилёта currency, glow на merge, чистка попапов, juicy easing, DOCounter.
